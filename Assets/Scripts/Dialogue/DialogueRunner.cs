using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    private DialogueData data;
    private DialogueNode current;

    public void Play(string storyId)
    {
        string path = Path.Combine(Application.streamingAssetsPath, storyId + ".json");
        string json = File.ReadAllText(path);
        data = JsonConvert.DeserializeObject<DialogueData>(json);

        current = data.nodes.First(n => n.id == data.startNodeId);
        Show(current);
    }

    void Show(DialogueNode node)
    {
        Debug.Log($"{node.speaker}: {node.text}");   // 나중에 UI로 교체
        if(node.choices != null && node.choices.Count > 0)
            foreach(var c in node.choices)
                Debug.Log($"[선택] {c.text} -> {c.next}");
    }

    public void Next()
    {
        if (current.next == null)
        {
            EndDialogue();
            return;
        }
        current = data.nodes.First(n => n.id == current.next);
        Show(current);
    }

    public void Choose(string nextId)
    {
        if (nextId == null)
        {
            EndDialogue();
            return;
        }

        current = data.nodes.First(n => n.id == nextId);
        Show(current);
    }

    void EndDialogue()
    {
        Debug.Log("대화 종료");
        // 다음 게임 흐름으로 연결
    }
}
