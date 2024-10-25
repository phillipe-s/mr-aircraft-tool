using System.Collections.Generic;
using Fusion;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Surfaces;
using UnityEngine;

/// <summary>
/// Manages the individual parts of a model, allowing for interactions such as grabbing and exploding.
/// </summary>
public class IndividualParts : NetworkBehaviour
{
    private bool enableIndividualParts = false;
    public bool IndividualPartsEnabled { get => enableIndividualParts; }
    private List<Transform> nestedParts = new List<Transform>();
    [SerializeField] private GameObject parentModelGrabInteractions;
    [SerializeField] private GameObject grabInteractionsPrefab;
    private Dictionary<Transform, GameObject> grabInteractionsDict = new Dictionary<Transform, GameObject>();
    [SerializeField] private GameObject rayGrabInteractionPrefab;
    private bool rayGrabInteractionsEnabled = false;
    public bool RayGrabInteractionsEnabled { get => rayGrabInteractionsEnabled; }
    private Dictionary<Transform, GameObject> rayGrabInteractionsDict = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, (Vector3 position, Quaternion rotation, Vector3 scale)> savedPartTransforms = new Dictionary<Transform, (Vector3, Quaternion, Vector3)>();
    private bool isExploded = false;
    private Vector3 positionBeforeExplode;
    private Dictionary<Transform, Vector3> partPositionsBeforeExplode = new Dictionary<Transform, Vector3>();

    void Awake()
    {
        AddComponentsToParts();
    }

    void Start()
    {
        SavePartTransforms();
        SavePartPositionsBeforeExplode();
    }

    [ContextMenu("Toggle Individual Parts")]
    public void ToggleIndividualParts() { RPC_ToggleIndividualParts(); }

    /// <summary>
    /// RPC method to toggle the individual parts.
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ToggleIndividualParts()
    {
        // If individual parts are disabled, enable them
        if (!enableIndividualParts)
        {
            foreach (Transform child in nestedParts)
            {
                if (rayGrabInteractionsEnabled) ToggleRayInteraction(child, true);
                ToggleGrabInteraction(child, true);
            }
            parentModelGrabInteractions.GetComponent<GrabInteractable>().enabled = false;
        }
        // If individual parts are enabled, disable them and 
        // reset the model only if it is not in an exploded state
        else
        {
            if (!isExploded) RestorePartTransforms();
            foreach (Transform child in nestedParts)
            {
                if (!rayGrabInteractionsEnabled) ToggleRayInteraction(child, false);
                ToggleGrabInteraction(child, false);
            }
            parentModelGrabInteractions.GetComponent<GrabInteractable>().enabled = false;
        }

        enableIndividualParts = !enableIndividualParts;
        Debug.Log($"Individual parts toggled to {enableIndividualParts}");
    }

    /// <summary>
    /// Toggles the ray grab interactions.
    /// </summary>
    [ContextMenu("Toggle Ray Grab Interactions")]
    public void ToggleRayGrabInteractions()
    {
        rayGrabInteractionsEnabled = !rayGrabInteractionsEnabled;
        foreach (Transform child in nestedParts)
            ToggleRayInteraction(child, rayGrabInteractionsEnabled);
    }

    [ContextMenu("Restore Part Transforms")]
    public void RestorePartTransforms() { RPC_RestorePartTransforms(); }

    /// <summary>
    /// Restores the transforms of the parts to their saved states.
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_RestorePartTransforms()
    {
        // Save the current position of the parent object
        Vector3 positionBeforeRestore = transform.parent.position;

        foreach (Transform child in nestedParts)
        {
            (Vector3 localPosition, Quaternion localRotation, Vector3 localScale) = savedPartTransforms[child];
            child.localPosition = localPosition;
            child.localRotation = localRotation;
            child.localScale = localScale;
        }

        // Restore the parent object's position to its position before the restore
        transform.parent.position = positionBeforeRestore;
    }

    /// <summary>
    /// Explodes the model based on the given value.
    /// </summary>
    /// <param name="value">The explosion value.</param>
    public void Explode(float value) { RPC_Explode(value); }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_Explode(float value)
    {
        // If the model is going to be exploded, save the part positions before exploding
        if ((value > 0) && !isExploded)
        {
            SavePartPositionsBeforeExplode();
            isExploded = true;
        }
        // If the model explosion is being reset, restore the part positions
        else if (value == 0)
        {
            RestorePartTransforms();
            isExploded = false;
        }
        // If the position of the parent object has changed, save the new part positions before exploding
        else if (transform.parent.position != positionBeforeExplode)
        {
            RestorePartTransforms();
            SavePartPositionsBeforeExplode();
        }
        else
        {
            // Linearly interpolate the position of each part to its exploded position
            foreach (MeshRenderer part in GetComponentsInChildren<MeshRenderer>())
            {
                Vector3 explodedPosition = part.bounds.center; // The center of the part's bounds is the exploded position
                part.transform.position = Vector3.Lerp(partPositionsBeforeExplode[part.transform], explodedPosition, value);
            }
        }
    }

    /// <summary>
    /// Adds necessary components to the parts for interactions.
    /// </summary>
    private void AddComponentsToParts()
    {
        nestedParts = GetAllNestedObjectsWithMeshComponents(transform);

        foreach (Transform part in nestedParts)
        {
            AddGrabInteractions(part);
            AddRayGrabInteraction(part);
        }
    }

    /// <summary>
    /// Adds grab interactions to a part.
    /// </summary>
    /// <param name="child">The part to add grab interactions to.</param>
    private void AddGrabInteractions(Transform child)
    {
        GameObject grabInteractions = Instantiate(grabInteractionsPrefab, child);
        grabInteractionsDict[child] = grabInteractions;

        // Inject the necessary components into the grab interactions
        grabInteractions.GetComponent<GrabInteractable>().enabled = false;
        grabInteractions.GetComponent<HandGrabInteractable>().enabled = false;
        grabInteractions.GetComponent<Grabbable>().InjectOptionalTargetTransform(child);
        grabInteractions.GetComponent<Grabbable>().InjectOptionalRigidbody(null);
        grabInteractions.GetComponent<HandGrabInteractable>().InjectRigidbody(null);
        grabInteractions.GetComponent<GrabInteractable>().InjectRigidbody(null);
        grabInteractions.GetComponent<MaterialPropertyBlockEditor>().Renderers = new List<Renderer> { child.gameObject.GetComponent<Renderer>() };
    }

    /// <summary>
    /// Adds ray grab interactions to a part.
    /// </summary>
    /// <param name="child">The part to add ray grab interactions to.</param>
    private void AddRayGrabInteraction(Transform child)
    {
        GameObject rayGrabInteraction = Instantiate(rayGrabInteractionPrefab, child);
        rayGrabInteractionsDict[child] = rayGrabInteraction;

        // Inject the necessary components into the ray grab interaction
        rayGrabInteraction.GetComponent<Grabbable>().InjectOptionalTargetTransform(child);
        rayGrabInteraction.GetComponent<ColliderSurface>().InjectAllColliderSurface(child.gameObject.GetComponent<Collider>());
        rayGrabInteraction.GetComponent<RayInteractable>().enabled = false;
        rayGrabInteraction.GetComponent<InteractableColorVisual>().InjectMaterialPropertyBlockEditor(grabInteractionsDict[child].GetComponent<MaterialPropertyBlockEditor>());
    }

    /// <summary>
    /// Toggles the ray interaction for a part.
    /// </summary>
    /// <param name="part">The part to toggle ray interaction for.</param>
    /// <param name="state">The state to set the ray interaction to.</param>
    private void ToggleRayInteraction(Transform part, bool state)
    {
        GameObject rayGrabInteraction = rayGrabInteractionsDict[part];
        rayGrabInteraction.GetComponent<RayInteractable>().enabled = state;
    }

    /// <summary>
    /// Toggles the grab interaction for a part.
    /// </summary>
    /// <param name="part">The part to toggle grab interaction for.</param>
    /// <param name="state">The state to set the grab interaction to.</param>
    private void ToggleGrabInteraction(Transform part, bool state)
    {
        GameObject grabInteractions = grabInteractionsDict[part];
        Grabbable grabbable = grabInteractions.GetComponent<Grabbable>();
        HandGrabInteractable handGrabInteractable = grabInteractions.GetComponent<HandGrabInteractable>();
        GrabInteractable grabInteractable = grabInteractions.GetComponent<GrabInteractable>();

        // If the grab interaction is being enabled, add a rigidbody and inject it into the grab interactions
        if (state)
        {
            Rigidbody rigidbody = part.gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;

            grabbable.InjectOptionalRigidbody(rigidbody);
            handGrabInteractable.InjectRigidbody(rigidbody);
            grabInteractable.InjectRigidbody(rigidbody);
            grabInteractable.enabled = true;
            handGrabInteractable.enabled = true;
        }
        // If the grab interaction is being disabled, remove the rigidbody and destroy the grab interactions
        else
        {
            grabInteractionsDict.Remove(part);
            Destroy(grabInteractions);
            Destroy(part.gameObject.GetComponent<RigidbodyKinematicLocker>());
            Destroy(part.gameObject.GetComponent<Rigidbody>());
            AddGrabInteractions(part);
        }
    }

    /// <summary>
    /// Recursively gets all nested objects with mesh components (i.e. the individual parts). 
    /// </summary>
    /// <param name="parent">The parent transform to search within.</param>
    /// <returns>A list of nested objects with mesh components.</returns>
    private List<Transform> GetAllNestedObjectsWithMeshComponents(Transform parent)
    {
        List<Transform> result = new List<Transform>();

        // If the parent has a collider and a renderer, add it to the list
        foreach (Transform child in parent)
        {
            if ((child.GetComponent<Collider>() != null) && (child.GetComponent<Renderer>() != null))
                result.Add(child);

            // Recursively search for nested objects with mesh components
            result.AddRange(GetAllNestedObjectsWithMeshComponents(child));
        }

        return result;
    }

    /// <summary>
    /// Saves the transforms of the parts.
    /// </summary>
    private void SavePartTransforms()
    {
        foreach (Transform child in nestedParts)
            savedPartTransforms.Add(child, (child.localPosition, child.localRotation, child.localScale));
    }

    /// <summary>
    /// Saves the positions of the parts before the model is exploded.
    /// </summary>
    private void SavePartPositionsBeforeExplode()
    {
        positionBeforeExplode = transform.parent.position;
        partPositionsBeforeExplode.Clear();
        foreach (Transform child in nestedParts)
            partPositionsBeforeExplode.Add(child, child.position);
    }
}
