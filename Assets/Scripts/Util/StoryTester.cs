using UnityEngine;

public class StoryStarter : MonoBehaviour
{
    [SerializeField] private DialogueRunner runner;
    [SerializeField] private string storyId = "Prologue";

    void Start() => runner.Play(storyId);
}