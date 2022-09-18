using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : MonoBehaviour
{
    public TextAsset avatarJSON;

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

    [System.Serializable]
    public class SpriteList
    {
        public Sprite[] sprite;
    }

    public BodyPartList bodyPartList = new BodyPartList();
    public SpriteList[] spriteLists;

    //ordem corpo, olho, boca, cabelo
    public GameObject[] parts;
    public int[] partId;

    void Start()
    {
        if (FileManager.FileExists("avatarData.txt"))
        {
            bodyPartList = JsonUtility.FromJson<BodyPartList>(FileManager.ReadFile("avatarData.txt"));
        }
        else
        {
            FileManager.WriteFile("avatarData.txt", JsonUtility.ToJson(bodyPartList));
        }
        AvatarUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        AvatarUpdate();
    }

    void AvatarUpdate()
    {
        foreach (BodyPart bodyPart in bodyPartList.bodyParts)
        {
            Image image = parts[bodyPart.bodyPartType].GetComponent<Image>();
            image.sprite = spriteLists[bodyPart.bodyPartType].sprite[bodyPart.bodyPartId];
            image.color = bodyPart.bodyPartColor;
        }
    }
}
