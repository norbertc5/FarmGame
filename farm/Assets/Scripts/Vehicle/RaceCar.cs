using UnityEngine;

public class RaceCar : Car
{
    [SerializeField] Transform steerinHelper;
    [SerializeField] Transform targetsParent;
    Transform[] targets;
    Transform target;
    int targetIndex;
    float steeringWheelsAngle;
    [SerializeField] float n = 10;

    private void Awake()
    {
        targets = new Transform[targetsParent.childCount];
    }

    void Start()
    {
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

        steerinHelper.LookAt(target);
        steeringWheelsAngle = -transform.eulerAngles.y + steerinHelper.eulerAngles.y;

        // reach the target
        if (Vector3.Distance(transform.position, targets[targetIndex].position) < n)
        {
            if (targetIndex < targetsParent.childCount - 1)
                targetIndex++;
            else
                targetIndex = 0;
            target = targets[targetIndex];
        }

        // braking on curves when driving fast
        /* if(Mathf.Abs(steeringWheelsAngle) > 15 && GetComponent<Rigidbody>().velocity.x > 15)
         {
             axleInfos[1].leftWheel.brakeTorque = 2000;
             axleInfos[1].rightWheel.brakeTorque = 2000;
         }
         else
         {
             axleInfos[1].leftWheel.brakeTorque = 0;
             axleInfos[1].rightWheel.brakeTorque = 0;
         }*/

        #endregion
    }

    private void FixedUpdate()
    {
        // operation on wheel colliders
        Drive(maxMotorTorque, steeringWheelsAngle);
    }
}
