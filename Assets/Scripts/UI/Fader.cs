using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [SerializeField] private Image fadeOverlay;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private bool fadeInOnStart = false;

    public IEnumerator FadeIn()  => Fade(1f, 0f, true);
    public IEnumerator FadeOut() => Fade(0f, 1f, false);

    void Start()
    {
        if (fadeInOnStart)
            StartCoroutine(FadeIn());
    }
    
    IEnumerator Fade(float from, float to, bool disableAtEnd)
    {
        if (fadeOverlay == null) yield break;

        fadeOverlay.gameObject.SetActive(true);
        Color c = fadeOverlay.color;
        c.a = from;
        fadeOverlay.color = c;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = t / fadeDuration;
            k = k * k;
            c.a = Mathf.Lerp(from, to, k);
            fadeOverlay.color = c;
            yield return null;
        }
        c.a = to;
        fadeOverlay.color = c;

        if (disableAtEnd) fadeOverlay.gameObject.SetActive(false); 
    }
}