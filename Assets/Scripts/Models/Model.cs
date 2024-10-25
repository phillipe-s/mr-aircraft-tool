using System.Collections.Generic;
using Fusion;
using Oculus.Interaction;
using UnityEngine;

#nullable enable

/// <summary>
/// Represents a model in the application, which can have individual parts and refined parts.
/// </summary>
public class Model : NetworkBehaviour
{
    [SerializeField, Optional] private string modelName;
    public string ModelName { get => modelName; }
    [SerializeField] private IndividualParts modelParts;
    public IndividualParts ModelParts { get => modelParts; }
    private List<Model>? refinedParts = new List<Model>();
    public List<Model>? RefinedParts { get => refinedParts; }
    [SerializeField, Optional] private Model parentModel;
    public Model ParentModel { get => parentModel; }

    void Awake()
    {
        // If the model name is not set, use the GameObject's name
        if (string.IsNullOrEmpty(modelName)) modelName = name;

        // Get the refined parts from the RefinedPartController if it exists
        RefinedPartController? refinedPartController = GetComponentInChildren<RefinedPartController>();
        if (refinedPartController != null) refinedParts = refinedPartController.RefinedParts;
    }
}