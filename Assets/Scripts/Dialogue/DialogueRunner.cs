using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueRunner : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float typeSpeed = 0.04f;

    [Header("Choice UI")]
    [SerializeField] private PanelPopup choicePopup;
    [SerializeField] private GameObject[] choiceSlots;
    [SerializeField] private TMP_Text[] choiceLabels;
    [SerializeField] private GameObject[] choiceChecks;
    public bool choosing;

    [Header("Visuals")]
    [SerializeField] private SpriteDatabase spriteDB;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Vector2 actorSize = new Vector2(400, 400);

    [Header("Stage")]
    [SerializeField] private RectTransform characterRoot;
    [SerializeField] private RectTransform slotLeft, slotCenter, slotRight, slotOff;
    [SerializeField] private float moveSpeed = 8f;
    
    [Header("Shake")]
    [SerializeField] private RectTransform shakeRoot;      // 흔들 대상(화면 전체 담은 루트)
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 15f;

    private Vector2 shakeHome;
    private Coroutine shakeCo;

    private DialogueData data;
    private DialogueNode current;
    private Coroutine typing;
    private bool isTyping;

    private int selectedIndex;

    private Dictionary<string, Image> stage = new Dictionary<string, Image>();
    private Dictionary<string, Coroutine> movers = new Dictionary<string, Coroutine>();

    public bool IsActive => current != null;

    void Awake()
    {
        if (shakeRoot != null) shakeHome = shakeRoot.anchoredPosition;
    }
    // -------------------- 재생 --------------------
    public void Play(string storyId)
    {
        string path = Path.Combine(Application.streamingAssetsPath, storyId + ".json");
        data = JsonConvert.DeserializeObject<DialogueData>(File.ReadAllText(path));
        if (data == null) return;

        ClearStage();

        if (!string.IsNullOrEmpty(data.background) && spriteDB != null && backgroundImage != null)
        {
            Sprite bg = spriteDB.Get(data.background);
            if (bg != null) backgroundImage.sprite = bg;
        }

        panel.SetActive(true);

        current = data.nodes.FirstOrDefault(n => n.id == data.startNodeId);
        if (current == null) { Debug.LogWarning($"'{storyId}'에 시작 노드가 없습니다."); return; }
        Show(current);
    }

    void Show(DialogueNode node)
    {
        if (!string.IsNullOrEmpty(node.portrait) && spriteDB != null && portraitImage != null)
        {
            Sprite p = spriteDB.Get(node.portrait);
            if (p != null) portraitImage.sprite = p;
        }

        ApplyActors(node);

        speakerText.text = node.speaker;
        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(TypeText(node.text));

        if (node.shake) Shake();
    }

    IEnumerator TypeText(string full)
    {
        isTyping = true;
        dialogueText.text = full;
        dialogueText.ForceMeshUpdate();
        int total = dialogueText.textInfo.characterCount;

        dialogueText.maxVisibleCharacters = 0;
        for (int i = 0; i <= total; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;

        if (current.choices != null && current.choices.Count > 0)
            ShowChoices();
    }

    // ------------------------------------------------------------- 입력/진행
    public void HandleInput()
    {
        if (current == null) return;

        if (choosing)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))   Move(-1);
            if (Input.GetKeyDown(KeyCode.DownArrow)) Move(1);
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) Confirm();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) OnClick();
        }
    }

    public void OnClick()
    {
        if (current == null) return;

        if (isTyping)
        {
            if (typing != null) StopCoroutine(typing);
            dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
            isTyping = false;
            if (current.choices != null && current.choices.Count > 0) ShowChoices();
            return;
        }

        if (current.choices != null && current.choices.Count > 0) return;
        Next();
    }

    void Next()
    {
        if (current.next == null) { EndDialogue(); return; }
        current = data.nodes.FirstOrDefault(n => n.id == current.next);
        if (current == null) { EndDialogue(); return; }
        Show(current);
    }

    void Choose(string nextId)
    {
        if (string.IsNullOrEmpty(nextId)) { EndDialogue(); return; }
        current = data.nodes.FirstOrDefault(n => n.id == nextId);
        if (current == null) { EndDialogue(); return; }
        Show(current);
    }

    void EndDialogue()
    {
        panel.SetActive(false);
        current = null;
        // TODO: 챕터 클리어 처리 등 다음 흐름 연결
    }

    // ------------------------------------------------------------- 선택지
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
        if (choicePopup != null) choicePopup.Open();
    }

    void UpdateChecks()
    {
        for (int i = 0; i < choiceChecks.Length; i++)
            choiceChecks[i].SetActive(i == selectedIndex);
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
        if (choicePopup != null) choicePopup.Close();
        Choose(current.choices[selectedIndex].next);
    }

    // ------------------------------------------------------------- 무대(배우)
    Transform GetSlot(string s) => s switch
    {
        "Left"  => slotLeft,
        "Right" => slotRight,
        "Off"   => slotOff,
        _       => slotCenter,
    };

    void ApplyActors(DialogueNode node)
    {
        if (node.actors == null) return;

        foreach (var a in node.actors)
        {
            if (a.slot == "Off") { RemoveActor(a.id); continue; }

            Image img = GetOrCreateActor(a.id, out bool isNew);

            if (!string.IsNullOrEmpty(a.sprite) && spriteDB != null)
            {
                Sprite s = spriteDB.Get(a.sprite);
                if (s != null)
                {
                    img.sprite = s;
                    img.rectTransform.sizeDelta = actorSize;  
                }
                else Debug.LogWarning($"스프라이트 '{a.sprite}' 를 DB에서 못 찾음");
            }

            Transform target = GetSlot(a.slot);
            if (target != null) img.rectTransform.position = target.position;
            
            img.color = new Color(a.brightness, a.brightness, a.brightness, 1f); 
        }
    }

    Image GetOrCreateActor(string id, out bool isNew)
    {
        if (stage.TryGetValue(id, out var img) && img != null) { isNew = false; return img; }

        var go = new GameObject("Actor_" + id, typeof(Image));
        go.transform.SetParent(characterRoot, false);
        img = go.GetComponent<Image>();
        stage[id] = img;
        isNew = true;
        return img;
    }

    void MoveActor(string id, RectTransform rt, Vector3 targetPos)
    {
        if (movers.TryGetValue(id, out var c) && c != null) StopCoroutine(c);
        movers[id] = StartCoroutine(MoveRoutine(rt, targetPos));
    }

    IEnumerator MoveRoutine(RectTransform rt, Vector3 target)
    {
        while (Vector3.Distance(rt.position, target) > 0.5f)
        {
            rt.position = Vector3.Lerp(rt.position, target, Time.deltaTime * moveSpeed);
            yield return null;
        }
        rt.position = target;
    }

    void RemoveActor(string id)
    {
        if (stage.TryGetValue(id, out var img) && img != null) Destroy(img.gameObject);
        stage.Remove(id);
        movers.Remove(id);
    }

    void ClearStage()
    {
        foreach (var kv in stage) if (kv.Value != null) Destroy(kv.Value.gameObject);
        stage.Clear();
        movers.Clear();
    }
    
    void Shake()
    {
        if (shakeRoot == null) return;
        if (shakeCo != null) StopCoroutine(shakeCo);
        shakeCo = StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            float damper = 1f - (t / shakeDuration); 
            Vector2 off = Random.insideUnitCircle * shakeMagnitude * damper;
            shakeRoot.anchoredPosition = shakeHome + off;
            yield return null;
        }
        shakeRoot.anchoredPosition = shakeHome; 
    }
}