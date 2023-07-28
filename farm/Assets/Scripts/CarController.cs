using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using static Unity.Burst.Intrinsics.Arm;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class CarController : MonoBehaviour
{
    // script taken from Unity web page
    [Header("Settings")]
    [SerializeField] List<AxleInfo> axleInfos;
    [SerializeField] float maxMotorTorque;
    [SerializeField] float maxSteeringAngle;
    [Header("Wheels meshes")]
    [SerializeField] Transform frontLeftWheel;
    [SerializeField] Transform frontRightWheel;
    [SerializeField] Transform rearLeftWheel;
    [SerializeField] Transform rearRightWheel;
    [Header("Other objects")]
    [SerializeField] Transform centerOfMass;
    [SerializeField] Transform cameraTransform;
    [SerializeField] GameObject humanObject;
    public Transform playerGetOffTrans;
    [Header("Sounds")]
    [SerializeField] AudioClip engineStartSound;
    [SerializeField] AudioClip engineStopSound;
    AudioSource source;
    bool canChangePitch;

    private void Awake()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");

        // slowing down when no vertical input
        if (verticalInput == 0)
        {
            axleInfos[1].leftWheel.brakeTorque = 2000;
            axleInfos[1].rightWheel.brakeTorque = 2000;
        }
        else
        {
            axleInfos[1].leftWheel.brakeTorque = 0;
            axleInfos[1].rightWheel.brakeTorque = 0;
        }

        // braking
        if(Input.GetKey(KeyCode.Space))
        {
            axleInfos[1].leftWheel.brakeTorque = 20000;
            axleInfos[1].rightWheel.brakeTorque = 20000;
        }

        #region Wheels visual effects

        // rotating steering wheels according to drive direction
        frontLeftWheel.localEulerAngles = new Vector3
            (frontLeftWheel.localEulerAngles.x, axleInfos[0].leftWheel.steerAngle - 90, frontLeftWheel.localEulerAngles.z);
        frontRightWheel.localEulerAngles = new Vector3
            (frontRightWheel.localEulerAngles.x, axleInfos[0].rightWheel.steerAngle - 90, frontRightWheel.localEulerAngles.z);

        // rotating wheels around their own axis
        frontLeftWheel.Rotate(0, 0, axleInfos[0].leftWheel.rpm / 60 * -360 * Time.deltaTime);
        frontRightWheel.Rotate(0, 0, axleInfos[0].rightWheel.rpm / 60 * -360 * Time.deltaTime);
        rearLeftWheel.Rotate(0, 0, axleInfos[1].leftWheel.rpm / 60 * -360 * Time.deltaTime);
        rearRightWheel.Rotate(0, 0, axleInfos[1].rightWheel.rpm / 60 * -360 * Time.deltaTime);

        #endregion

        #region Vehicle sound

        // change pitch to simulate engine work
        if(canChangePitch)
        {
            if (verticalInput > 0 || verticalInput < 0)
                source.pitch += Time.deltaTime / 2;
            else
                source.pitch -= Time.deltaTime / 2;
            source.pitch = Mathf.Clamp(source.pitch, 0.8f, 1.4f);
        }

        #endregion
    }

    public void FixedUpdate()
    {
        // steering vehicle
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
    }

    /// <summary> Set all things needed to drive. </summary>
    /// <param name="getOn"></param>
    void PlayerInteraction(bool getOn)
    {
        // invoke when player get off/on vehicle
        transform.Find("Camera").gameObject.SetActive(getOn);
        humanObject.SetActive(getOn);
        GetComponent<Rigidbody>().isKinematic = !getOn;

        if (getOn)
            StartCoroutine(PlayEngineStartSound());
        else
        {
            StartCoroutine(PlayEngineStopSound());
        }
    }

    IEnumerator PlayEngineStartSound()
    {
        source.enabled = true;
        canChangePitch = false;
        source.pitch = 1;
        source.PlayOneShot(engineStartSound, 0.5f);
        yield return new WaitForSeconds(engineStartSound.length);
        canChangePitch = true;
        source.pitch = 0.8f;
        source.Play();
    }

    IEnumerator PlayEngineStopSound()
    {
        source.Stop();
        source.pitch = 0.8f;
        source.PlayOneShot(engineStopSound, 0.5f);
        yield return new WaitForSeconds(engineStopSound.length);
        source.enabled = false;
    }
}