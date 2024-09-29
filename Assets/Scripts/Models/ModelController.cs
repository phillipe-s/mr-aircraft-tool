using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    public List<Model> models;
    private Model currentModel;
    public Model CurrentModel { get => currentModel; }

    void Awake()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        currentModel = models[0];
        currentModel.gameObject.SetActive(true);
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start)) ToggleIndividualParts();
    }

    public void SwitchToModel(Model model)
    {
        currentModel.gameObject.SetActive(false);
        currentModel = model;
        currentModel.gameObject.SetActive(true);
    }

    [ContextMenu("Toggle Individual Parts For Current Model")]
    public void ToggleIndividualParts()
    {
        currentModel.ToggleIndividualParts();
    }

    // ======================== TESTING FUNCTIONS ======================== //

    [ContextMenu("[TESTING ONLY] Switch to Model at index 1")]
    private void SwitchToModel()
    {
        currentModel.gameObject.SetActive(false);
        currentModel = models[1];
        currentModel.gameObject.SetActive(true);
    }

}