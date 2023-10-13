using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class Weapon : MonoBehaviour
{
    [Header("General")]
    public string weaponName;
    [Range(1, 10)] public int damage;
    [Range(1, 5)] public int recoil = 2;
    public int crosshairSpreadWhenShoot = 20;
    public int range = 100;
    public float additionalForceForRagdoll = 1;
    public bool isUseable = true;

    [Header("Ammo")]
    public int magazineCapacity;
    public int ammoAmount;
    [HideInInspector] public int ammoInMagazine;

    [Header("Fire")]
    public bool isAuto;
    public bool isMelee;
    [Tooltip("Fire rate for auto weapons.")]
    public float fireRate = 0.1f;
    public int raysAmount = 1;
    public float additionalDispersionSize = 1;
    public float additionalDelayForRay = 0;  // it's needed to throw ray in suitable animation moment (melee weapons)

    [Header("Sounds")]
    public AudioClip shootSound;

    [Header("References")]
    public Transform flashTrasform;
    public ShakeData shootShake;
    [Tooltip("Animator for second hand while reloading.")]
    public Animator additinonalReloadingAnim;

    private void Awake()
    {
        ammoInMagazine = magazineCapacity;
    }

    private void Start() { }  // to make possibility to turn off from inspector
}
