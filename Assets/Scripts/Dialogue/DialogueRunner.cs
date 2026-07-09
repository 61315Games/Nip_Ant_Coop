using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    [SerializeField] private PanelPopup choicePopup;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float typeSpeed = 0.08f;

    [Header("Choice UI")]
    [SerializeField] private GameObject[] choiceSlots;
    [SerializeField] private TMP_Text[] choiceLabels;
    [SerializeField] private GameObject[] choiceChecks;
    [SerializeField] private GameObject[] choiceDirs;
    
    private DialogueData data;
    private DialogueNode current;
    private Coroutine typing;
    private bool isTyping;
    private int selectedIndex;
    public bool choosing;

    public bool IsActive => current != null;

    public void Play(string storyId)
    {
        string path = Path.Combine(Application.streamingAssetsPath, storyId + ".json");
        data = JsonConvert.DeserializeObject<DialogueData>(File.ReadAllText(path));

        panel.SetActive(true);
        current = data.nodes.First(n => n.id == data.startNodeId);
        Show(current);
    }

    void Show(DialogueNode node)
    {
        speakerText.text = node.speaker;
        if(typing != null) StopCoroutine(typing);
        typing = StartCoroutine(TypeText(node.text));
    }

    IEnumerator TypeText(string full)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in full)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        if(current.choices != null && current.choices.Count > 0)
            ShowChoices();
    }

    public void OnClick()
    {
        if (isTyping)
        {
            if(typing != null) StopCoroutine(typing);
            dialogueText.text = current.text;
            isTyping = false;
            if(current.choices != null && current.choices.Count > 0)
                ShowChoices();
            return;
        }

        if (current.choices != null && current.choices.Count > 0) return;
        Next();
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

    void ShowChoices()
    {
        choosing = true;
        selectedIndex = 0;

        for (int i = 0; i < choiceSlots.Length; i++)
        {
            bool used = i < current.choices.Count;
            choiceSlots[i].SetActive(used);
            if (used) choiceLabels[i].text = current.choices[i].text;
        }
        UpdateChecks();
        choicePopup.Open();
    }

    void UpdateChecks()
    {
        for (int i = 0; i < choiceChecks.Length; i++)
        {
            choiceChecks[i].SetActive(i == selectedIndex);
            choiceDirs[i].SetActive(i == selectedIndex);
        }
    }

    public void Move(int dir)
    {
        int count = current.choices.Count;
        selectedIndex = (selectedIndex + dir + count) % count;
        UpdateChecks();
    }

    public void Confirm()
    {
        choosing = false;
        choicePopup.Close();
        Choose(current.choices[selectedIndex].next);
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
        panel.SetActive(false);
        // 다음 게임 흐름으로 연결
    }
}
