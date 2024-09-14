using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddButtons : MonoBehaviour
{
    public ModelController modelController;
    private Model currentModel;
    private List<Model> refinedParts;
    private GameObject Content;

    void Start()
    {
        currentModel = modelController.CurrentModel;
        refinedParts = currentModel.RefinedParts;

        Content = GameObject.Find("Content");
        Debug.Log($"Content: {Content}");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
