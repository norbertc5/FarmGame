using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ViewRange : MonoBehaviour
{
    HumanEnemy enemy;
    bool isPlayerInView;

    private void Awake()
    {
        enemy = GetComponentInParent<HumanEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // when player enter into the view range ChangeBehaviourIfSeePlayer invokes
        if (other.CompareTag("Player"))
            StartCoroutine(enemy.ChangeBehaviourIfSeePlayer(true));
    }

    private void OnTriggerStay(Collider other)
    {
        // all time when player is in the view range isPlayerInView is true
        if (other.CompareTag("Player"))
            isPlayerInView = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // when palyer leves view range
        if (other.CompareTag("Player"))
        {
            isPlayerInView = false;
            StartCoroutine(CheckIfSeePlayer());
        }
    }

    /// <summary> Waits some time and check if palyer is in the view range. </summary>
    IEnumerator CheckIfSeePlayer()
    {
        // it's to avoid error when player is still in view range but the enemy is still moving
        yield return new WaitForSeconds(1f);

        if(!isPlayerInView)
            StartCoroutine(enemy.ChangeBehaviourIfSeePlayer(false));
    }
}
