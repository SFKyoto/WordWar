using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using TMPro;
using UnityEngine.SceneManagement;

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
    public TMP_InputField TMPIPTUsername;

    public static readonly string[][] defaultAvatarParts =
    {
        new string[] { "2/1", "#1F1D1C" },
        new string[] { "2/0", "#1F1D1C" },
        new string[] { "2/2", "#1F1D1C" },
        new string[] { "2/1", "#B2592D" },
        new string[] { "2/0", "#B2592D" },
        new string[] { "2/2", "#B2592D" },
        new string[] { "2/1", "#DFE20A" },
        new string[] { "2/0", "#DFE20A" },
        new string[] { "2/2", "#DFE20A" },
        new string[] { "2/1", "#FF2314" },
        new string[] { "2/0", "#FF2314" },
        new string[] { "2/2", "#FF2314" },
        new string[] { "2/1", "#2FFF48" },
        new string[] { "2/0", "#2FFF48" },
        new string[] { "2/2", "#2FFF48" },
        new string[] { "2/1", "#2FC9FF" },
        new string[] { "2/0", "#2FC9FF" },
        new string[] { "2/2", "#2FC9FF" },
        new string[] { "0/0", "#DDD974" },
        new string[] { "0/0", "#CC865A" },
        new string[] { "0/0", "#663414" },
        new string[] { "1/0", "#000000" },
        new string[] { "1/1", "#000000" },
        new string[] { "1/2", "#000000" },
        new string[] { "3/0", "#000000" },
        new string[] { "3/1", "#000000" },
        new string[] { "3/2", "#000000" },
    };

    public PlayerData playerData = new PlayerData();
    //public BodyPartList bodyPartList = new BodyPartList();

    //ordem corpo, olho, boca, cabelo
    public GameObject[] parts;
    public Image IMGActiveCross;
    public Image IMGBLock;

    void Start()
    {
        if (isPlayerAvatar)
        {
            if (FileManager.FileExists("avatarData.txt"))
            {
                PlayerData tempData = JsonUtility.FromJson<PlayerData>(FileManager.ReadFile("avatarData.txt"));
                playerData.username = tempData.username == null ? "FileExists" : tempData.username;
                playerData.bodyPartList = tempData.bodyPartList;
            }
            else
            {
                playerData.bodyPartList = JsonUtility.FromJson<BodyPartList>(defaultAvatar.text);
                playerData.username = "Jogador";
            }
            AvatarUpdate();
            try { TMPIPTUsername.text = playerData.username; }
            catch { }
        }
    }

    public void SavePlayerData()
    {
        PlayerData tempData = new PlayerData();
        tempData.username = TMPIPTUsername.text != "" ? TMPIPTUsername.text : playerData.username;
        tempData.bodyPartList = playerData.bodyPartList;
        FileManager.WriteFile("avatarData.txt", JsonUtility.ToJson(tempData));
        SceneManager.LoadScene("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
    }

    public PlayerData GetPlayerData()
    {
        if(playerData.username == null)
        {
            if (FileManager.FileExists("avatarData.txt"))
            {
                PlayerData tempData = JsonUtility.FromJson<PlayerData>(FileManager.ReadFile("avatarData.txt"));
                playerData.username = tempData.username == null ? "FileExists" : tempData.username;
                playerData.bodyPartList = tempData.bodyPartList;
            }
            else
            {
                playerData.bodyPartList = JsonUtility.FromJson<BodyPartList>(defaultAvatar.text);
                playerData.username = "Jogador";
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
                parts[bodyPart.bodyPartType].SetActive(true);
                Image image = parts[bodyPart.bodyPartType].GetComponent<Image>();
                Sprite sprite = Resources.Load<Sprite>("Avatar/" + bodyPart.bodyPartType + "/" + bodyPart.bodyPartId);
                //Debug.Log(sprite);
                image.sprite = sprite;
                //Debug.Log("Avatar/" + bodyPart.bodyPartType + "/" + bodyPart.bodyPartId);
                image.color = new Color(bodyPart.bodyPartColor.r, bodyPart.bodyPartColor.g, bodyPart.bodyPartColor.b, bodyPart.bodyPartColor.a);
            }
        }
        //IMGActiveCross?.gameObject.SetActive(!playerData.active);
        IMGBLock?.gameObject.SetActive(!playerData.active);
    }

    public void SetBodyPart(int bodyPartArray)
    {
        int bodyPartWantedType = Int32.Parse(defaultAvatarParts[bodyPartArray][0][0] + "");
        int bodyPartWantedNumber = Int32.Parse(defaultAvatarParts[bodyPartArray][0][2] + "");
        playerData.bodyPartList.bodyParts[bodyPartWantedType].bodyPartId = bodyPartWantedNumber;
        UnityEngine.Color tempColor;
        ColorUtility.TryParseHtmlString(defaultAvatarParts[bodyPartArray][1], out tempColor);
        playerData.bodyPartList.bodyParts[bodyPartWantedType].bodyPartColor.r = tempColor.r;
        playerData.bodyPartList.bodyParts[bodyPartWantedType].bodyPartColor.g = tempColor.g;
        playerData.bodyPartList.bodyParts[bodyPartWantedType].bodyPartColor.b = tempColor.b;
        AvatarUpdate();
        Debug.Log(bodyPartArray);
    }

    public void ClearAvatar()
    {
        //Debug.Log("clear avatar");
        if (parts.Length > 0)
        {
            foreach(GameObject part in parts)
            {
                part.SetActive(false);
            }
        }
        //parts[bodyPart.bodyPartType].GetComponent<Image>().sprite = null;
    }
}
