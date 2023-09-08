using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DamageIndicator : MonoBehaviour
{
    [SerializeField] Transform playertTrans;
    [SerializeField] Transform targetTrans;
    [SerializeField] Transform helper;
    RectTransform rectTrans;
    CanvasGroup canvasGroup;

    void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        StartCoroutine(HideWithDelay());
        canvasGroup.alpha = 1;
    }

    void Update()
    {
        // rotating indicator is based on helper rotation
        helper.LookAt(targetTrans);
        rectTrans.eulerAngles = new Vector3(0f, 0f, -helper.localEulerAngles.y);
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
