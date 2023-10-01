using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DamageIndicator : RotateToTarget
{
    CanvasGroup canvasGroup;
    float elapsedTime = 0;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        StartCoroutine(HideWithDelay());
        canvasGroup.alpha = 1;
    }

    private void Update()
    {
        // delay for setting target. To prevent indicator 'jumps' between group of enenimes who gave damage to player.
        if(elapsedTime >= 0)
            elapsedTime -= Time.deltaTime;

        RotateIndicator();
    }

    /// <summary> Wait some time and smoothly hide indicator. </summary>
    IEnumerator HideWithDelay()
    {
        yield return new WaitForSeconds(1);
        while(canvasGroup.alpha >= 0)
        {
            canvasGroup.alpha -= Time.deltaTime;

            if(canvasGroup.alpha <= 0)
                this.gameObject.SetActive(false);

            yield return null;
        }
    }

    /// <summary> Set rotation target for damage indicator. </summary>
    /// <param name="newTarget"></param>
    public void SetTarget(Transform newTarget)
    {
        if (elapsedTime > 0)
            return;

        target = newTarget;
        elapsedTime = 2;
    }
}
