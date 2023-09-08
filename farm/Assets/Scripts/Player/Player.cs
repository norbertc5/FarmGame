using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : PlayerManager
{
    [Header("Health")]
    public float playerHealth = 100;
    [SerializeField] int lowHealthFrom = 20;
    [SerializeField] Image healthBar;
    Coroutine showVignietteCoroutine;
    float timeToPlayPainSound;

    [Header("Sounds")]
    [SerializeField] AudioClip playerDamageSound;
    [SerializeField] AudioClip playerHealSound;

    [Header("Vehicle")]
    bool isNearVehicle;
    CarController actualVehicle;

    [Header("References")]
    [SerializeField] GameObject interactionSign;
    DamageIndicator damageIndicator;
    PlayerMovement playerMovement;

    [Header("Door using")]
    bool canUseDoor;
    Door actualDoor;

    private void Awake()
    {
        damageIndicator = FindObjectOfType<DamageIndicator>();
        playerMovement = GetComponent<PlayerMovement>();
        damageIndicator.gameObject.SetActive(false);
        UpdatePlayerHealth();
    }

    private void Update()
    {
        if (timeToPlayPainSound >= 0)
            timeToPlayPainSound -= Time.deltaTime;

        #region Interaction with vehicles

        // get on vehicle
        if (isNearVehicle && Input.GetKeyDown(KeyCode.E) && !isPlayerInVehicle)
            StartCoroutine(VehicleInteraction(true));

        // get off vehicle
        if(isPlayerInVehicle && Input.GetKeyDown(KeyCode.E))
            StartCoroutine(VehicleInteraction(false));

        #endregion

        #region Interaction with doors

        if (canUseDoor && Input.GetKeyDown(KeyCode.E))
            actualDoor.Use(-90);

        #endregion
    }

    /// <summary> Update health bar. </summary>
    void UpdatePlayerHealth()
    {
        healthBar.fillAmount = playerHealth / 100;
        playerHealth = Mathf.Clamp(playerHealth, 0, 100);
    }

    /// <summary> Decrease player health with effects (e.g. sounds, red vignette). </summary>
    /// <param name="value"></param>
    public void GiveDamageToPlayer(float value)
    {
        playerHealth -= (int)value;
        UpdatePlayerHealth();
        damageIndicator.gameObject.SetActive(true);

        // show red vignette
        if (playerHealth <= lowHealthFrom && showVignietteCoroutine == null)
            showVignietteCoroutine = StartCoroutine(ShowVignette());

        // play damage sound with dealy
        if (timeToPlayPainSound <= 0)
        {
            playerSource.PlayOneShot(playerDamageSound);
            timeToPlayPainSound = 0.5f;
        }

        // death
        if (playerHealth <= 0)
        {
            animator.CrossFade("playerDeath", 0);
            GetComponent<PlayerMovement>().enabled = false;
            GetComponentInChildren<Shooting>().GetComponentInChildren<Animator>().CrossFade("weaponChange", 0);
        }
    }

    /// <summary> Return health to player. </summary>
    /// <param name="value"></param>
    public void HealPlayer(float value)
    {
        playerHealth += (int)value;
        UpdatePlayerHealth();
        playerSource.PlayOneShot(playerHealSound);
    }

    /// <summary> Show red vignette by post processing and make it pulse. </summary>
    IEnumerator ShowVignette()
    {
        float speed = 0.1f;
        while (true)
        {
            // increasing size of vignette
            while (gameManager.vignette.intensity.value <= 0.3f)
            {
                // don't increase when playerHealth is bigger whan lowHealthFrom
                if (playerHealth > lowHealthFrom)
                    break;

                gameManager.vignette.intensity.value += Time.deltaTime * speed;
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

            // decreasing size of vignette
            while ((gameManager.vignette.intensity.value >= 0.2f && playerHealth <= lowHealthFrom) ||
                (gameManager.vignette.intensity.value >= 0f && playerHealth > lowHealthFrom))
            {
                gameManager.vignette.intensity.value -= Time.deltaTime * speed;
                yield return null;
            }

            // stop when playerHealth is bigger whan lowHealthFrom
            if (gameManager.vignette.intensity.value <= 0)
            {
                StopCoroutine(showVignietteCoroutine);
                showVignietteCoroutine = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            isNearVehicle = true;
            actualVehicle = other.GetComponentInParent<CarController>();
            interactionSign.SetActive(true);
        }
        if(other.CompareTag("Door"))
        {
            canUseDoor = true;
            actualDoor = other.GetComponentInParent<Door>();
            interactionSign.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            isNearVehicle = false;
            interactionSign.SetActive(false);
        }
        if (other.CompareTag("Door"))
        {
            canUseDoor = false;
            interactionSign.SetActive(false);
        }
    }

    /// <summary> Make player steering a vehicle and vice versa. </summary>
    /// <param name="getOn"> Is player getting on or off vehicle. </param>
    IEnumerator VehicleInteraction(bool getOn)
    {
        yield return new WaitForSeconds(0.2f);  // delay is needed to avoid glitch
        gameManager.OnInteractionWithVehicle?.Invoke();
        transform.position = actualVehicle.playerGetOffTrans.position;
        isPlayerInVehicle = getOn;
        PlayerMovement.ChangeMovementPossibility(!getOn);
        actualVehicle.enabled = getOn;
        characterController.enabled = !getOn;
        playerMovement.enabled = !getOn;
        crosshair.gameObject.SetActive(!getOn);
        playerCamera.enabled = !getOn;
        playerCamera.GetComponent<AudioListener>().enabled = !getOn;
        actualVehicle.SendMessage("PlayerInteraction", getOn);
        Shooting.canPlayerChangeWeapon = !getOn;
        interactionSign.SetActive(!getOn);
    }
}
