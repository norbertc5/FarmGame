using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class Weapon : MonoBehaviour
{
    [Header("Ammo")]
    [Space(6)]
    public int magazineCapacity;
    public int ammoAmount;
    [HideInInspector] public int ammoInMagazine;

    [Space(10)]
    [Header("Fire")]
    [Space(6)]
    public bool isAuto;
    [Tooltip("Fire rate for auto weapons.")]
    public float fireRate = 0.1f;

    [Space(10)]
    [Header("References")]
    [Space(6)]
    public Transform flashTrasform;
    [Tooltip("Animator for second hand while reloading.")]
    public Animator additinonalReloadingAnim;

    private void Awake()
    {
        ammoInMagazine = magazineCapacity;
    }
}
