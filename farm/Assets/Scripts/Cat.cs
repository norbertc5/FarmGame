using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Enemy
{
    Transform target;
    [SerializeField] LayerMask obstaclesLayers;
    [SerializeField] int squareArea;
    [SerializeField] float defaultSpeed = 2.5f;
    bool isTargetNearCollider;
    bool isRunning;

    private void Awake()
    {
        AssignComponents();
        SetRagdollActive(false);
        target = GetComponent<AIDestinationSetter>().target;
        pathfinding.maxSpeed = defaultSpeed;
        gameManager.OnLoudShoot += Run;
    }

    private void Update()
    {
        // reach the target
        if (Vector3.Distance(transform.position, target.position) <= 0.1f)
        {
            SetTargetPosition();
            if(isRunning)
            {
                RunningToggle(false);
                isRunning = false;
            }

        }

        // when taget is in any abstacle
        if(isTargetNearCollider)
            SetTargetPosition();
    }

    private void FixedUpdate()
    {
        isTargetNearCollider = Physics.CheckSphere(target.position, 1, obstaclesLayers);
    }

    /// <summary> Make cat running. </summary>
    void Run()
    {
        StartCoroutine(FindFarTarget());
        isRunning = true;
    }

    /// <summary> Find position which is far from player and make the cat starts running. </summary>
    IEnumerator FindFarTarget()
    {
        while (true)
        {
            if (Vector3.Distance(target.position, player.position) <= 20)
            {
                // repeat when found position is near player
                SetTargetPosition();
                yield return null;
            }
            else
            {
                RunningToggle(true);
                yield break;
            }
        }
    }

    void SetTargetPosition()
    {
        target.position = new Vector3(Random.Range(-squareArea, squareArea + 1), 0, Random.Range(-squareArea, squareArea + 1));
    }

    /// <summary> Change cat's state (running or walking). </summary>
    /// <param name="value"></param>
    void RunningToggle(bool value)
    {
        pathfinding.maxSpeed = (value == true) ? defaultSpeed * 3 : defaultSpeed;  // speed
        if(value) animator.CrossFade("catRun", 0); else animator.CrossFade("catWalk", 0);  // animation
    }
}
