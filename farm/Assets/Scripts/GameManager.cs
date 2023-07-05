using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public static AudioSource playerSource;  // playerSource is source to play all sounds related to player

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
}
