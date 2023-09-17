using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DamageIndicator : MonoBehaviour
{
    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        StartCoroutine(HideWithDelay());
        canvasGroup.alpha = 1;
    }

    /// <summary> Wait some time and smoothly hide indicator. </summary>
    IEnumerator HideWithDelay()
    {
        yield return new WaitForSeconds(3);
        while(canvasGroup.alpha >= 0)
        {
            canvasGroup.alpha -= Time.deltaTime;

            if(canvasGroup.alpha <= 0)
                this.gameObject.SetActive(false);

            yield return null;
        }
    }
}
