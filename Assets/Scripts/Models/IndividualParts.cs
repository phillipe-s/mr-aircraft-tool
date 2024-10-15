using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
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
    [SerializeField] private GameObject grabInteractions;
    [SerializeField] private GameObject grabInteractionsPrefab;
    private Dictionary<Transform, GameObject> grabInteractionsDict = new Dictionary<Transform, GameObject>();
    [SerializeField] private GameObject rayGrabInteractionPrefab;
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
                ToggleInteractions(child, true);
            }
            grabInteractions.SetActive(false);
        }
        else
        {
            if (!isExploded) RestorePartTransforms();
            foreach (Transform child in nestedParts)
            {
                ToggleInteractions(child, false);
            }
            grabInteractions.SetActive(true);
        }

        enableIndividualParts = !enableIndividualParts;
        Debug.Log($"Individual parts toggled to {enableIndividualParts}");
    }

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
            isExploded = true;
            SavePartPositionsBeforeExplode();
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
            AddRayGrabInteraction(part);
            AddGrabInteractions(part);
            // AddNetworkComponents(part);
        }
    }

    private void AddRayGrabInteraction(Transform child)
    {
        GameObject rayGrabInteraction = Instantiate(rayGrabInteractionPrefab, child);
        rayGrabInteraction.SetActive(false);
        rayGrabInteractionsDict[child] = rayGrabInteraction;

        rayGrabInteraction.GetComponent<Grabbable>().InjectOptionalTargetTransform(child);
        rayGrabInteraction.GetComponent<ColliderSurface>().InjectAllColliderSurface(child.gameObject.GetComponent<Collider>());
        rayGrabInteraction.GetComponent<MaterialPropertyBlockEditor>().Renderers = new List<Renderer> { child.gameObject.GetComponent<Renderer>() };
        rayGrabInteraction.GetComponent<RayInteractable>().enabled = false;

        rayGrabInteraction.SetActive(true);
    }

    private void AddGrabInteractions(Transform child)
    {
        GameObject grabInteractions = Instantiate(grabInteractionsPrefab, child);
        grabInteractions.SetActive(false);
        grabInteractionsDict[child] = grabInteractions;

        grabInteractions.GetComponent<Grabbable>().InjectOptionalTargetTransform(child);
        grabInteractions.GetComponent<Grabbable>().InjectOptionalRigidbody(null);
        grabInteractions.GetComponent<HandGrabInteractable>().InjectRigidbody(null);
    }

    // private void AddNetworkComponents(Transform child)
    // {
    //     // !IMPORTANT: Each part must be 'baked' with a NetworkObject component (added through the Unity Editor)
    //     // - Allow State Authoirty Override = false - current implementation does not support this
    //     // - Destroy When State Authority Leaves = false 
    //     // The part must also have a NetworkTransform component so that it is added to the NetworkBehaviour list
    //     // NetworkObject: https://doc.photonengine.com/fusion/current/manual/network-object
    //     // NetworkBehaviour: https://doc.photonengine.com/fusion/current/manual/network-behaviour

    //     NetworkTransform networkTransform = child.gameObject.AddComponent<NetworkTransform>();
    //     networkTransform.SyncScale = true;
    //     networkTransform.SyncParent = false;
    //     networkTransform.AutoUpdateAreaOfInterestOverride = true;
    //     networkTransform.DisableSharedModeInterpolation = true;

    //     // child.gameObject.AddComponent<TransferOwnershipFusion>().enabled = false;
    //     // child.gameObject.AddComponent<TransferOwnershipOnSelect>().enabled = false;

    //     // Get the NetworkObject component from the child object
    //     NetworkObject networkObject = child.gameObject.GetComponent<NetworkObject>();
    // }

    // private void ToggleTransferOwnership(Transform part, bool state)
    // {
    //     part.gameObject.GetComponent<TransferOwnershipFusion>().enabled = state;
    //     part.gameObject.GetComponent<TransferOwnershipOnSelect>().enabled = state;
    // }

    private void ToggleInteractions(Transform part, bool state)
    {
        GameObject rayGrabInteraction = rayGrabInteractionsDict[part];
        rayGrabInteraction.GetComponent<RayInteractable>().enabled = state;

        GameObject grabInteractions = grabInteractionsDict[part];
        if (state)
        {
            Rigidbody rigidbody = part.gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;

            grabInteractions.GetComponent<Grabbable>().InjectOptionalRigidbody(rigidbody);
            grabInteractions.GetComponent<HandGrabInteractable>().InjectRigidbody(rigidbody);
            grabInteractions.GetComponent<GrabInteractable>().InjectRigidbody(rigidbody);
            grabInteractions.SetActive(true);
        }
        else
        {
            Destroy(part.gameObject.GetComponent<RigidbodyKinematicLocker>());
            grabInteractions.GetComponent<Grabbable>().InjectOptionalRigidbody(null);
            grabInteractions.GetComponent<HandGrabInteractable>().InjectRigidbody(null);
            grabInteractions.GetComponent<GrabInteractable>().InjectRigidbody(null);
            Destroy(part.gameObject.GetComponent<Rigidbody>());
            grabInteractions.SetActive(false);
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
