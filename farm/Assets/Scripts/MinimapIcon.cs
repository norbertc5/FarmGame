using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        // moving the icon to the target
        transform.position = target.position;
    }
}
