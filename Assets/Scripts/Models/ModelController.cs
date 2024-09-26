using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class ModelsController : MonoBehaviour
{
    public List<Model> models;
    private Model currentModel;
    public Model CurrentModel { get => currentModel; }
    [SerializeField] private GameObject centerEyeAnchor;
    [SerializeField] private GameObject modelMenu;
    [SerializeField] private GameObject modelMenuToggle;
    [SerializeField] private GameObject individualPartsToggle;
    [SerializeField] private GameObject sliderPrefab;
    private GameObject explosionSlider;
    [SerializeField] private GameObject explosionSliderToggle;
    private float menuDistanceInFrontOfCamera = 0.3f;

    void Start()
    {
        foreach (Model model in models)
        {
            model.gameObject.SetActive(false);
        }

        currentModel = models[0];
        currentModel.gameObject.SetActive(true);
        modelMenu.SetActive(false);

        explosionSlider = Instantiate(sliderPrefab);
        explosionSlider.SetActive(false);
        explosionSlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(OnExplosionSliderChange);

        ToggleIcons(modelMenuToggle, false);
        ToggleIcons(individualPartsToggle, false);
        ToggleIcons(explosionSliderToggle, false);
    }

    public void SwitchToModel(Model model)
    {
        currentModel.gameObject.SetActive(false);
        currentModel = model;
        currentModel.gameObject.SetActive(true);
        ToggleIcons(individualPartsToggle, currentModel.ModelParts.IndividualPartsEnabled);
    }

    [ContextMenu("Toggle Individual Parts For Current Model")]
    public void ToggleIndividualPartsCurrentModel()
    {
        currentModel.ModelParts.ToggleIndividualParts();
        ToggleIcons(individualPartsToggle, currentModel.ModelParts.IndividualPartsEnabled);
    }

    public void ToggleModelMenu()
    {
        modelMenu.transform.position = GetPositionInFrontOfCamera(menuDistanceInFrontOfCamera);
        modelMenu.SetActive(!modelMenu.activeSelf);
        ToggleIcons(modelMenuToggle, modelMenu.activeSelf);
    }

    public void ToggleExplosionSlider()
    {
        explosionSlider.transform.position = GetPositionInFrontOfCamera(menuDistanceInFrontOfCamera);
        explosionSlider.SetActive(!explosionSlider.activeSelf);
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

    private void ToggleIcons(GameObject toggle, bool active)
    {
        toggle.transform.Find("IconOn").gameObject.SetActive(active);
        toggle.transform.Find("IconOff").gameObject.SetActive(!active);
    }

    private Vector3 GetPositionInFrontOfCamera(float distanceInFront)
    {
        // Set the position of the modelMenu in front of the user's camera
        Vector3 cameraForward = centerEyeAnchor.transform.forward;
        Vector3 cameraPosition = centerEyeAnchor.transform.position;
        return cameraPosition + cameraForward * distanceInFront;
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