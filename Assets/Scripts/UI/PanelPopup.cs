using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PanelPopup : MonoBehaviour
{
    [SerializeField] private float openTime = 0.25f;
    [SerializeField] private float closeTime = 0.15f;
    [SerializeField] private float overshoot = 1.7f;
    [SerializeField] private CanvasGroup group;

    private Tween anim;

    public void Open()
    {
        anim?.Kill();
        gameObject.SetActive(true);

        transform.localScale = new Vector3(1f, 0f, 1f);
        anim = transform.DOScaleY(1f, openTime)
            .SetEase(Ease.OutBack, overshoot)
            .SetUpdate(true)
            .SetLink(gameObject);

        if (group != null)
        {
            group.alpha = 0f;
            group.DOFade(1f, openTime * 0.7f).SetUpdate(true).SetLink(gameObject);
        }
    }

    public void Close()
    {
        if (!gameObject.activeSelf) return;
        anim?.Kill();
        anim = transform.DOScaleY(0f, closeTime)
            .SetEase(Ease.InBack, overshoot)
            .SetUpdate(true)
            .SetLink(gameObject)
            .OnComplete(() => gameObject.SetActive(false));
    }
}