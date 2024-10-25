using System.Collections;
using System.Collections.Generic;
using Fusion;
using Oculus.Interaction.Input;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the UI elements and interactions in the application.
/// </summary>
public class UIController : MonoBehaviour
{
    [SerializeField] private ModelController modelController;
    [SerializeField] private GameObject centerEyeAnchor;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private AudioSource showMenuSound;
    [SerializeField] private AudioSource hideMenuSound;
    [SerializeField] private GameObject modelMenu;
    [SerializeField] private GameObject scrollMenuPrefab;
    [SerializeField] private GameObject scrollMenuButtonPrefab;
    [SerializeField] private GameObject individualPartsToggle;
    public GameObject IndividualPartsToggle { get => individualPartsToggle; }
    [SerializeField] private GameObject rayInteractorToggle;
    public GameObject RayInteractorToggle { get => rayInteractorToggle; }
    private bool refinedPartsMenuActive = false;
    public bool RefinedPartsMenuActive { get => refinedPartsMenuActive; }
    public GameObject refinedPartsMenuToggle;
    [SerializeField] private GameObject explosionSlider;
    private List<Toggle> refinedPartToggles = new List<Toggle>();
    private float menuDistanceInFrontOfCamera = 0.5f;

    void Start()
    {
        mainMenu.SetActive(false);

        // Add listeners to the buttons in the model menu
        explosionSlider.GetComponentInChildren<Slider>().onValueChanged.AddListener(modelController.OnExplosionSliderChange);

        ToggleIcons(individualPartsToggle, false);
        ToggleIcons(refinedPartsMenuToggle, false);
        ToggleIcons(rayInteractorToggle, false);
    }

    void Update()
    {
        // Toggle the main menu when the start button is pressed
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            ToggleMenu();
        }
    }

    /// <summary>
    /// Toggles the visibility of the main menu.
    /// </summary>
    [ContextMenu("Toggle Model Menu")]
    public void ToggleMenu()
    {
        (mainMenu.transform.position, mainMenu.transform.rotation) = GetPositionInFrontOfCamera();
        mainMenu.SetActive(!mainMenu.activeSelf);

        if (mainMenu.activeSelf) showMenuSound.Play();
        else hideMenuSound.Play();
    }

    /// <summary>
    /// Toggles the visibility of the refined parts menu.
    /// </summary>
    public void ToggleRefinedPartsMenu()
    {
        Transform content = modelMenu.transform.Find("Unity Canvas/LeftSide/Scroll View/Viewport/Content");
        Model currentModel;

        // If the current model has a parent model, use the parent model
        if (modelController.CurrentModel.ParentModel != null) currentModel = modelController.CurrentModel.ParentModel;
        else currentModel = modelController.CurrentModel;

        if (!refinedPartsMenuActive)
        {
            // Populate the refined parts menu with buttons
            foreach (Model refinedPart in currentModel.RefinedParts)
            {
                foreach (Transform modelButton in content)
                    modelButton.gameObject.SetActive(false);

                content.GetComponent<ToggleGroup>().allowSwitchOff = true;

                // Create a button for each refined part 
                GameObject button = Instantiate(scrollMenuButtonPrefab, content);
                button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = refinedPart.ModelName;

                // Add a listener to the button
                Toggle toggle = button.GetComponent<Toggle>();
                toggle.group = content.GetComponent<ToggleGroup>();
                toggle.onValueChanged.AddListener((value) =>
                {
                    modelController.ToggleRefinedPart(refinedPart, value);
                });
                refinedPartToggles.Add(toggle);
            }

            ToggleIcons(refinedPartsMenuToggle, true);
            refinedPartsMenuActive = true;
        }
        else
        {
            // Remove all listeners and destroy the toggle buttons
            foreach (Toggle toggle in refinedPartToggles)
            {
                toggle.onValueChanged.RemoveAllListeners();
                Destroy(toggle.gameObject);
            }

            refinedPartToggles.Clear();
            refinedPartsMenuActive = false;

            ToggleIcons(refinedPartsMenuToggle, false);
            content.GetComponent<ToggleGroup>().allowSwitchOff = false;

            foreach (Transform modelButton in content)
                modelButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Sets the active state of the refined parts toggle.
    /// </summary>
    /// <param name="active">Whether the toggle should be active.</param>
    public void SetRefinedPartsToggleActive(bool active)
    {
        refinedPartsMenuToggle.gameObject.SetActive(active);
    }

    /// <summary>
    /// Toggles the icons on a given GameObject.
    /// </summary>
    /// <param name="toggle">The GameObject containing the icons.</param>
    /// <param name="active">Whether the icons should be active.</param>
    public void ToggleIcons(GameObject toggle, bool active)
    {
        toggle.transform.Find("IconOn").gameObject.SetActive(active);
        toggle.transform.Find("IconOff").gameObject.SetActive(!active);
    }

    /// <summary>
    /// Gets the position and rotation in front of the camera.
    /// </summary>
    /// <returns>A tuple containing the position and rotation.</returns>
    private (Vector3, Quaternion) GetPositionInFrontOfCamera()
    {
        Vector3 cameraForward = centerEyeAnchor.transform.forward;
        Vector3 cameraPosition = centerEyeAnchor.transform.position;
        Quaternion cameraRotation = centerEyeAnchor.transform.rotation;
        return (cameraPosition + cameraForward * menuDistanceInFrontOfCamera, cameraRotation);
    }

}
