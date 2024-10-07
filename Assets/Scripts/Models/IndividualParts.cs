using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Surfaces;
using Oculus.Interaction.Throw;
using UnityEngine;

public class IndividualParts : MonoBehaviour
{
    private bool enableIndividualParts = false;
    [SerializeField] private GameObject grabInteractions;
    public bool IndividualPartsEnabled { get => enableIndividualParts; }
    [SerializeField] private GameObject rayGrabInteractionPrefab;
    private Dictionary<Transform, GameObject> rayGrabInteractions = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, (Vector3 position, Quaternion rotation, Vector3 scale)> savedPartTransforms = new Dictionary<Transform, (Vector3, Quaternion, Vector3)>();
    private bool isExploded = true;

    void Start()
    {
        AttachRayGrabInteractionToParts();
        SavePartTransforms();
    }

    [ContextMenu("Toggle Individual Parts")]
    public void ToggleIndividualParts()
    {
        if (!enableIndividualParts)
        {
            foreach (Transform child in transform) ToggleRayGrabInteraction(child, true);
            grabInteractions.SetActive(false);
        }
        else
        {
            if (!isExploded) RestorePartTransforms();
            foreach (Transform child in transform) ToggleRayGrabInteraction(child, false);
            grabInteractions.SetActive(true);
        }

        enableIndividualParts = !enableIndividualParts;
        Debug.Log($"Individual parts toggled to {enableIndividualParts}");
    }

    public void RestorePartTransforms()
    {
        // Save the current position of the parent object
        Vector3 positionBeforeRestore = transform.parent.position;

        foreach (Transform child in transform)
        {
            (Vector3 localPosition, Quaternion localRotation, Vector3 localScale) = savedPartTransforms[child];
            child.localPosition = localPosition;
            child.localRotation = localRotation;
            child.localScale = localScale;
        }

        // Restore the parent object's position to its position before the restore
        transform.parent.position = positionBeforeRestore;
    }

    public void Explode(float value)
    {
        if ((value > 0) && !isExploded) isExploded = true;
        else if (value == 0)
        {
            RestorePartTransforms();
            isExploded = false;
        }
        else
        {
            foreach (MeshRenderer part in GetComponentsInChildren<MeshRenderer>())
            {
                Vector3 explodedPosition = part.bounds.center;
                part.transform.position = Vector3.Lerp(transform.parent.position, explodedPosition, value);
            }
        }
    }

    private void AttachRayGrabInteractionToParts()
    {
        rayGrabInteractionPrefab.SetActive(false);

        foreach (Transform child in transform)
        {
            GameObject rayGrabInteraction = Instantiate(rayGrabInteractionPrefab, child);
            rayGrabInteractions[child] = rayGrabInteraction;

            rayGrabInteraction.GetComponent<Grabbable>().InjectOptionalTargetTransform(child);
            rayGrabInteraction.GetComponent<ColliderSurface>().InjectAllColliderSurface(child.gameObject.GetComponent<Collider>());
            rayGrabInteraction.GetComponent<MaterialPropertyBlockEditor>().Renderers = new List<Renderer> { child.gameObject.GetComponent<Renderer>() };
            rayGrabInteraction.GetComponent<RayInteractable>().enabled = false;
            rayGrabInteraction.SetActive(true);
        }
    }

    private void ToggleRayGrabInteraction(Transform part, bool state)
    {
        GameObject rayGrabInteraction = rayGrabInteractions[part];
        rayGrabInteraction.GetComponent<RayInteractable>().enabled = state;
    }


    private void SavePartTransforms()
    {
        foreach (Transform child in transform)
        {
            savedPartTransforms.Add(child, (child.localPosition, child.localRotation, child.localScale));
        }
    }
}
