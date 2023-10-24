using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;
using UnityEngine.UIElements;

public class RaceManager : MonoBehaviour
{
    // Checkpoints are invisible triggers which are detected by the cars durring race.
    // Based on info about checkpoints, RaceManager counts which car is winning the race.
    // Checkpoints also are used to get info about actual lap for each car.

    [Header("Race managing")]
    public Car[] carsInWiningOrder = new Car[3];
    [HideInInspector] public Transform[] checkpoints;
    [SerializeField] Car[] carsInRace;
    [SerializeField] Car playerCar;
    [SerializeField] Transform checkpointsParent;
    [SerializeField] int timeScale = 1;
    RaceCollisionForAllCars playerLapCounter;
    RaceCollisionForAllCars firstCarLapCounter;
    Transform actualCheckpoint;
    int checkpointIndex;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI playerPlaceText;
    [SerializeField] TextMeshProUGUI lapText;

    void Awake()
    {
        checkpoints = new Transform[checkpointsParent.childCount];
        for (int i = 0; i < checkpointsParent.childCount; i++)
        {
            checkpoints[i] = checkpointsParent.GetChild(i);
        }

        actualCheckpoint = checkpoints[0];
        playerLapCounter = playerCar.GetComponent<RaceCollisionForAllCars>();
        Time.timeScale = timeScale;
    }

    void Update()
    {
        // first car in race
        firstCarLapCounter = carsInRace.OrderBy(t => Vector3.Distance(actualCheckpoint.transform.position,t.transform.position))
            .FirstOrDefault().GetComponent<RaceCollisionForAllCars>();

        carsInWiningOrder = carsInRace.OrderBy(t => Vector3.Distance(actualCheckpoint.transform.position, t.transform.position)).ToArray();

        // make player don't become 1st for a while when he's staying in place and opponents pass him
        if(playerCar != carsInWiningOrder[carsInWiningOrder.Length - 1])
        {
            int rcfacPlayer = playerCar.GetComponent<RaceCollisionForAllCars>().TotalCrosses;
            int rcfacLastOpponent = carsInWiningOrder[carsInWiningOrder.Length - 1].GetComponent<RaceCollisionForAllCars>().TotalCrosses;
            if(rcfacLastOpponent > rcfacPlayer)
            {
                // change place in carsInWiningOrder between player and lastCar
                Car lastCar = carsInWiningOrder[carsInWiningOrder.Length - 1];
                int idOfPlayer = Array.IndexOf(carsInWiningOrder, playerCar);
                carsInWiningOrder[idOfPlayer] = lastCar;
                carsInWiningOrder[carsInWiningOrder.Length - 1] = playerCar;
            }
        }

        playerPlaceText.text = $"Place: {Array.IndexOf(carsInWiningOrder, playerCar) + 1} / {carsInRace.Length}";
        lapText.text = $"Lap: {playerLapCounter.Lap}";
    }

    /// <summary> Automaticly set next checkpoint. To use when a car touch one. </summary>
    public void SetNewCheckpoint(int newCheckpointId)
    {
        checkpointIndex = newCheckpointId;
        try { actualCheckpoint = checkpoints[checkpointIndex]; }
        catch { actualCheckpoint = checkpoints[1]; };
    }
}
