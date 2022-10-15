using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEditor;

public class PlayerTest : NetworkBehaviour
{
    public Image image;
    public int speed = 20;

    void Update()
    {

        Vector3 vector3 = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        image.transform.Translate(vector3 * speed);
    }
}
