using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ItemSelector : MonoBehaviour
{

    public AvatarManager avatarManager;
    public int bodyPartId;
    public int partId;

    public Image image;

    private void Start()
    {
        Sprite sprite = Resources.Load<Sprite>("Avatar/" + bodyPartId + "/" + partId);
        image.sprite = sprite;
    }
}
