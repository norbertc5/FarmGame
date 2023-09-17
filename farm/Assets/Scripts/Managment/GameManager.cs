using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public static AudioSource playerSource;  // playerSource is a source to play all sounds related to player
    public const int HEAD_DMG_MULTIPLAYER = 10;
    public const int BODY_DMG_MULTIPLAYER = 5;
    public const int LIMBS_DMG_MULTIPLAYER = 2;
    [Header("Post processing")]
    public VolumeProfile volumeProfile;
    [HideInInspector] public Vignette vignette;
    [Header("Dialogue system")]
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] AudioClip dialogueClip;
    [SerializeField] AudioSource dialogueSource;
    [Header("Tutorial bar")]
    [SerializeField] GameObject tutorialBar;

    public delegate void Action();
    public Action OnInteractionWithVehicle;
    public Action OnLoudShoot;
    public Action BushOptymalization;

    private void Awake()
    {
        playerSource = GameObject.Find("PlayerSoundSource").GetComponent<AudioSource>();
        volumeProfile.TryGet(out vignette);
        vignette.intensity.value = 0;
        StartCoroutine(BushOptymalizationRepeating());
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        PlayDialogue("You", "What a beautiful day!", dialogueClip);
        StartCoroutine(ShowTutorialBar("Use wsad to move."));
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

    /// <summary> Play dialogue sound and show subtitles. </summary>
    /// <param name="character"></param>
    /// <param name="text"></param>
    /// <param name="sound"></param>
    void PlayDialogue(string character, string text, AudioClip sound)
    {
        dialogueSource.PlayOneShot(sound);
        StringBuilder sb = new StringBuilder(character + ": " + text);
        sb.Replace(character, "<b><color=orange>" + character + "</b></color>");  // make charackter name orange
        string s = sb.ToString();
        sb.Clear();
        // make background for text. To work well is necessary to change the font.
        // In inspector font must be set to the other one which I don't want to use and here it's changes to properly one.
        sb.Append("<font=LiberationSans SDF - Fallback><mark=#383838>" + s + "</font></mark>");
        dialogueText.text = sb.ToString();
        StartCoroutine(HideDialogueText(sound.length));
    }

    IEnumerator HideDialogueText(float delay)
    {
        yield return new WaitForSeconds(delay);
        CanvasGroup canvasGroup = dialogueText.GetComponent<CanvasGroup>();
        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }
    }

    /// <summary> Show tutorial bar and hide it with delay. </summary>
    /// <param name="text"></param>
    IEnumerator ShowTutorialBar(string text)
    {
        Animator helpBarAnim = tutorialBar.GetComponent<Animator>();
        helpBarAnim.CrossFade("Show", 0);
        tutorialBar.GetComponentInChildren<TextMeshProUGUI>().text = text;
        yield return new WaitForSeconds(5);
        helpBarAnim.CrossFade("Hide", 0);
    }

    /// <summary> Invoke BushOptymalization coroutine in same delays. </summary>
    IEnumerator BushOptymalizationRepeating()
    {
        while(true)
        {
            yield return new WaitForSeconds(5);
            BushOptymalization?.Invoke();
            yield return null;
        }
    }
}
