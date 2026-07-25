using UnityEngine;

public class StoryStarter : MonoBehaviour
{
    [SerializeField] private DialogueRunner runner;
    [SerializeField] private string overrideStoryId = ""; 
    [SerializeField] private int startFromLine = 1;

    void Start()
    {
        string id = string.IsNullOrEmpty(overrideStoryId) ? GameFlow.CurrentStage : overrideStoryId;
        runner.Play(id, Mathf.Max(0, startFromLine - 1));
    }
}