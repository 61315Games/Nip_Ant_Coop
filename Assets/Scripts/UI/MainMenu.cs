using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string startStage = "Prologue_1";
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private string progressFormat = "진행중인 챕터:{0}";

    private void Start()
    {
        Time.timeScale = 1f;

        if (progressText != null)
            progressText.text = string.Format(progressFormat, GameFlow.ChapterLabel);
    }

    public void OnClickPlay()
    {
        SceneRouter.Load(SceneRouter.StoryScene, startStage);
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
