using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashUI : MonoBehaviour
{
    public Image panel;
    public AudioSource flashSound;

    float elapsedTime = 0;
    public float toTheFlashTime = 1;
    public float outTheFlashTime = 1;

    private int playerCount;
    bool isFlashedShowed;

    private void Start()
    {
        flashSound = GetComponent<AudioSource>();
        panel.gameObject.SetActive(false);
        isFlashedShowed = false;
    }
    private void Update()
    {

        if (GameObject.FindGameObjectsWithTag("Frag").Length == 0 && !isFlashedShowed)
        {
            StartCoroutine(Flash());
            isFlashedShowed = true;
        }

    }

    IEnumerator Flash()
    {
        panel.gameObject.SetActive(true);
         
        Color alpha = panel.color;
        alpha.a = 0;

        elapsedTime = 0;
        while(alpha.a < 1f)
        { 
            elapsedTime += Time.deltaTime / toTheFlashTime;
            alpha.a = Mathf.Lerp(0, 1, elapsedTime);
            panel.color = alpha;
            yield return null;
        }

        elapsedTime = 0;
        flashSound.Play();
        while (alpha.a > 0f)
        {
            elapsedTime += Time.deltaTime / outTheFlashTime;
            alpha.a = Mathf.Lerp(1, 0, elapsedTime);
            panel.color = alpha;
            yield return null;
        }

        if(alpha.a == 0f)
            panel.gameObject.SetActive(false);

        yield return null;
    }
}
