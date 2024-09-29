using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefinedPartController : MonoBehaviour
{
    private List<Model> refinedParts;
    public List<Model> RefinedParts { get => refinedParts; }

    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            refinedParts.Add(child.GetComponent<Model>());

            // Set the parent of the refined part to null so it does not follow the parent's transform
            child.SetParent(null);
        }
    }
}
