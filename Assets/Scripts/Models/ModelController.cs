using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class ModelsController : MonoBehaviour
{
    public List<Model> models;
    private Model currentModel;
    public Model CurrentModel { get => currentModel; }
    [SerializeField] private GameObject centerEyeAnchor;
    [SerializeField] private GameObject modelMenu;
    [SerializeField] private GameObject modelMenuEnabledIcon;
    [SerializeField] private GameObject modelMenuDisabledIcon;
    [SerializeField] private GameObject individualPartsEnabledIcon;
    [SerializeField] private GameObject individualPartsDisabledIcon;

    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        currentModel = models[0];
        currentModel.gameObject.SetActive(true);

        modelMenu.SetActive(false);
        modelMenuEnabledIcon.SetActive(modelMenu.activeSelf);
        modelMenuDisabledIcon.SetActive(!modelMenu.activeSelf);
        ToggleIndividualPartsIcons();

    }

    public void SwitchToModel(Model model)
    {
        currentModel.gameObject.SetActive(false);
        currentModel = model;
        currentModel.gameObject.SetActive(true);
        ToggleIndividualPartsIcons();
    }

    [ContextMenu("Toggle Individual Parts For Current Model")]
    public void ToggleIndividualPartsCurrentModel()
    {
        currentModel.ModelParts.ToggleIndividualParts();
        ToggleIndividualPartsIcons();
    }


    public void ToggleModelMenu()
    {
        // Set the position of the modelMenu in front of the user's camera
        float distanceInFront = 0.3f; // Distance in front of the user
        Vector3 cameraForward = centerEyeAnchor.transform.forward;
        Vector3 cameraPosition = centerEyeAnchor.transform.position;
        modelMenu.transform.position = cameraPosition + cameraForward * distanceInFront;

        modelMenu.SetActive(!modelMenu.activeSelf);
        modelMenuEnabledIcon.SetActive(modelMenu.activeSelf);
        modelMenuDisabledIcon.SetActive(!modelMenu.activeSelf);
    }

    private void ToggleIndividualPartsIcons()
    {
        individualPartsEnabledIcon.SetActive(currentModel.ModelParts.IndividualPartsEnabled);
        individualPartsDisabledIcon.SetActive(!currentModel.ModelParts.IndividualPartsEnabled);
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