using System.Collections;
using UnityEngine;

public class PanelPopup : MonoBehaviour
{
    [SerializeField] private float[] openSteps = { 0f, 0.5f, 1.15f, 1f }; 
    [SerializeField] private float stepTime = 0.05f;

    private Coroutine anim;

    public void Open()
    {
        gameObject.SetActive(true);
        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(Step(openSteps, false));
    }

    public void Close()
    {
        if (!gameObject.activeSelf) return;
        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(Step(new float[] { 1f, 0.5f, 0f }, true));
    }

    IEnumerator Step(float[] steps, bool disableAtEnd)
    {
        Vector3 s = transform.localScale;
        foreach (float v in steps)
        {
            s.y = v;
            transform.localScale = s;
            yield return new WaitForSeconds(stepTime);
        }
        if (disableAtEnd) gameObject.SetActive(false);
    }
}