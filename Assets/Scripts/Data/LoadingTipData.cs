using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "LoadingTips", menuName = "Tips/Loading Tips")]
public class LoadingTipData : ScriptableObject
{
    [System.Serializable]
    public class Tip
    {
        [TextArea(2, 4)] public string text;
    }
    
    [SerializeField] private Tip[] tips;

    static int lastIndex = -1;

    public string GetRandom(string targetScene)
    {
        if (tips == null || tips.Length == 0) return string.Empty;

        var pool = new List<int>();
        for (int i = 0; i < tips.Length; i++)
        {
            if (string.IsNullOrEmpty(tips[i].text)) continue;
            pool.Add(i);
        }

        if (pool.Count == 0) return string.Empty;
        if (pool.Count > 1) pool.Remove(lastIndex);

        lastIndex = pool[Random.Range(0, pool.Count)];
        return tips[lastIndex].text;
    }
}
