using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string startStage = "Prologue_1";

    private void Start()
    {
        Time.timeScale = 1f;
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
