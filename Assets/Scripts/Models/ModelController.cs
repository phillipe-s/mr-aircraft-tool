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
    [SerializeField] private GameObject centerEyeAnchor;
    [SerializeField] private GameObject scrollMenuPrefab;
    [SerializeField] private GameObject scrollMenuButtonPrefab;
    [SerializeField] private GameObject modelMenu;
    [SerializeField] private GameObject modelMenuToggle;
    [SerializeField] private GameObject individualPartsToggle;
    [SerializeField] private GameObject sliderPrefab;
    private GameObject explosionSlider;
    [SerializeField] private GameObject explosionSliderToggle;
    private GameObject refinedPartsMenu;
    public GameObject refinedPartsMenuToggle;
    private List<Toggle> refinedPartToggles = new List<Toggle>();
    private float menuDistanceInFrontOfCamera = 0.3f;

    void Awake()
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
        ToggleIcons(refinedPartsMenuToggle, false);
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
        (modelMenu.transform.position, modelMenu.transform.rotation) = GetPositionInFrontOfCamera(menuDistanceInFrontOfCamera);
        modelMenu.SetActive(!modelMenu.activeSelf);
        ToggleIcons(modelMenuToggle, modelMenu.activeSelf);
    }

    [ContextMenu("Toggle Refined Parts Menu For Current Model")]
    public void ToggleRefinedPartsMenu()
    {
        if (currentModel.RefinedParts == null) return;

        if (refinedPartsMenu == null)
        {
            refinedPartsMenu = Instantiate(scrollMenuPrefab);
            (refinedPartsMenu.transform.position, refinedPartsMenu.transform.rotation) = GetPositionInFrontOfCamera(menuDistanceInFrontOfCamera);
            refinedPartsMenu.SetActive(false);

            // Populate the refined parts menu with buttons
            foreach (Model refinedPart in currentModel.RefinedParts)
            {
                Transform content = refinedPartsMenu.transform.Find("Unity Canvas/LeftSide/Scroll View/Viewport/Content");
                content.GetComponent<ToggleGroup>().allowSwitchOff = true;

                GameObject button = Instantiate(scrollMenuButtonPrefab, content);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = refinedPart.modelName;
                Toggle toggle = button.GetComponent<Toggle>();
                toggle.group = content.GetComponent<ToggleGroup>();
                toggle.onValueChanged.AddListener((value) =>
                {
                    ToggleRefinedPart(refinedPart, value);
                });
                refinedPartToggles.Add(toggle);
            }
            refinedPartsMenu.SetActive(true);
            ToggleIcons(refinedPartsMenuToggle, true);
        }
        else
        {
            foreach (Toggle toggle in refinedPartToggles)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
            refinedPartToggles.Clear();

            Destroy(refinedPartsMenu);
            refinedPartsMenu = null;
            ToggleIcons(refinedPartsMenuToggle, false);
        }
    }

    public void ToggleExplosionSlider()
    {
        (explosionSlider.transform.position, explosionSlider.transform.rotation) = GetPositionInFrontOfCamera(menuDistanceInFrontOfCamera);
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

    private (Vector3, Quaternion) GetPositionInFrontOfCamera(float distanceInFront)
    {
        // Set the position of the modelMenu in front of the user's camera
        Vector3 cameraForward = centerEyeAnchor.transform.forward;
        Vector3 cameraPosition = centerEyeAnchor.transform.position;
        Quaternion cameraRotation = centerEyeAnchor.transform.rotation;
        return (cameraPosition + cameraForward * distanceInFront, cameraRotation);
    }

    private void ToggleRefinedPart(Model refinedPart, bool active)
    {
        Debug.Log($"Toggling {refinedPart.modelName} to {active}");
        refinedPart.gameObject.SetActive(active);
        (refinedPart.transform.position, refinedPart.transform.rotation) = GetPositionInFrontOfCamera(menuDistanceInFrontOfCamera);
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