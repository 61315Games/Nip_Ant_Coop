using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageLoader : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        public string chapterId;
        public GameObject envPrefab;
    }

    [SerializeField] private Entry[] chapters;
    private void Awake()
    {
        string id = GameFlow.CurrentStage;
        var e = chapters.FirstOrDefault(x => x.chapterId == id);
        if (e == null || e.envPrefab == null)
        {
            Debug.LogWarning($"'{id}' 스테이지 환경을 찾을 수 없음");
            return;
        }
        Instantiate(e.envPrefab);
    }
}
