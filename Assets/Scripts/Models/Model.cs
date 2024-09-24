using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

#nullable enable

public class Model : MonoBehaviour
{
    [SerializeField] private IndividualParts modelParts;
    public IndividualParts ModelParts { get => modelParts; }

    private List<Model>? refinedParts;
    public List<Model>? RefinedParts { get => refinedParts; }

    void Start()
    {
        RefinedPartController? refinedPartController = GetComponentInChildren<RefinedPartController>();
        if (refinedPartController != null) refinedParts = refinedPartController.RefinedParts;
        else Debug.LogWarning($"{name} does not have refined parts");
    }
}
