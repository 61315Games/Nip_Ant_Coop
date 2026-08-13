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
    
    [Header("Shake")]
    [SerializeField] private RectTransform shakeRoot;      // 흔들 대상(화면 전체 담은 루트)
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 15f;

    [Header("Fade")]
    [SerializeField] private Fader fader;
    [SerializeField] private CanvasGroup characterGroup;       
    [SerializeField] private float characterFadeDuration = 0.5f;
    [SerializeField] private float actorFadeDuration = 0.5f;
    private Dictionary<string, Coroutine> fades = new Dictionary<string, Coroutine>();
    
    private Vector2 shakeHome;
    private Coroutine shakeCo;

    private DialogueData data;
    private DialogueNode current;
    private Coroutine typing;
    private bool isTyping;

    private bool ending;
    public string Summary => data != null ? data.summary : null;

    private int selectedIndex;

    private Dictionary<string, Image> stage = new Dictionary<string, Image>();
    private Dictionary<string, Coroutine> movers = new Dictionary<string, Coroutine>();

    public bool IsActive => current != null;

    void Awake()
    {
        if (shakeRoot != null) shakeHome = shakeRoot.anchoredPosition;
    }
    // -------------------- 재생 --------------------
    public void Play(string storyId, int startIndex = 0)
    {
        string path = Path.Combine(Application.streamingAssetsPath, storyId + ".json");
        data = JsonConvert.DeserializeObject<DialogueData>(File.ReadAllText(path));
        ending = false;
        if (data == null) return;

        if(!string.IsNullOrEmpty(data.chapterLabel)) GameFlow.ChapterLabel = data.chapterLabel;
        if (data.day > 0) GameFlow.Day = data.day;
        ChapterHeader.instance?.Refresh();
        SkipController.instance?.Refresh();
        ClearStage();

        if (!string.IsNullOrEmpty(data.background) && spriteDB != null && backgroundImage != null)
        {
            Sprite bg = spriteDB.Get(data.background);
            if (bg != null) backgroundImage.sprite = bg;
        }

        startIndex = Mathf.Clamp(startIndex, 0, data.nodes.Count - 1);
        StartCoroutine(IntroRoutine(startIndex));
    }

    void Show(DialogueNode node)
    {
        if (portraitImage != null)
        {
            if (!string.IsNullOrEmpty(node.portrait) && spriteDB != null)
            {
                Sprite p = spriteDB.Get(node.portrait);
                if (p != null)
                {
                    portraitImage.sprite = p;
                    float b = node.portraitBrightness;
                    portraitImage.color = new Color(b, b, b, 1f);
                    portraitImage.gameObject.SetActive(true);
                }
            }
            else portraitImage.gameObject.SetActive(false);
        }

        ApplyActors(node);

        speakerText.text = node.speaker;
        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(TypeText(node.text));

        if (node.shake) Shake();
    }

    IEnumerator IntroRoutine(int startIndex)
    {
        panel.SetActive(false);
        if (characterRoot != null) characterRoot.gameObject.SetActive(false);
        if (characterGroup != null) characterGroup.alpha = 0f;

        if (fader != null)
            yield return fader.FadeIn();

        if (characterRoot != null) characterRoot.gameObject.SetActive(true);
        panel.SetActive(true);

        for (int k = 0; k < startIndex; k++)
            ApplyActors(data.nodes[k]);

        current = data.nodes[startIndex];
        Show(current);

        if (characterGroup != null)
            yield return FadeCanvas(characterGroup, 0f, 1f, characterFadeDuration);
    }

    IEnumerator OutroRoutine()
    {
        if (fader != null)
            yield return fader.FadeOut();
        
        panel.SetActive(false);
        if(characterRoot != null)
            characterRoot.gameObject.SetActive(false);

        if(data == null) yield break;

        if (!string.IsNullOrEmpty(data.nextScene))
            SceneRouter.Load(data.nextScene, data.nextStage);
        else if (!string.IsNullOrEmpty(data.nextStage))
            GameFlow.CurrentStage = data.nextStage;
    }
    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator TypeText(string full)
    {
        isTyping = true;

        dialogueText.maxVisibleCharacters = 0;
        dialogueText.text = full;
        dialogueText.ForceMeshUpdate();
        int total = dialogueText.textInfo.characterCount;
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

    public void Skip()
    {
        if (ending || data == null) return;
        if (typing != null)
        {
            StopCoroutine(typing);
            typing = null;
        }
        isTyping = false;

        if (choosing)
        {
            choosing = false;
            if (choicePopup != null) choicePopup.Close();
        }
        EndDialogue();
    }

    void EndDialogue()
    {
        if (ending) return;
        ending = true;
        current = null;
        StartCoroutine(OutroRoutine());
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
            
            img.rectTransform.localScale = new Vector3(a.flip ? -1f : 1f, 1f, 1f);
            
            if (a.fadeIn)
                StartActorFade(a.id, img, a.brightness);                        // 서서히 등장
            else
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
    
    void StartActorFade(string id, Image img, float brightness)
    {
        if (fades.TryGetValue(id, out var c) && c != null) StopCoroutine(c);
        fades[id] = StartCoroutine(ActorFadeRoutine(img, brightness));
    }
    
    IEnumerator ActorFadeRoutine(Image img, float brightness)
    {
        float t = 0f;
        while (t < actorFadeDuration)
        {
            if (img == null) yield break;
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / actorFadeDuration);
            img.color = new Color(brightness, brightness, brightness, alpha);
            yield return null;
        }
        if (img == null) yield break;
        img.color = new Color(brightness, brightness, brightness, 1f);
    }
    void RemoveActor(string id)
    {
        if (fades.TryGetValue(id, out var c) && c != null) StopCoroutine(c);
        fades.Remove(id);

        if (stage.TryGetValue(id, out var img) && img != null) Destroy(img.gameObject);
        stage.Remove(id);
        movers.Remove(id);
    }

    void ClearStage()
    {
        foreach (var kv in fades) if (kv.Value != null) StopCoroutine(kv.Value);
        fades.Clear();

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