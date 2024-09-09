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


    [ContextMenu("Next Model")]
    public void NextModel()
    {
        SwitchToModel((currentModelIndex + 1) % models.Count);
    }

    [ContextMenu("Previous Model")]
    public void PreviousModel()
    {
        SwitchToModel((currentModelIndex - 1 + models.Count) % models.Count);
    }

    public void SwitchToModel(int index)
    {
        if (index < 0 || index >= models.Count)
        {
            Debug.LogError("Index out of bounds");
            return;
        }

        models[currentModelIndex].SetActive(false);
        models[index].SetActive(true);
        currentModelIndex = index;
    }

    [ContextMenu("Toggle Individual Parts For Current Model")]
    public void ToggleIndividualParts()
    {
        models[currentModelIndex].GetComponentInChildren<IndividualPartsToggler>().ToggleIndividualParts();
    }
}