using UnityEngine;

public class StoryStarter : MonoBehaviour
{
    [SerializeField] private DialogueRunner runner;
    [SerializeField] private string storyId = "Prologue";
    [SerializeField] private int startFromLine = 1;

    void Start() => runner.Play(storyId, Mathf.Max(0, startFromLine - 1));
}