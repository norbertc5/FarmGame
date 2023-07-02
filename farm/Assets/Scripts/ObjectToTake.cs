using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToTake : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 2;
    [SerializeField] int weaponIndex;
    Shooting shooting;

    private void Awake()
    {
        shooting = FindObjectOfType<Shooting>();
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            shooting.TakeWeapon(weaponIndex);
            this.gameObject.SetActive(false);
        }
    }
}
