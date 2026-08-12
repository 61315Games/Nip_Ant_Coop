using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChapterHeader : MonoBehaviour
{
    public static ChapterHeader instance;

    [SerializeField] private TMP_Text chapterText;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private string dayFormat = "개미력 {0}일";

    void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if(instance == this) instance = null;
    }

    void Start() => Refresh();

    public void Refresh()
    {
        if (chapterText != null) chapterText.text = GameFlow.ChapterLabel;
        if (dayText != null) dayText.text = string.Format(dayFormat, GameFlow.Day);
    }
}
