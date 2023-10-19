using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    // script taken from Unity web page

    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
    }

    [Header("Settings")]
    [SerializeField] protected List<AxleInfo> axleInfos;
    [SerializeField] protected float maxMotorTorque;
    [SerializeField] protected float maxSteeringAngle;
    [SerializeField] Transform centerOfMass;
    [Header("Wheels meshes")]
    [SerializeField] protected Transform frontLeftWheel;
    [SerializeField] protected Transform frontRightWheel;
    [SerializeField] protected Transform rearLeftWheel;
    [SerializeField] protected Transform rearRightWheel;

    /// <summary> Move vehicel. Add motorTorque and angle to steering wheels. </summary>
    /// <param name="motorValue"></param>
    /// <param name="steeringValue"></param>
    /// <param name="steeringByPlater"></param>
    protected void Drive(float motorValue, float steeringValue, bool steeringByPlater = true)
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)  // add angle to steering wheels
            {
                axleInfo.leftWheel.steerAngle = steeringValue;
                axleInfo.rightWheel.steerAngle = steeringValue;
            }
            if (axleInfo.motor)  // add motorTorque
            {
                axleInfo.leftWheel.motorTorque = motorValue;
                axleInfo.rightWheel.motorTorque = motorValue;
            }
        }
    }

    /// <summary> Add rotation to wheels. </summary>
    protected void RotateWheelsMeshes()
    {
        // rotating wheels around their own axis
        frontLeftWheel.Rotate(0, 0, axleInfos[0].leftWheel.rpm / 60 * -360 * Time.deltaTime);
        frontRightWheel.Rotate(0, 0, axleInfos[0].rightWheel.rpm / 60 * -360 * Time.deltaTime);
        rearLeftWheel.Rotate(0, 0, axleInfos[1].leftWheel.rpm / 60 * -360 * Time.deltaTime);
        rearRightWheel.Rotate(0, 0, axleInfos[1].rightWheel.rpm / 60 * -360 * Time.deltaTime);

        // rotating steering wheels according to drive direction
        frontLeftWheel.localEulerAngles = new Vector3
            (frontLeftWheel.localEulerAngles.x, axleInfos[0].leftWheel.steerAngle - 90, frontLeftWheel.localEulerAngles.z);
        frontRightWheel.localEulerAngles = new Vector3
            (frontRightWheel.localEulerAngles.x, axleInfos[0].rightWheel.steerAngle - 90, frontRightWheel.localEulerAngles.z);
    }

    /// <summary> Set center of mass in rigidbody. To use in Awake() in derived classes. </summary>
    protected void SetCenterOfMass()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
        //raceManager = FindObjectOfType<RaceManager>();  // also do this in this function
    }
}
