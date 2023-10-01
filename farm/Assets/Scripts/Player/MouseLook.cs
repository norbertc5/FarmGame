using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] float sensitivity = 100;
    [SerializeField] Transform playerBody;
    [SerializeField] float deviateSpeed = 10;  // how fast cam moving while shooting
    float xRotation;
    float cameraDeviate;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90 + cameraDeviate, 90 + cameraDeviate);
        transform.localRotation = Quaternion.Euler(xRotation - cameraDeviate, 0, 0);
        playerBody.transform.Rotate(Vector3.up * mouseX);

        // camera changes rotation in x axis (vertical) when shooting
        if(Shooting.recoil > 0)
        {
            cameraDeviate += Time.deltaTime * deviateSpeed;
            Shooting.recoil -= Time.deltaTime * deviateSpeed;
        }
    }
}
