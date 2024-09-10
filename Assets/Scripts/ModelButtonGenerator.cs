using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // Ensure this is included if using TextMeshPro


// DEPRECATED: This script is no longer used in the final project
public class AircraftButtonGenerator : MonoBehaviour
{
    public GameObject buttonPrefab; // Drag the 'Model Name Button' Prefab here
    public Transform contentParent; // Drag the 'Content' GameObject here
    public ModelSwitcher modelSwitcher; // Drag the ModelSwitcher object here
    public ModelsController modelsController;
    public List<GameObject> modelList;

    // Path to the folder containing your models
    public string modelsFolderPath = "Assets/Prefabs/Models";

    void Start()
    {
        GenerateButtons();
    }

    void GenerateButtons()
    {
        Debug.Log("GenerateButtons is running");

        // Load model names from the specified folder
        string[] modelGuids = UnityEditor.AssetDatabase.FindAssets("t:Prefab", new[] { modelsFolderPath });
        List<string> modelNames = new List<string>();

        foreach (string guid in modelGuids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            modelNames.Add(prefab.name);
        }

        // Ensure buttonPrefab and contentParent are assigned
        if (buttonPrefab == null || contentParent == null)
        {
            Debug.LogError("Button prefab or content parent is not assigned.");
            return;
        }

        // Iterate through the model names and create buttons
        for (int i = 0; i < modelNames.Count; i++)
        {
            string modelName = modelNames[i];

            // Instantiate a new button
            GameObject newButton = Instantiate(buttonPrefab, contentParent);

            // Set the button's name to match the model name
            newButton.name = modelName;

            // Find the TextMeshProUGUI component within the button at the specific hierarchy path
            Transform textTransform = newButton.transform.Find("Text");
            TextMeshProUGUI buttonText = textTransform.GetComponent<TextMeshProUGUI>();

            // Set the button text to the model's name
            buttonText.text = modelName;

            // Add a listener to handle button clicks
            int index = i; // Capture index for use inside the lambda function
            newButton.GetComponent<Button>().onClick.AddListener(() => OnModelSelected(index));
        }

        // After all buttons are created, destroy the original "Model Name Button" if it exists
        foreach (Transform child in contentParent)
        {
            if (child.name == "Model Name Button")
            {
                Destroy(child.gameObject);
                Debug.Log("Destroyed 'Model Name Button'");
            }
        }


    }

    // Function to handle what happens when a button is clicked
    void OnModelSelected(int index)
    {
        // Call the ShowModel method from ModelSwitcher
        if (modelSwitcher != null)
        {
            modelSwitcher.ShowModel(index);
        }
        else
        {
            Debug.LogError("ModelSwitcher reference is not assigned.");
        }
    }



}