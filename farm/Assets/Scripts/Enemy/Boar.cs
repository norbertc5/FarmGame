using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Boar : Enemy
{
    [SerializeField] float attackDistance = 2;
    [SerializeField] Transform hitColliderPos;
    [SerializeField] LayerMask hitableLayers;
    [SerializeField] AudioClip hitSound;
    bool hitPlayer;

    private void Awake()
    {
        AssignComponents();
        SetRagdollActive(false);
    }

    private void Update()
    {
        if(Vector3.Distance(player.position, transform.position) <= attackDistance)
        {
            animator.CrossFade("boarAttack", 0);
            StartCoroutine(StopWalkForAttack());
        }
    }

    private void FixedUpdate()
    {
        hitPlayer = Physics.CheckBox(hitColliderPos.position, Vector3.one, Quaternion.identity, hitableLayers);
    }

    /// <summary> Make boar stops walking for some time. </summary>
    IEnumerator StopWalkForAttack()
    {
        pathfinding.enabled = false;
        yield return new WaitForSeconds(1);
        pathfinding.enabled = true;
    }

    /// <summary> Attack to player. </summary>
    void Attack()
    {
        if(hitPlayer)
        {
            player.GetComponent<Player>().GiveDamageToPlayer(10);
            source.PlayOneShot(hitSound);
        }
    }
}
