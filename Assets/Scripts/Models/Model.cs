using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

#nullable enable

public class Model : MonoBehaviour
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
        if (string.IsNullOrEmpty(modelName)) modelName = name;

        RefinedPartController? refinedPartController = GetComponentInChildren<RefinedPartController>();
        if (refinedPartController != null) refinedParts = refinedPartController.RefinedParts;
    }
}