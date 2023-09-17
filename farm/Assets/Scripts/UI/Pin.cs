using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField] GameObject directionPin;  // pin which rotates on edge of mini map
    Transform playerTrans;

    private void Awake()
    {
        playerTrans = FindObjectOfType<Player>().transform;
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(90, playerTrans.eulerAngles.y, 0);  // make the pin always upright

        // show or hide directionPin
        if (Vector3.Distance(playerTrans.position, transform.position) <= 50)
            directionPin.gameObject.SetActive(false);
        else
            directionPin.gameObject.SetActive(true);
    }
}
