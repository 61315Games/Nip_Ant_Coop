using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text timeText;

    private float totalTime = 180f;
    private float remaining;
    private bool running;
    private bool unlimited;

    void Start()
    {
        unlimited = (GameFlow.CurrentStage == "Tutorial");

        if (unlimited)
        {
            if (slider != null) slider.value = 1f;
            if (timeText != null) timeText.text = "∞";
            running = false;
            return;
        }
        remaining = totalTime;
        running = true;
        UpdateUI();
    }

    void Update()
    {
        if (!running) return;
        
        remaining -= Time.deltaTime;

        if (remaining <= 0f)
        {
            remaining = 0f;
            running = false;
            OnTimeUp();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (slider != null) slider.value = remaining / totalTime;
        int min = Mathf.FloorToInt(remaining / 60f);
        int sec = Mathf.FloorToInt(remaining % 60f);
        timeText.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    void OnTimeUp()
    {
        // TODO: 게임 종료시 처리 추가
        Debug.Log("시간 종료!");
    }
}
