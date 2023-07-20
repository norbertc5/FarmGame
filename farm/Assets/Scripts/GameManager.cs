using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public static AudioSource playerSource;  // playerSource is source to play all sounds related to player
    public const int HEAD_DMG_MULTIPLAYER = 10;
    public const int BODY_DMG_MULTIPLAYER = 5;
    public const int LIMBS_DMG_MULTIPLAYER = 2;
    public VolumeProfile volumeProfile;
    [HideInInspector] public Vignette vignette;

    private void Awake()
    {
        playerSource = GameObject.Find("PlayerSoundSource").GetComponent<AudioSource>();
        volumeProfile.TryGet(out vignette);
        vignette.intensity.value = 0;
    }

    /// <summary> Show text in middle of screen for some time. </summary>
    /// <param name="text"></param>
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
}
