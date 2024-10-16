using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using JetBrains.Annotations;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using Meta.XR.MultiplayerBlocks.Fusion;
using Meta.XR.MultiplayerBlocks.Shared;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Surfaces;
using Oculus.Interaction.Throw;
using UnityEngine;

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

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ToggleIndividualParts()
    {
        if (!enableIndividualParts)
        {
            foreach (Transform child in nestedParts)
            {
                if (rayGrabInteractionsEnabled) ToggleRayInteraction(child, true);
                ToggleGrabInteraction(child, true);
            }
            parentModelGrabInteractions.GetComponent<GrabInteractable>().enabled = false;
        }
        else
        {
            if (!isExploded) RestorePartTransforms();
            foreach (Transform child in nestedParts)
            {
                if (!rayGrabInteractionsEnabled) ToggleGrabInteraction(child, false);
                ToggleRayInteraction(child, false);
            }
            parentModelGrabInteractions.GetComponent<GrabInteractable>().enabled = false;
        }

        enableIndividualParts = !enableIndividualParts;
        Debug.Log($"Individual parts toggled to {enableIndividualParts}");
    }

    [ContextMenu("Toggle Ray Grab Interactions")]
    public void ToggleRayGrabInteractions()
    {
        rayGrabInteractionsEnabled = !rayGrabInteractionsEnabled;
        foreach (Transform child in nestedParts)
        {
            ToggleRayInteraction(child, rayGrabInteractionsEnabled);
        }
    }

    [ContextMenu("Restore Part Transforms")]
    public void RestorePartTransforms() { RPC_RestorePartTransforms(); }

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


    public void Explode(float value) { RPC_Explode(value); }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_Explode(float value)
    {
        if ((value > 0) && !isExploded)
        {
            SavePartPositionsBeforeExplode();
            isExploded = true;
        }
        else if (value == 0)
        {
            RestorePartTransforms();
            isExploded = false;
        }
        else if (transform.parent.position != positionBeforeExplode)
        {
            RestorePartTransforms();
            SavePartPositionsBeforeExplode();
        }
        else
        {
            foreach (MeshRenderer part in GetComponentsInChildren<MeshRenderer>())
            {
                Vector3 explodedPosition = part.bounds.center;
                part.transform.position = Vector3.Lerp(partPositionsBeforeExplode[part.transform], explodedPosition, value);
            }
        }
    }

    private void AddComponentsToParts()
    {
        nestedParts = GetAllNestedObjectsWithMeshComponents(transform);

        foreach (Transform part in nestedParts)
        {
            AddGrabInteractions(part);
            AddRayGrabInteraction(part);
        }
    }

    private void AddGrabInteractions(Transform child)
    {
        GameObject grabInteractions = Instantiate(grabInteractionsPrefab, child);
        grabInteractionsDict[child] = grabInteractions;

        grabInteractions.GetComponent<GrabInteractable>().enabled = false;
        grabInteractions.GetComponent<HandGrabInteractable>().enabled = false;
        grabInteractions.GetComponent<Grabbable>().InjectOptionalTargetTransform(child);
        grabInteractions.GetComponent<Grabbable>().InjectOptionalRigidbody(null);
        grabInteractions.GetComponent<HandGrabInteractable>().InjectRigidbody(null);
        grabInteractions.GetComponent<GrabInteractable>().InjectRigidbody(null);
        grabInteractions.GetComponent<MaterialPropertyBlockEditor>().Renderers = new List<Renderer> { child.gameObject.GetComponent<Renderer>() };
    }

    private void AddRayGrabInteraction(Transform child)
    {
        GameObject rayGrabInteraction = Instantiate(rayGrabInteractionPrefab, child);
        rayGrabInteractionsDict[child] = rayGrabInteraction;

        rayGrabInteraction.GetComponent<Grabbable>().InjectOptionalTargetTransform(child);
        rayGrabInteraction.GetComponent<ColliderSurface>().InjectAllColliderSurface(child.gameObject.GetComponent<Collider>());
        rayGrabInteraction.GetComponent<RayInteractable>().enabled = false;
        rayGrabInteraction.GetComponent<InteractableColorVisual>().InjectMaterialPropertyBlockEditor(grabInteractionsDict[child].GetComponent<MaterialPropertyBlockEditor>());
    }

    private void ToggleRayInteraction(Transform part, bool state)
    {
        GameObject rayGrabInteraction = rayGrabInteractionsDict[part];
        rayGrabInteraction.GetComponent<RayInteractable>().enabled = state;
    }

    private void ToggleGrabInteraction(Transform part, bool state)
    {
        GameObject grabInteractions = grabInteractionsDict[part];
        Grabbable grabbable = grabInteractions.GetComponent<Grabbable>();
        HandGrabInteractable handGrabInteractable = grabInteractions.GetComponent<HandGrabInteractable>();
        GrabInteractable grabInteractable = grabInteractions.GetComponent<GrabInteractable>();

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
        else
        {
            grabInteractionsDict.Remove(part);
            Destroy(grabInteractions);
            Destroy(part.gameObject.GetComponent<RigidbodyKinematicLocker>());
            Destroy(part.gameObject.GetComponent<Rigidbody>());
            AddGrabInteractions(part);
        }
    }

    private List<Transform> GetAllNestedObjectsWithMeshComponents(Transform parent)
    {
        List<Transform> result = new List<Transform>();

        foreach (Transform child in parent)
        {
            if ((child.GetComponent<Collider>() != null) && (child.GetComponent<Renderer>() != null))
            {
                result.Add(child);
            }

            result.AddRange(GetAllNestedObjectsWithMeshComponents(child));
        }

        return result;
    }

    private void SavePartTransforms()
    {
        foreach (Transform child in nestedParts)
        {
            savedPartTransforms.Add(child, (child.localPosition, child.localRotation, child.localScale));
        }
    }

    private void SavePartPositionsBeforeExplode()
    {
        positionBeforeExplode = transform.parent.position;
        partPositionsBeforeExplode.Clear();
        foreach (Transform child in nestedParts)
        {
            partPositionsBeforeExplode.Add(child, child.position);
        }
    }
}
