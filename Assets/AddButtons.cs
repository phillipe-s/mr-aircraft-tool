using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddButtons : MonoBehaviour
{
    public ModelController modelController;
    public GameObject buttonPrefab;

    void Start()
    {
        if (modelController.CurrentModel.RefinedParts == null) return;

        foreach (Model model in modelController.CurrentModel.RefinedParts)
        {
            GameObject button = Instantiate(buttonPrefab, GameObject.Find("Content").transform);
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = model.modelName;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
