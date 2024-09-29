using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefinedPartController : MonoBehaviour
{
    private List<Model> refinedParts = new List<Model>();
    public List<Model> RefinedParts { get => refinedParts; }

    void Start()
    {
        foreach (Transform child in transform)
        {
            refinedParts.Add(child.GetComponent<Model>());
        }

        foreach (Model refinedPart in refinedParts)
        {
            // Set the parent of the refined part to null so it does not follow the parent's transform
            refinedPart.transform.SetParent(null);
            refinedPart.gameObject.SetActive(false);
        }
    }
}
