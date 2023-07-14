using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public static AudioSource playerSource;  // playerSource is source to play all sounds related to player
    [SerializeField] int playerHealth = 100;
    [SerializeField] Image healthBar;
    public const int HEAD_DMG_MULTIPLAYER = 10;
    public const int BODY_DMG_MULTIPLAYER = 5;
    public const int LIMBS_DMG_MULTIPLAYER = 2;

    private void Awake()
    {
        playerSource = GameObject.Find("PlayerSoundSource").GetComponent<AudioSource>();
    }

    public IEnumerator ShowInfoText(string text)
    {
        infoText.text = text;
        infoText.alpha = 1;
        yield return new WaitForSeconds(1);
        while (infoText.alpha > 0)
        {
            infoText.alpha -= Time.deltaTime;
            yield return null;
        }
    }

    public void GivePlayerDamage(float value)
    {
        playerHealth -= (int)value;
        healthBar.fillAmount -= value / 100;
    }
}
