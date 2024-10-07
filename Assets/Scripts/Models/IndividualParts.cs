using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Surfaces;
using UnityEngine;

public class IndividualParts : MonoBehaviour
{
    private bool enableIndividualParts = false;
    [SerializeField] private GameObject grabInteractions;
    public bool IndividualPartsEnabled { get => enableIndividualParts; }
    [SerializeField] private GameObject rayGrabInteractionPrefab;
    private Dictionary<Transform, GameObject> rayGrabInteractions = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, (Vector3 position, Quaternion rotation, Vector3 scale)> savedPartTransforms = new Dictionary<Transform, (Vector3, Quaternion, Vector3)>();
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();

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
            grabInteractions.SetActive(false);
        }
        else
        {
            RestorePartTransforms();
            grabInteractions.SetActive(true);
        }

        enableIndividualParts = !enableIndividualParts;
        Debug.Log($"Individual parts toggled to {enableIndividualParts}");
    }

    public void RestorePartTransforms()
    {
        foreach (Transform child in transform)
        {
            (Vector3 position, Quaternion rotation, Vector3 scale) = savedPartTransforms[child];
            child.position = position;
            child.rotation = rotation;
            child.localScale = scale;
            ToggleRayGrabInteraction(child, false);
        }
    }

    public void Explode(float value)
    {
        foreach (var part in GetComponentsInChildren<MeshRenderer>())
        {
            if (!originalPositions.ContainsKey(part.transform))
            {
                originalPositions[part.transform] = part.transform.position;
            }

            Vector3 explodedPosition = part.bounds.center;
            part.transform.position = Vector3.Lerp(originalPositions[part.transform], explodedPosition, value);
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
        savedPartTransforms.Clear();
        foreach (Transform child in transform)
        {
            savedPartTransforms.Add(child, (child.position, child.rotation, child.localScale));
            ToggleRayGrabInteraction(child, true);
        }
    }
}
