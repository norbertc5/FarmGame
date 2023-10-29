using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : Enemy
{
    [SerializeField] int animationIndex;

    private void Awake()
    {
        AssignComponents();
        SetRagdollActive(false);
        animator.SetInteger("Index", animationIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
