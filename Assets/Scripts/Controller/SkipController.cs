using TMPro;
using UnityEngine;

public class SkipController : MonoBehaviour
{
    public static SkipController instance;

    [SerializeField] private DialogueRunner runner;
    [SerializeField] private PanelPopup panel;
    [SerializeField] private TMP_Text summaryText;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject overlay;
    
    public bool IsOpen { get; private set; }

    void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if(instance == this) instance = null;
    }

    private void Start()
    {
        if(panel != null) panel.gameObject.SetActive(false);
        if (overlay != null) overlay.SetActive(false);
        Refresh();
    }

    public void Refresh()
    {
        if (skipButton == null) return;
        bool hasSummary = runner != null && !string.IsNullOrWhiteSpace(runner.Summary);
        skipButton.SetActive(hasSummary);
    }

    public void OnClickSkip()
    {
        if(runner == null || IsOpen) return;

        string s = runner.Summary;
        if(summaryText != null)
            summaryText.text = runner.Summary;

        IsOpen = true;
        if (overlay != null) overlay.SetActive(true);
        if (panel != null) panel.Open();
    }

    public void OnClickCancel()
    {
        IsOpen = false;
        if (overlay != null) overlay.SetActive(false);
        if (panel != null) panel.Close();
    }

    public void OnClickConfirm()
    {
        IsOpen = false;
        if (overlay != null) overlay.SetActive(false);
        if (panel != null) panel.Close();
        if(skipButton != null) skipButton.SetActive(false);
        if (runner != null) runner.Skip();
    }
}
