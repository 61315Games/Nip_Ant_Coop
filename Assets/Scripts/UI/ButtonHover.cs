using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Refs")]
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text label;

    [Header("Label Color")]
    [SerializeField] private Color labelNormal = new Color32(0x8C, 0x8C, 0x8C, 0xFF);
    [SerializeField] private Color labelHover  = Color.white;

    [Header("Background Alpha")]
    [SerializeField] private float alphaNormal = 0f;
    [SerializeField] private float alphaHover  = 1f;
    [SerializeField] private float alphaPress  = 0.75f;

    [Header("Press")]
    [SerializeField] private Vector2 pressOffset = new Vector2(0f, -4f);
    [SerializeField] private float fadeDuration = 0.12f;

    private RectTransform labelRect;
    private Vector2 labelHome;
    private bool hovering, pressed;
    private Coroutine anim;

    void Awake()
    {
        if (background == null) background = GetComponent<Image>();
        if (label != null)
        {
            labelRect = label.rectTransform;
            labelHome = labelRect.anchoredPosition;
            label.color = labelNormal;
        }
        SetAlpha(alphaNormal);
    }

    public void OnPointerEnter(PointerEventData e) { hovering = true;  Apply(); }
    public void OnPointerExit (PointerEventData e) { hovering = false; pressed = false; Apply(); }
    public void OnPointerDown (PointerEventData e) { pressed  = true;  Apply(); }
    public void OnPointerUp   (PointerEventData e) { pressed  = false; Apply(); }

    void Apply()
    {
        float a = !hovering ? alphaNormal : (pressed ? alphaPress : alphaHover);
        Color c = hovering ? labelHover : labelNormal;

        if (labelRect != null)
            labelRect.anchoredPosition = labelHome + (pressed ? pressOffset : Vector2.zero);

        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(Routine(a, c));
    }

    System.Collections.IEnumerator Routine(float targetAlpha, Color targetLabel)
    {
        float fromA = background.color.a;
        Color fromC = label != null ? label.color : Color.white;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeDuration;
            SetAlpha(Mathf.Lerp(fromA, targetAlpha, k));
            if (label != null) label.color = Color.Lerp(fromC, targetLabel, k);
            yield return null;
        }
        SetAlpha(targetAlpha);
        if (label != null) label.color = targetLabel;
    }

    void SetAlpha(float a)
    {
        Color c = background.color;
        c.a = a;
        background.color = c;
    }
}