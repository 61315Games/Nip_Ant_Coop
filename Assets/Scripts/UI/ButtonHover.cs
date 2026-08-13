using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [System.Serializable]
    public class LabelEntry
    {
        public TMP_Text text;
        public Color normal = new Color32(0x8C, 0x8C, 0x8C, 0xFF);
        public Color hover  = Color.white;
        public bool moveOnPress = true;

        [HideInInspector] public Vector2 home;
    }

    [Header("Labels")]
    [SerializeField] private LabelEntry[] labels;

    [Header("Press")]
    [SerializeField] private Vector2 pressOffset = new Vector2(0f, -4f);
    [SerializeField] private float fadeDuration = 0.12f;

    private bool hovering, pressed;
    private Coroutine anim;

    void Awake()
    {
        foreach (var e in labels)
        {
            if (e == null || e.text == null) continue;
            e.home = e.text.rectTransform.anchoredPosition;
            e.text.color = e.normal;
        }
    }

    public void OnPointerEnter(PointerEventData p) { hovering = true;  Apply(); }
    public void OnPointerExit (PointerEventData p) { hovering = false; pressed = false; Apply(); }
    public void OnPointerDown (PointerEventData p) { pressed  = true;  Apply(); }
    public void OnPointerUp   (PointerEventData p) { pressed  = false; Apply(); }

    void Apply()
    {
        foreach (var e in labels)
        {
            if (e == null || e.text == null) continue;
            Vector2 off = (pressed && e.moveOnPress) ? pressOffset : Vector2.zero;
            e.text.rectTransform.anchoredPosition = e.home + off;
        }

        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(Routine(hovering));
    }

    System.Collections.IEnumerator Routine(bool toHover)
    {
        Color[] from = new Color[labels.Length];
        for (int i = 0; i < labels.Length; i++)
            from[i] = (labels[i] != null && labels[i].text != null)
                ? labels[i].text.color : Color.white;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            SetLabels(from, toHover, t / fadeDuration);
            yield return null;
        }
        SetLabels(from, toHover, 1f);
    }

    void SetLabels(Color[] from, bool toHover, float k)
    {
        for (int i = 0; i < labels.Length; i++)
        {
            var e = labels[i];
            if (e == null || e.text == null) continue;
            e.text.color = Color.Lerp(from[i], toHover ? e.hover : e.normal, k);
        }
    }
}