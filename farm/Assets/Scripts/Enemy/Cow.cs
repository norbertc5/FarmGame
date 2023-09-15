using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : Enemy
{
    private void Awake()
    {
        AssignComponents();
        SetRagdollActive(false);
        StartCoroutine(EnabelAnimatorWithDealy());
    }

    IEnumerator EnabelAnimatorWithDealy()
    {
        // make each cow play idle anim a bit different
        yield return new WaitForSeconds(Random.Range(0, 6));
        animator.enabled = true;
    }
}
