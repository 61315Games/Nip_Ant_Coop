using UnityEngine;

public class TermiteSpawner : MonoBehaviour
{
    [SerializeField] private Transform cubeTransform;
    [SerializeField] private GameObject termitePrefab;
    [SerializeField] private StageTermiteData currentStage;
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
        foreach (var info in stage.termites)
        {
            GameObject t = Instantiate(termitePrefab, cubeTransform);
            t.transform.localPosition = info.localPosition;
            t.transform.localScale = Vector3.one * info.scale;
            
            var termite = t.GetComponent<Termite>();
            if(termite != null) termite.Init(info);

            if (info.isReal) realCount++;
        }

        AntCounter.instance?.SetTotal(realCount);
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
