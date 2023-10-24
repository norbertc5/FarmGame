using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaceCollisionForAllCars : MonoBehaviour
{
    // this class is response for detecting collisions with checkpoints during race

    public int Lap { get; private set; }
    public int TotalCrosses { get; private set; }
    RaceManager raceManager;
    Car thisCar;
    [SerializeField] List<GameObject> crossedCheckpoints;
    int crossedCheckpointsAmount;

    private void Awake()
    {
        raceManager = FindObjectOfType<RaceManager>();
        thisCar = GetComponent<Car>();
        Lap = 1;
    }

    protected void OnTriggerEnter(Collider other)
    {
        // when reach a checkpoint
        if (other.CompareTag("RaceCheckpoint"))
        {
            // if it's winning car, SetNewCheckpoint() 
            if (Array.IndexOf(raceManager.carsInWiningOrder, thisCar) == 0)
                raceManager.SetNewCheckpoint(crossedCheckpointsAmount + 1);

            crossedCheckpointsAmount++;
            TotalCrosses++;

            // if the car has crossed all checkpoints, it means that one lap has been finished
            if (crossedCheckpointsAmount == raceManager.checkpoints.Length + 1)
            {
                Lap++;
                crossedCheckpointsAmount = 1;
                crossedCheckpoints.Clear();
            }

            crossedCheckpoints.Add(other.gameObject);
        }
    }
}
