using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchWithSpectators : MonoBehaviour
{
    Spectator[] spectators;

    private void Awake()
    {
        // turn two random spectators on the bench off. It makes each one looks a bit different
        spectators = GetComponentsInChildren<Spectator>();
        for (int i = 0; i <= 2; i++)
        {
            if (spectators[i].gameObject.activeSelf == true)
                spectators[Random.Range(0, spectators.Length)].gameObject.SetActive(false);
        }
    }
}
