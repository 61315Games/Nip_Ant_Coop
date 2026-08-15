using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

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
    [SerializeField] private float moveDuration = 0.08f;
    [SerializeField] private float fadeDuration = 0.12f;

    private bool hovering, pressed;

    void Awake()
    {
        foreach (var e in labels)
        {
            if (e == null || e.text == null) continue;
            e.home = e.text.rectTransform.anchoredPosition;
            e.text.color = e.normal;
        }
    }

    void OnDisable() { KillAll(); }

    void KillAll()
    {
        foreach (var e in labels)
        {
            if (e == null || e.text == null) continue;
            e.text.rectTransform.DOKill();
            e.text.DOKill();
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

            e.text.rectTransform.DOKill();
            e.text.rectTransform
                  .DOAnchorPos(e.home + off, moveDuration, true)
                  .SetUpdate(true)
                  .SetLink(e.text.gameObject);

            e.text.DOKill();
            e.text.DOColor(hovering ? e.hover : e.normal, fadeDuration)
                  .SetUpdate(true)
                  .SetLink(e.text.gameObject);
        }
    }
}