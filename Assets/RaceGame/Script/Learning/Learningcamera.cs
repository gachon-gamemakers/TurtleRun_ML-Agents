using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Learningcamera : MonoBehaviour
{
    public Transform pos;
    void Update()
    {
        transform.position = new Vector3(pos.position.x, pos.position.y, -10);
    }
}
