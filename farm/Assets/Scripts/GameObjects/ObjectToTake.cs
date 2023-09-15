using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectToTake : MonoBehaviour
{
    float rotationSpeed = 100;
    enum ObjectType { Weapon, Health}
    [SerializeField] ObjectType objectType;

    [Header("Weapon")]
    [SerializeField] int weaponIndex;
    Shooting shooting;

    [Header("Health")]
    Player player;

    private void Awake()
    {
        switch(objectType)
        {
            case ObjectType.Weapon: shooting = FindObjectOfType<Shooting>(); break;
            case ObjectType.Health: player = FindObjectOfType<Player>(); break;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            switch(objectType)
            {
                case ObjectType.Weapon: shooting.TakeWeapon(weaponIndex); break;
                case ObjectType.Health: if (player.playerHealth < 100) player.HealPlayer(20); else return; break;  // can't pick up health when health is full
            }
            this.gameObject.SetActive(false);
        }
    }
}
