using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class Weapon : MonoBehaviour
{
    [Header("General")]
    public string weaponName;

    [Header("Ammo")]
    public int magazineCapacity;
    public int ammoAmount;
    [HideInInspector] public int ammoInMagazine;

    [Header("Fire")]
    public bool isAuto;
    public bool isMelee;
    [Tooltip("Fire rate for auto weapons.")]
    public float fireRate = 0.1f;

    [Header("References")]
    public Transform flashTrasform;
    [Tooltip("Animator for second hand while reloading.")]
    public Animator additinonalReloadingAnim;

    private void Awake()
    {
        ammoInMagazine = magazineCapacity;
    }
}
