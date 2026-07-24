using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public static TutorialController instance;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private Image stepImage;
    [SerializeField] private List<Step> steps = new();
    private int index = -1;
    private bool waitingForAction = false;
    
    public enum Trigger { ReportAnt, Rotate, Magnify, Continue }

    [System.Serializable]
    public class Step
    {
        [TextArea] public string text;
        public Sprite image;
        public Trigger trigger;
    }

    void Awake() { instance = this; }
    void OnDestroy() { if(instance == this) instance = null; }
    public bool IsBlocking =>
        enabled && index >= 0 && index < steps.Count && !waitingForAction;
    
    void Start()
    {
        if (GameFlow.CurrentStage != "Tutorial")
        {
            if(tutorialPanel != null) tutorialPanel.SetActive(false);
            enabled = false;
            return;
        }
        Next();
    }

    void Next()
    {
        index++;
        waitingForAction = false;
        if (index >= steps.Count)
        {
            EndTutorial();
            return;
        }
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
        if (tutorialText != null) tutorialText.text = steps[index].text;
        
        ShowStepImage(steps[index].image); 
    }

    public void NotifyReportAnt() => Check(Trigger.ReportAnt);
    public void NotifyRotate() => Check(Trigger.Rotate);
    public void NotifyMagnify() => Check(Trigger.Magnify);

    void Check(Trigger t)
    {
        if (index < 0 || index >= steps.Count) return;
        if (!waitingForAction) return;
        if(steps[index].trigger == t) Next();
    }
    
    public void Advance()
    {
        if (index < 0 || index >= steps.Count) return;
        if (waitingForAction) return;

        if (steps[index].trigger == Trigger.Continue)
            Next();
        else
        {
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
            if (stepImage != null) stepImage.gameObject.SetActive(false);
            waitingForAction = true;
        }
    }
    void ShowStepImage(Sprite s)
    {
        if (stepImage == null) return;
        if (s != null)
        {
            stepImage.sprite = s;
            stepImage.rectTransform.sizeDelta = new Vector2(400, 400);
            stepImage.gameObject.SetActive(true);
        }
        else stepImage.gameObject.SetActive(false);
    }

    void EndTutorial()
    {
        tutorialText.text = "";
        if (stepImage != null) stepImage.gameObject.SetActive(false);
        var mag = FindFirstObjectByType<MagnifierController>();
        if (mag != null && mag.IsSearchMode)
            mag.ToggleSearchMode();
        
        GameFlow.CurrentStage = "Prologue_2";
        UnityEngine.SceneManagement.SceneManager.LoadScene("SC_Story");
    }
}
