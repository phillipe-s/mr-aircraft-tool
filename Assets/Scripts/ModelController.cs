using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.UIElements;

public class ModelController : MonoBehaviour
{
    private GameObject model;
    private bool individualParts = false;
    private Dictionary<Transform, (Vector3 position, Quaternion rotation)> savedPartTransforms = new Dictionary<Transform, (Vector3, Quaternion)>();

    private void Start()
    {
        loadModel("C130");
    }

    // Update is called once per frame
    private void Update()
    {
        // if the start button is pressed with the hands
        if (OVRInput.GetDown(OVRInput.Button.Start)) ToggleIndividualParts();

    }

    private void loadModel(string modelName)
    {
        model = Instantiate(Resources.Load<GameObject>("Models/" + modelName), transform);

        GameObject rayGrabInteractionPrefab = Resources.Load<GameObject>("RayGrabInteraction");
        rayGrabInteractionPrefab.SetActive(false);

        foreach (Transform child in model.transform)
        {
            GameObject rayGrabInteraction = Instantiate(rayGrabInteractionPrefab, child);
            rayGrabInteraction.GetComponent<Grabbable>().InjectOptionalTargetTransform(child);
            rayGrabInteraction.GetComponent<ColliderSurface>().InjectAllColliderSurface(child.gameObject.GetComponent<Collider>());
        }
    }

    private void ToggleRayGrabInteraction(Transform child, bool state)
    {
        GameObject rayGrabInteraction = child.gameObject.transform.Find("RayGrabInteraction(Clone)").gameObject;
        rayGrabInteraction.SetActive(state);
    }

    private void SavePartTransforms()
    {
        savedPartTransforms.Clear();
        foreach (Transform child in model.transform)
        {
            savedPartTransforms.Add(child, (child.position, child.rotation));
            ToggleRayGrabInteraction(child, true);
        }
    }

    private void RestorePartTransforms()
    {
        foreach (Transform child in model.transform)
        {
            (Vector3 position, Quaternion rotation) = savedPartTransforms[child];
            child.position = position;
            child.rotation = rotation;
            ToggleRayGrabInteraction(child, false);
        }
    }

    private void ToggleIndividualParts()
    {
        if (!individualParts)
        {
            SavePartTransforms();
        }
        else
        {
            RestorePartTransforms();
        }

        individualParts = !individualParts;
        Debug.Log("Individual parts toggled");
    }
}
