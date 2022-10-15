using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class PlayerData
{
    public string username;
    public BodyPartList bodyPartList;
    public ushort Id;// { get; private set; }
    public BodyPartList Avatar;// { get; private set; }
    public ushort PalavraAtual;// { get; private set; }
    public ushort QtdTentativas;// { get; private set; }
}

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

public class AvatarManager : MonoBehaviour
{
    public TextAsset defaultAvatar;

    public PlayerData playerData = new PlayerData();
    //public BodyPartList bodyPartList = new BodyPartList();

    //ordem corpo, olho, boca, cabelo
    public GameObject[] parts;

    void Start()
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

    // Update is called once per frame
    void Update()
    {
        // AvatarUpdate();
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

    void AvatarUpdate()
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
                //image.sprite = spriteLists[bodyPart.bodyPartType].sprite[bodyPart.bodyPartId];
                image.color = bodyPart.bodyPartColor;
            }
        }
    }
}
