using System.Collections;
using System.Collections.Generic;
using Fusion;
using Oculus.Interaction.Input;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private ModelController modelController;
    [SerializeField] private GameObject centerEyeAnchor;
    [SerializeField] private GameObject scrollMenuPrefab;
    [SerializeField] private GameObject scrollMenuButtonPrefab;
    [SerializeField] private GameObject modelMenu;
    [SerializeField] private GameObject modelMenuToggle;
    [SerializeField] private GameObject individualPartsToggle;
    public GameObject IndividualPartsToggle { get => individualPartsToggle; }
    [SerializeField] private GameObject sliderPrefab;
    private GameObject explosionSlider;
    [SerializeField] private GameObject explosionSliderToggle;
    private GameObject refinedPartsMenu;
    public GameObject refinedPartsMenuToggle;
    private List<Toggle> refinedPartToggles = new List<Toggle>();
    private float menuDistanceInFrontOfCamera = 0.3f;

    void Start()
    {
        modelMenu.SetActive(false);

        explosionSlider = Instantiate(sliderPrefab);
        explosionSlider.SetActive(false);
        explosionSlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(modelController.OnExplosionSliderChange);

        ToggleIcons(modelMenuToggle, false);
        ToggleIcons(individualPartsToggle, false);
        ToggleIcons(explosionSliderToggle, false);
        ToggleIcons(refinedPartsMenuToggle, false);
    }

    [ContextMenu("Toggle Model Menu")]
    public void ToggleModelMenu()
    {
        (modelMenu.transform.position, modelMenu.transform.rotation) = GetPositionInFrontOfCamera();
        modelMenu.SetActive(!modelMenu.activeSelf);
        ToggleIcons(modelMenuToggle, modelMenu.activeSelf);
    }

    [ContextMenu("Toggle Explosion Slider")]
    public void ToggleExplosionSlider()
    {
        (explosionSlider.transform.position, explosionSlider.transform.rotation) = GetPositionInFrontOfCamera();
        explosionSlider.SetActive(!explosionSlider.activeSelf);
        ToggleIcons(explosionSliderToggle, explosionSlider.activeSelf);
    }

    [ContextMenu("Toggle Refined Parts Menu For Current Model")]
    public void ToggleRefinedPartsMenu()
    {
        if (modelController.CurrentModel.RefinedParts == null)
        {
            Debug.LogWarning("Current model does not have refined parts");
            return;
        }

        if (refinedPartsMenu == null)
        {
            refinedPartsMenu = Instantiate(scrollMenuPrefab);
            (refinedPartsMenu.transform.position, refinedPartsMenu.transform.rotation) = GetPositionInFrontOfCamera();
            refinedPartsMenu.SetActive(false);

            Model currentModel;
            if (modelController.CurrentModel.ParentModel != null) currentModel = modelController.CurrentModel.ParentModel;
            else currentModel = modelController.CurrentModel;


            // Populate the refined parts menu with buttons
            foreach (Model refinedPart in currentModel.RefinedParts)
            {
                Transform content = refinedPartsMenu.transform.Find("Unity Canvas/LeftSide/Scroll View/Viewport/Content");
                content.GetComponent<ToggleGroup>().allowSwitchOff = true;

                GameObject button = Instantiate(scrollMenuButtonPrefab, content);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = refinedPart.ModelName;
                Toggle toggle = button.GetComponent<Toggle>();
                toggle.group = content.GetComponent<ToggleGroup>();
                toggle.onValueChanged.AddListener((value) =>
                {
                    modelController.ToggleRefinedPart(refinedPart, value);
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

    public void ToggleIcons(GameObject toggle, bool active)
    {
        toggle.transform.Find("IconOn").gameObject.SetActive(active);
        toggle.transform.Find("IconOff").gameObject.SetActive(!active);
    }

    private (Vector3, Quaternion) GetPositionInFrontOfCamera()
    {
        Vector3 cameraForward = centerEyeAnchor.transform.forward;
        Vector3 cameraPosition = centerEyeAnchor.transform.position;
        Quaternion cameraRotation = centerEyeAnchor.transform.rotation;
        return (cameraPosition + cameraForward * menuDistanceInFrontOfCamera, cameraRotation);
    }

}
