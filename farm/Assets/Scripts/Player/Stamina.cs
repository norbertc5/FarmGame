using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : PlayerManager
{
    public float playerStamina = 100;
    public bool isStaminaRenewaling;
    [SerializeField] int staminaLosingSpeed = 1;
    [SerializeField] Image staminaBar;
    [SerializeField] Color staminaBarDefaultColor;
    [SerializeField] Color staminaBarRenewalingColor;
    [SerializeField] AudioClip pantSound;

    void Start()
    {
        staminaBar.color = staminaBarDefaultColor;
    }

    void Update()
    {
        // counting stamina
        if (PlayerMovement.IsRunning)
            playerStamina -= Time.deltaTime * staminaLosingSpeed;
        else
            playerStamina += Time.deltaTime * staminaLosingSpeed / 2;

        // if no stamina, player has to wait until it renewal
        if (playerStamina <= 0)
        {
            staminaBar.color = staminaBarRenewalingColor;
            playerSource.PlayOneShot(pantSound);
            isStaminaRenewaling = true;
        }

        if (playerStamina >= 100 && isStaminaRenewaling)
        {
            staminaBar.color = staminaBarDefaultColor;
            isStaminaRenewaling = false;
        }


        staminaBar.fillAmount = playerStamina / 100;
        playerStamina = Mathf.Clamp(playerStamina, 0, 100);
    }
}
