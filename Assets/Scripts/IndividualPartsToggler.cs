using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;

public class IndividualPartsToggler : MonoBehaviour
{
    private bool enableIndividualParts = false;
    [SerializeField] private GameObject parts;
    [SerializeField] private GameObject rayGrabInteractionPrefab;
    private Dictionary<Transform, (Vector3 position, Quaternion rotation)> savedPartTransforms = new Dictionary<Transform, (Vector3, Quaternion)>();
    private Dictionary<Transform, GameObject> rayGrabInteractions = new Dictionary<Transform, GameObject>();

    void Start()
    {
        AttachRayGrabInteractionToParts();
    }

    [ContextMenu("Toggle Individual Parts")]
    public void ToggleIndividualParts()
    {
        if (!enableIndividualParts)
        {
            SavePartTransforms();
        }
        else
        {
            RestorePartTransforms();
        }

        enableIndividualParts = !enableIndividualParts;
        Debug.Log($"Individual parts toggled to {enableIndividualParts}");
    }

    private void AttachRayGrabInteractionToParts()
    {
        rayGrabInteractionPrefab.SetActive(false);

        foreach (Transform child in parts.transform)
        {
            GameObject rayGrabInteraction = Instantiate(rayGrabInteractionPrefab, child);
            rayGrabInteraction.GetComponent<Grabbable>().InjectOptionalTargetTransform(child);
            rayGrabInteraction.GetComponent<ColliderSurface>().InjectAllColliderSurface(child.gameObject.GetComponent<Collider>());
            rayGrabInteraction.GetComponent<MaterialPropertyBlockEditor>().Renderers = new List<Renderer> { child.gameObject.GetComponent<Renderer>() };

            // Store the reference to the instantiated rayGrabInteraction
            rayGrabInteractions[child] = rayGrabInteraction;
        }
    }

    private void ToggleRayGrabInteraction(Transform part, bool state)
    {
        GameObject rayGrabInteraction = rayGrabInteractions[part];
        rayGrabInteraction.GetComponent<InteractableColorVisual>().enabled = state;
        rayGrabInteraction.SetActive(state);
    }

    private void SavePartTransforms()
    {
        savedPartTransforms.Clear();
        foreach (Transform child in parts.transform)
        {
            savedPartTransforms.Add(child, (child.position, child.rotation));
            ToggleRayGrabInteraction(child, true);
        }
    }

    private void RestorePartTransforms()
    {
        foreach (Transform child in parts.transform)
        {
            (Vector3 position, Quaternion rotation) = savedPartTransforms[child];
            child.position = position;
            child.rotation = rotation;
            ToggleRayGrabInteraction(child, false);
        }
    }
}
