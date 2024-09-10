using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelsController : MonoBehaviour
{
    public List<GameObject> models;
    private int currentModelIndex = 0;

    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        models[currentModelIndex].SetActive(true);
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start)) ToggleIndividualParts();
    }

    public void SwitchToModel(GameObject model)
    {
        models[currentModelIndex].SetActive(false);
        currentModelIndex = models.IndexOf(model);
        models[currentModelIndex].SetActive(true);
    }

    [ContextMenu("Switch Model")]
    private void SwitchToModel()
    {
        models[currentModelIndex].SetActive(false);
        currentModelIndex = currentModelIndex + 1;
        models[currentModelIndex].SetActive(true);
    }

    [ContextMenu("Toggle Individual Parts For Current Model")]
    public void ToggleIndividualParts()
    {
        models[currentModelIndex].GetComponentInChildren<IndividualPartsToggler>().ToggleIndividualParts();
    }
}