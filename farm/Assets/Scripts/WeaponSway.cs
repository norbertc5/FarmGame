using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] float defaultSpeed = 1;
    [SerializeField] float startSwayDelay = 1;
    [SerializeField] float swayAmplitude = 0.1f;
    [SerializeField] float elapsedTime;
    float defaulyXPos;
    float swaySpeed = 1;
    float timeToStartSway = 0;

    void Start()
    {
        defaulyXPos = transform.localPosition.x;
    }

    void Update()
    {
        transform.localPosition = new Vector3(defaulyXPos + Mathf.Sin(elapsedTime) * swayAmplitude, transform.localPosition.y);

        // swaying is possible only when player is moving
        if (PlayerMovement.IsWalking || PlayerMovement.IsRunning)
        {
            timeToStartSway += Time.deltaTime;

            // set sway speed
            if(PlayerMovement.IsWalking)
                swaySpeed = defaultSpeed;
            else if(PlayerMovement.IsRunning)
                swaySpeed = defaultSpeed * 2;
        }
        else
        {
            // reset sway when no move
            timeToStartSway = 0;
            if(PlayerMovement.IsCrouching)
                elapsedTime = 0;
        }

        // sway start with delay to avoid glithes
        if (timeToStartSway > startSwayDelay)
        {
            elapsedTime += swaySpeed * Time.deltaTime;
        }
    }
}
