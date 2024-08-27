using UnityEngine;
using System.Collections.Generic;

public class ModelSwitcher : MonoBehaviour
{
    // List to hold your models
    public List<GameObject> models;

    // Index of the currently displayed model
    private int currentModelIndex = 0;

    void Start()
    {
        // Initialize by displaying the first model
        ShowModel(currentModelIndex);
    }

    // Method to show a specific model by index
    public void ShowModel(int index)
    {
        // Ensure the index is within bounds
        if (index < 0 || index >= models.Count)
        {
            Debug.LogError("Index out of bounds");
            return;
        }

        // Disable all models
        foreach (GameObject model in models)
        {
            model.SetActive(false);
        }

        // Enable the selected model
        models[index].SetActive(true);
        currentModelIndex = index;
    }

    // Method to switch to the next model
    public void NextModel()
    {
        int nextIndex = (currentModelIndex + 1) % models.Count;
        ShowModel(nextIndex);
    }

    // Method to switch to the previous model
    public void PreviousModel()
    {
        int previousIndex = (currentModelIndex - 1 + models.Count) % models.Count;
        ShowModel(previousIndex);
    }
}