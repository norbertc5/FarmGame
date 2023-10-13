using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // this class storages all variables needed by other classes related to player
    protected static CharacterController characterController;
    protected static AudioSource playerSource;
    protected static Animator animator;
    protected static GameManager gameManager;
    protected static Shooting shooting;
    protected static Crosshair crosshair;
    protected static Camera playerCamera;
    protected static bool isPlayerInVehicle;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        shooting = FindObjectOfType<Shooting>();
        crosshair = FindObjectOfType<Crosshair>();
        playerCamera = GameObject.Find("PlayerCam").GetComponent<Camera>();
    }

    public bool CheckIfPlayerInVehicle()
    {
        return isPlayerInVehicle;
    }
}
