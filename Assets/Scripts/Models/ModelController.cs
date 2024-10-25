using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Manages the models in the application, including switching between models and toggling their parts.
/// </summary>
public class ModelController : NetworkBehaviour
{
    public List<Model> models;
    private Model currentModel;
    public Model CurrentModel { get => currentModel; }
    [SerializeField] UIController uiController;

    void Awake()
    {
        foreach (Model model in models)
            model.gameObject.SetActive(false);

        currentModel = models[0];
    }

    public void SwitchToModel(Model model) { RPC_SwitchToModel(model); }

    /// <summary>
    /// RPC method to switch to the specified model.
    /// </summary>
    /// <param name="model">The model to switch to.</param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SwitchToModel(Model model)
    {
        currentModel.gameObject.SetActive(false);

        // Set the location of the new model to the location of the current model
        model.transform.position = currentModel.transform.position;

        currentModel = model;
        currentModel.gameObject.SetActive(true);

        uiController.ToggleIcons(uiController.IndividualPartsToggle, currentModel.ModelParts.IndividualPartsEnabled);
        uiController.ToggleIcons(uiController.RayInteractorToggle, currentModel.ModelParts.RayGrabInteractionsEnabled);

        // If the current model has no refined parts, disable the refined parts toggle
        if (currentModel.RefinedParts.Count == 0) uiController.SetRefinedPartsToggleActive(false);
        else uiController.SetRefinedPartsToggleActive(true);

        Debug.Log($"Switched to model: {currentModel.ModelName}");
    }

    /// <summary>
    /// Toggles the individual parts for the current model.
    /// </summary>
    [ContextMenu("Toggle Individual Parts For Current Model")]
    public void ToggleIndividualPartsCurrentModel()
    {
        currentModel.ModelParts.ToggleIndividualParts();
        uiController.ToggleIcons(uiController.IndividualPartsToggle, currentModel.ModelParts.IndividualPartsEnabled);

        Debug.Log($"Individual Parts Enabled: {currentModel.ModelParts.IndividualPartsEnabled}");
    }

    /// <summary>
    /// Toggles the ray interactor for the current model.
    /// </summary>
    [ContextMenu("Toggle Ray Interactor For Current Model")]
    public void ToggleRayInteractor()
    {
        currentModel.ModelParts.ToggleRayGrabInteractions();
        uiController.ToggleIcons(uiController.RayInteractorToggle, currentModel.ModelParts.RayGrabInteractionsEnabled);

        Debug.Log($"Ray Interactor Enabled: {currentModel.ModelParts.RayGrabInteractionsEnabled}");
    }

    /// <summary>
    /// Resets the current model to its original state.
    /// </summary
    [ContextMenu("Reset Current Model")]
    public void ResetModel()
    {
        currentModel.ModelParts.RestorePartTransforms();
        Debug.Log($"Reset model: {currentModel.ModelName}");
    }

    /// <summary>
    /// Handles changes to the explosion slider value.
    /// </summary>
    /// <param name="value">The new value of the explosion slider.</param>
    public void OnExplosionSliderChange(float value)
    {
        currentModel.ModelParts.Explode(value);
    }

    /// <summary>
    /// Toggles the visibility of a refined part.
    /// </summary>
    /// <param name="refinedPart">The refined part to toggle.</param>
    /// <param name="active">Whether the refined part should be active.</param>
    public void ToggleRefinedPart(Model refinedPart, bool active)
    {
        if (active) SwitchToModel(refinedPart);
        else SwitchToModel(refinedPart.ParentModel);
        Debug.Log($"Toggled refined part: {refinedPart.ModelName}");
    }
}