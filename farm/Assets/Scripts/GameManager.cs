using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI infoText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
