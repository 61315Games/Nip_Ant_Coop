using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFlow : MonoBehaviour
{
    public static StageFlow instance;

    [System.Serializable]
    public class Route
    {
        public string stageId;
        public string clearStoryId;
        public string failStoryId;
    }

    [SerializeField] private Fader fader;
    [SerializeField] private Route[] routes;
    private bool ended;

    void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if(instance == this) instance = null;
    }

    public void Clear() => Go(true);
    public void Fail() => Go(false);

    void Go(bool cleared)
    {
        if (ended) return;
        ended = true;

        var r = System.Array.Find(routes, x => x.stageId == GameFlow.CurrentStage);
        if (r == null)
        {
            ended = false;
            return;
        }
        
        string next = cleared ? r.clearStoryId : r.failStoryId;
        if (string.IsNullOrEmpty(next)) return;

        StartCoroutine(Routine(next));
    }

    IEnumerator Routine(string storyId)
    {
        if (fader != null) yield return fader.FadeOut();
        SceneRouter.Load(SceneRouter.StoryScene, storyId);
    }
}
