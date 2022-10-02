using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectorMenu : MonoBehaviour
{
    public GameObject selector;
    public GameObject item;

    public GameObject textMeshObject;

    private string[] bodyParts = new string[4] { "Corpo", "Olhos", "Cabelo", "Boca" };
    [SerializeField]
    private int index = 0;
    private TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = textMeshObject.GetComponent<TextMeshProUGUI>();
        textMesh.text = bodyParts[index];
    }

    private void UpdateText()
    {
        textMesh.text = bodyParts[index];
    }

    public void Back()
    {
        index--;
        if(index<0)
        {
            index = bodyParts.Length-1;
        }
        UpdateText();
    }

    public void Next()
    {
        index++;
        if(index>bodyParts.Length-1)
        {
            index = 0;
        }
        UpdateText();
    }
}
