using UnityEngine;

public class RaceCar : Car
{
    [SerializeField] Transform helper;
    [SerializeField] Transform targetsParent;
    Transform[] targets;
    Transform target;
    int targetIndex;
    float steeringWheelsAngle;

    void Start()
    {
        targets = new Transform[targetsParent.childCount];
        for (int i = 0; i < targetsParent.childCount; i++)
        {
            targets[i] = targetsParent.GetChild(i);
        }

        target = targets[targetIndex];
        SetCenterOfMass();
    }

    void Update()
    {
        RotateWheelsMeshes();
        #region Leading vehicel to target.

        helper.LookAt(target);
        steeringWheelsAngle = -transform.eulerAngles.y + helper.eulerAngles.y;
        //Debug.Log(-90 + helper.eulerAngles.y);

        // make steering wheels don't get over the range
        /*if (steeringWheelsAngle < -maxSteeringAngle)
            steeringWheelsAngle = -maxSteeringAngle - 5;
        if (steeringWheelsAngle > maxSteeringAngle)
            steeringWheelsAngle = maxSteeringAngle - 5;*/

        // braking on curves when driving fast
        if(Mathf.Abs(steeringWheelsAngle) > 15 && GetComponent<Rigidbody>().velocity.x > 15)
        {
            axleInfos[1].leftWheel.brakeTorque = 2000;
            axleInfos[1].rightWheel.brakeTorque = 2000;
        }
        else
        {
            axleInfos[1].leftWheel.brakeTorque = 0;
            axleInfos[1].rightWheel.brakeTorque = 0;
        }

        #endregion

        // reach the target
        if (Vector3.Distance(transform.position, targets[targetIndex].position) < 10)
        {
            if (targetIndex < targetsParent.childCount-1)
                targetIndex++;
            else
                targetIndex = 0;
            target = targets[targetIndex];
        }
    }

    private void FixedUpdate()
    {
        Drive(maxMotorTorque, steeringWheelsAngle);
    }
}
