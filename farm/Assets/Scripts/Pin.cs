using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform helper;
    RectTransform rectTrans;

    void Start()
    {
        rectTrans= GetComponent<RectTransform>();
    }

    void Update()
    {
        helper.LookAt(target);
        rectTrans.eulerAngles = new Vector3(0f, 0f, -helper.localEulerAngles.y);
    }
}
