using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    // this class make UI element rotates to face target
    public Transform target;
    [SerializeField] Transform helper;  // any object from the scene
    RectTransform rectTrans;

    void Start()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    void Update()
    {
        // rotating indicator is based on helper rotation
        helper.LookAt(target);
        rectTrans.eulerAngles = new Vector3(0f, 0f, -helper.localEulerAngles.y);
    }
}
