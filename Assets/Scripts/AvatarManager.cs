using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

public class PlayerData
{
    public string username;
    public BodyPartList bodyPartList;
    public ushort id;// { get; private set; }
    public int score;
    public ushort palavraAtual;// { get; private set; }
    public ushort qtdTentativas;// { get; private set; }
    public Boolean active;
}

[System.Serializable]
public class BodyPart
{
    public int bodyPartType; //id do tipo da parte
    public int bodyPartId; //id do sprite
    public MockColor bodyPartColor; //cor da parte
}
[System.Serializable]
public class MockColor
{
    public float r, g, b, a;
}

[System.Serializable]
public class BodyPartList
{
    public BodyPart[] bodyParts;
}

public class AvatarManager : MonoBehaviour
{
    public Boolean isPlayerAvatar;
    public TextAsset defaultAvatar;

    public PlayerData playerData = new PlayerData();
    //public BodyPartList bodyPartList = new BodyPartList();

    //ordem corpo, olho, boca, cabelo
    public GameObject[] parts;

    void Start()
    {
        if (isPlayerAvatar)
        {
            if (FileManager.FileExists("avatarData.txt"))
            {
                playerData.bodyPartList = JsonUtility.FromJson<BodyPartList>(FileManager.ReadFile("avatarData.txt"));
                playerData.username = "FileExists";
            }
            else
            {
                playerData.bodyPartList = JsonUtility.FromJson<BodyPartList>(defaultAvatar.text);
                playerData.username = "Usrnm";
            }
            AvatarUpdate();
        }
    }

    void SavePlayerData()
    {
        FileManager.WriteFile("avatarData.txt", JsonUtility.ToJson(playerData.bodyPartList));
    }
    public PlayerData GetPlayerData()
    {
        if(playerData.username == null)
        {
            if (FileManager.FileExists("avatarData.txt"))
            {
                playerData.bodyPartList = JsonUtility.FromJson<BodyPartList>(FileManager.ReadFile("avatarData.txt"));
                playerData.username = "FileExists";
            }
            else
            {
                playerData.bodyPartList = JsonUtility.FromJson<BodyPartList>(defaultAvatar.text);
                playerData.username = "Usrnm";
            }
        }
        return playerData;
    }
    public void SetPlayerData(PlayerData playerData)
    {
        if (playerData != null)
        {
            this.playerData = playerData;
            AvatarUpdate();
        }
    }

    public void AvatarUpdate()
    {
        if(parts.Length > 0)
        {
            foreach (BodyPart bodyPart in playerData.bodyPartList.bodyParts)
            {
                Image image = parts[bodyPart.bodyPartType].GetComponent<Image>();
                Sprite sprite = Resources.Load<Sprite>("Avatar/" + bodyPart.bodyPartType + "/" + bodyPart.bodyPartId);
                Debug.Log(sprite);
                image.sprite = sprite;
                Debug.Log("Avatar/" + bodyPart.bodyPartType + "/" + bodyPart.bodyPartId);
                image.color = new Color(bodyPart.bodyPartColor.r, bodyPart.bodyPartColor.g, bodyPart.bodyPartColor.b, bodyPart.bodyPartColor.a);
            }
        }
    }

    public void ClearAvatar()
    {
        parts[bodyPart.bodyPartType].GetComponent<Image>().sprite = null;
    }
}
