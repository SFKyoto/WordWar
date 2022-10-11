using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class AvatarManager : MonoBehaviour
{
    public TextAsset defaultAvatar;

    [System.Serializable]
    public class BodyPart
    {
        public int bodyPartType; //id do tipo da parte
        public int bodyPartId; //id do sprite
        public Color bodyPartColor; //cor da parte
    }

    [System.Serializable]
    public class BodyPartList
    {
        public BodyPart[] bodyParts;
    }

    public BodyPartList bodyPartList = new BodyPartList();

    //ordem corpo, olho, boca, cabelo
    public GameObject[] parts;

    void Start()
    {
        if (FileManager.FileExists("avatarData.txt"))
        {
            bodyPartList = JsonUtility.FromJson<BodyPartList>(FileManager.ReadFile("avatarData.txt"));
        }
        else
        {
            bodyPartList = JsonUtility.FromJson<BodyPartList>(defaultAvatar.text);
        }
        AvatarUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        // AvatarUpdate();
    }

    void AvatarUpdate()
    {
        foreach (BodyPart bodyPart in bodyPartList.bodyParts)
        {
            Image image = parts[bodyPart.bodyPartType].GetComponent<Image>();
            Sprite sprite = Resources.Load<Sprite>("Avatar/" + bodyPart.bodyPartType + "/" + bodyPart.bodyPartId);
            Debug.Log(sprite);
            image.sprite = sprite;
            Debug.Log("Avatar/" + bodyPart.bodyPartType + "/" + bodyPart.bodyPartId);
            //image.sprite = spriteLists[bodyPart.bodyPartType].sprite[bodyPart.bodyPartId];
            image.color = bodyPart.bodyPartColor;
        }
    }
}
