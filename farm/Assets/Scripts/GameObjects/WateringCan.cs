using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCan : MonoBehaviour
{
    Animator animator;
    new ParticleSystem particleSystem;
    [SerializeField] GameObject weaponWheel;
    [HideInInspector] public bool isWatering;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !weaponWheel.activeSelf)
        {
            animator.CrossFade("TiltWateringCan", 0);
            particleSystem.Play();
            isWatering = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            animator.CrossFade("ReturnToIdle", 0);
            particleSystem.Stop();
            isWatering = false;
        }
    }
}
