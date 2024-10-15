using System.Collections;
using System.Collections.Generic;
using Fusion;
using Oculus.Interaction.Input;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class ModelController : NetworkBehaviour
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

    public void SwitchToModel(Model model) { RPC_SwitchToModel(model); }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SwitchToModel(Model model)
    {
        currentModel.gameObject.SetActive(false);

        // Set the location of the new model to the location of the current model
        model.transform.position = currentModel.transform.position;

        currentModel = model;
        currentModel.gameObject.SetActive(true);

        uiController.ToggleIcons(uiController.IndividualPartsToggle, currentModel.ModelParts.IndividualPartsEnabled);

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
        if (active) SwitchToModel(refinedPart);
        else SwitchToModel(refinedPart.ParentModel);
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