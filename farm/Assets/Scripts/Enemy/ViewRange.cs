using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRange : MonoBehaviour
{
    Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(enemy.ChangeBehaviourIfSeePlayer(true));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(enemy.ChangeBehaviourIfSeePlayer(false));
        }
    }
}
