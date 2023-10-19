using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

public class RaceManager : MonoBehaviour
{

    // Checkpoints are invisible triggers which are detected by the cars durring race.
    // Based on info about checkpoints, RaceManager counts which car is winning the race.
    // Checkpoints also are used to get info about actual lap for each car.

    [Header("Race managing")]
    [HideInInspector] public Car[] carsInWiningOrder = new Car[4];
    [HideInInspector] public Transform[] checkpoints;
    [SerializeField] Car[] carsInRace;
    [SerializeField] Car playerCar;
    [SerializeField] Transform checkpointsParent;
    RaceCollisionForAllCars playerLapCounter;
    Transform actualCheckpoint;
    int checkpointIndex;
    int playerRacePos;

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
    }

    void Update()
    {
        // first car in race
        var firstCar = carsInRace.OrderBy(t => Vector3.Distance(actualCheckpoint.transform.position, t.transform.position)).FirstOrDefault();

        carsInWiningOrder = carsInRace.OrderBy(t => Vector3.Distance(actualCheckpoint.transform.position, t.transform.position)).ToArray();

        playerPlaceText.text = $"Place: {playerRacePos + 1} / {carsInRace.Length}";
        lapText.text = $"Lap: {playerLapCounter.Lap}";
        playerRacePos = Array.IndexOf(carsInWiningOrder, playerCar);
    }

    /// <summary> Automaticly set next checkpoint. To use when a car touch one. </summary>
    public void SetNewCheckpoint()
    {
        if (checkpointIndex < checkpointsParent.childCount - 1)
            checkpointIndex++;
        else
            checkpointIndex = 0;

        actualCheckpoint = checkpoints[checkpointIndex];
    }
}
