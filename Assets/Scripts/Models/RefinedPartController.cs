using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Manages the refined parts of a model, ensuring they are properly initialized and managed.
/// </summary>
public class RefinedPartController : NetworkBehaviour
{
    private List<Model> refinedParts = new List<Model>();
    public List<Model> RefinedParts { get => refinedParts; }

    void Awake()
    {
        // Add each child transform's Model component to the refined parts list
        foreach (Transform child in transform)
            refinedParts.Add(child.GetComponent<Model>());

        foreach (Model refinedPart in refinedParts)
        {
            // Set the parent of the refined part to null so it does not follow the parent's transform
            refinedPart.transform.SetParent(null);
            refinedPart.gameObject.SetActive(false);
        }
    }
}
