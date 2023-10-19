using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaceCollisionForAllCars : MonoBehaviour
{
    // this class is response for detecting collisions with checkpoints during race

    public int Lap { get; private set; }
    RaceManager raceManager;
    int crossedCheckpointsAmount;
    Car thisCar;

    private void Awake()
    {
        raceManager = FindObjectOfType<RaceManager>();
        thisCar = GetComponent<Car>();
    }

    protected void OnTriggerEnter(Collider other)
    {
        // when the winning car reach a checkpoint
        if (other.CompareTag("RaceCheckpoint") && Array.IndexOf(raceManager.carsInWiningOrder, thisCar) == 0)
        {
            raceManager.SetNewCheckpoint();
            other.tag = "LapCheck";  // change tag of checkpoint to make it impossible to set new checkpoint by each car
        }

        if (other.CompareTag("LapCheck"))
        {
            crossedCheckpointsAmount++;

            // if the car has crossed all checkpoints, it means that one lap has been finished
            if (crossedCheckpointsAmount == raceManager.checkpoints.Length)
            {
                Lap++;
                crossedCheckpointsAmount = 0;

                foreach (Transform checkpoint in raceManager.checkpoints)
                {
                    checkpoint.tag = "RaceCheckpoint";
                }
            }
        }
    }
}
