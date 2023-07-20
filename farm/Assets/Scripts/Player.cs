using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Player : MonoBehaviour
{
    public float playerHealth = 100;
    [SerializeField] int lowHealthFrom = 20;
    [SerializeField] Image healthBar;
    AudioSource source;
    [SerializeField] AudioClip playerDamageSound;
    [SerializeField] AudioClip playerHealSound;
    GameManager gameManager;
    Coroutine showVignietteCoroutine;
    Animator animator;
    float timeToPlayPainSound;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        UpdatePlayerHealth();
    }

    private void Update()
    {
        if (timeToPlayPainSound >= 0)
            timeToPlayPainSound -= Time.deltaTime;
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

        // show red vignette
        if (playerHealth <= lowHealthFrom && showVignietteCoroutine == null)
            showVignietteCoroutine = StartCoroutine(ShowVignette());

        // play damage sound with dealy
        if (timeToPlayPainSound <= 0)
        {
            source.PlayOneShot(playerDamageSound);
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

        if (playerHealth < 100)
            source.PlayOneShot(playerHealSound);
    }

    IEnumerator ShowVignette()
    {
        float speed = 0.05f;
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
}
