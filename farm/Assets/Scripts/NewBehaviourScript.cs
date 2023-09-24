using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    Vector3 pos;
    float y;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.localPosition;
        //y = pos.y;
    }

    // Update is called once per frame
    void Update()
    {
        float x = -Input.GetAxis("Horizontal");
        float z = -Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.E))
        {
            y -= 0.1f;
        }
        else if (Input.GetKey(KeyCode.Q))
            y += 0.1f;

        pos.z += z;
        pos.y = y;
        pos.x += x;
        transform.localPosition = pos;
    }
}
