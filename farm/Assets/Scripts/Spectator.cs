using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : Enemy
{
    [SerializeField] int animationIndex;
    bool checking;
    CharacterController characterController;

    private void Awake()
    {
        AssignComponents();
        SetRagdollActive(false);
        animator.SetInteger("Index", animationIndex);
        animator.enabled = true;
    }

    private IEnumerator Start()
    {
        // set spectator's pos on beginning
        yield return new WaitForSeconds(Random.Range(3, 6));
        animator.enabled = false;
        checking = true;
    }

    void Update()
    {
        // distance to player won't be checked when animation is setting up
        if(checking && health > 0)
        {
            // spectator doesn't have animation if far from player. To optymalization
            if (Vector3.Distance(transform.position, player.transform.position) > 20)
                animator.enabled = false;
            else
                animator.enabled = true;
        }
    }
}
