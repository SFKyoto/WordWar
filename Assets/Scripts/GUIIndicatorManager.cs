using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIIndicatorManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void MoveIndicator(float pos)
    {
        transform.position = new Vector3((-2.5f)+pos,4.5f,0);
    }
}
