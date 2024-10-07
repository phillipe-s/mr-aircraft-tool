using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class ModelController : MonoBehaviour
{
    public List<Model> models;
    private Model currentModel;
    public Model CurrentModel { get => currentModel; }
    [SerializeField] UIController uiController;



    void Awake()
    {
        foreach (Model model in models)
        {
            model.gameObject.SetActive(false);
        }
        currentModel = models[0];
        currentModel.gameObject.SetActive(true);
    }

    public void SwitchToModel(Model model)
    {
        currentModel.gameObject.SetActive(false);
        currentModel = model;
        currentModel.gameObject.SetActive(true);
    }

    [ContextMenu("Toggle Individual Parts For Current Model")]
    public void ToggleIndividualPartsCurrentModel()
    {
        currentModel.ModelParts.ToggleIndividualParts();
        uiController.ToggleIcons(uiController.IndividualPartsToggle, currentModel.ModelParts.IndividualPartsEnabled);
    }


    [ContextMenu("Reset Current Model")]
    public void ResetModel()
    {
        currentModel.ModelParts.RestorePartTransforms();
    }

    public void OnExplosionSliderChange(float value)
    {
        currentModel.ModelParts.Explode(value);
    }

    public void ToggleRefinedPart(Model refinedPart, bool active)
    {
        Debug.Log($"Toggling {refinedPart.modelName} to {active}");
        // refinedPart.gameObject.SetActive(active);
        // (refinedPart.transform.position, refinedPart.transform.rotation) = GetPositionInFrontOfCamera(menuDistanceInFrontOfCamera);
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