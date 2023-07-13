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

    [Header("Sounds")]
    public AudioClip shootSound;

    [Header("References")]
    public Transform flashTrasform;
    [Tooltip("Animator for second hand while reloading.")]
    public Animator additinonalReloadingAnim;

    private void Awake()
    {
        ammoInMagazine = magazineCapacity;
    }
}
