using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TermiteSpawner : MonoBehaviour
{
    [SerializeField] private Transform cubeTransform;
    [SerializeField] private GameObject termitePrefab;
    [SerializeField] private GameObject blackAntPrefab;
    [SerializeField] private StageTermiteData currentStage;
    [SerializeField] private AntMonologueData antMonologueData;
    [SerializeField] private int speakerCount = 3;
    void Start()
    {
        if (currentStage != null)
            SpawnStage(currentStage);
    }

    public void SpawnStage(StageTermiteData stage)
    {
        currentStage = stage;
        ClearExisting();

        int realCount = 0;
        var blackAnts = new List<AntMonologue>();
        foreach (var info in stage.termites)
        {
            var isBlack = info.antType == TermiteSpawnInfo.AntType.BlackAnt;
            var prefab = info.antType == TermiteSpawnInfo.AntType.BlackAnt ? blackAntPrefab : termitePrefab;
            GameObject t = Instantiate(prefab, cubeTransform);
            t.transform.localPosition = info.localPosition;
            t.transform.localScale = Vector3.one * info.scale;
            
            var termite = t.GetComponent<Termite>();
            if(termite != null) termite.Init(info);

            if (isBlack)
            {
                var m = t.GetComponent<AntMonologue>();
                if (m != null) blackAnts.Add(m);
            }

            if (info.isReal) realCount++;
        }

        AntCounter.instance?.SetTotal(realCount);
        AssignMonologues(blackAnts);
    }

    private void AssignMonologues(List<AntMonologue> ants)
    {
        if (antMonologueData == null || antMonologueData.lines.Count == 0) return;
        if (ants.Count == 0) return;

        for (int i = ants.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (ants[i], ants[j]) = (ants[j], ants[i]);
        }

        int count = Mathf.Min(speakerCount, ants.Count);
        for (int i = 0; i < count; i++)
        {
            string line = antMonologueData.lines[Random.Range(0, antMonologueData.lines.Count)];
            ants[i].Begin(line);
        }
    }
    private void ClearExisting()
    {
        for (int i = cubeTransform.childCount - 1; i >= 0; i--)
        {
            var child = cubeTransform.GetChild(i);
            if(child.GetComponent<Termite>() != null)
                Destroy(child.gameObject);
        }
    }
}
