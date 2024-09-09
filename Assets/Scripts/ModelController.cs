using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    private GameObject model;
    private bool individualParts = false;
    public bool IndividualParts { get { return individualParts; } }
    private Dictionary<Transform, (Vector3 position, Quaternion rotation)> savedPartTransforms = new Dictionary<Transform, (Vector3, Quaternion)>();


    private void Start()
    {
        LoadModel(Resources.Load<GameObject>("Models/C130"));
    }

    private void Update()
    {
        if (Input.GetKey("p"))
        {
            SwitchModel(Resources.Load<GameObject>("Models/UH 60 blackhawk"));
        }
    }

    private void LoadModel(GameObject model)
    {
        this.model = Instantiate(model, transform);

        GameObject rayGrabInteractionPrefab = Resources.Load<GameObject>("RayGrabInteraction");
        rayGrabInteractionPrefab.SetActive(false);

        foreach (Transform child in this.model.transform)
        {
            GameObject rayGrabInteraction = Instantiate(rayGrabInteractionPrefab, child);
            rayGrabInteraction.GetComponent<Grabbable>().InjectOptionalTargetTransform(child);
            rayGrabInteraction.GetComponent<ColliderSurface>().InjectAllColliderSurface(child.gameObject.GetComponent<Collider>());
            rayGrabInteraction.GetComponent<MaterialPropertyBlockEditor>().Renderers = new List<Renderer> { child.gameObject.GetComponent<Renderer>() };
        }
    }


    public void SwitchModel(GameObject model)
    {
        Destroy(this.model);
        LoadModel(model);
    }


    private void ToggleRayGrabInteraction(Transform child, bool state)
    {
        GameObject rayGrabInteraction = child.gameObject.transform.Find("RayGrabInteraction(Clone)").gameObject;
        rayGrabInteraction.GetComponent<InteractableColorVisual>().enabled = state;
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

    public void ToggleIndividualParts()
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
