using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public class DialogueEditorWindow : EditorWindow
{
    private DialogueData data = new DialogueData();
    private string filePath;
    private Vector2 scroll;

    private string[] files = new string[0];
    private Vector2 listScroll;

    private string DialogueFolder => Application.streamingAssetsPath;

    static readonly string[] Slots = { "Left", "Center", "Right", "Off" };

    [MenuItem("Tools/Dialogue Editor")]
    static void Open() => GetWindow<DialogueEditorWindow>("Dialogue Editor");

    void OnEnable() => RefreshFileList();

    void RefreshFileList()
    {
        files = Directory.Exists(DialogueFolder)
            ? Directory.GetFiles(DialogueFolder, "*.json")
            : new string[0];
    }

    //------------------- GUI -------------------
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        // ===== 왼쪽: 파일 목록 =====
        EditorGUILayout.BeginVertical(GUILayout.Width(170));
        if (GUILayout.Button("새로고침")) { RefreshFileList(); GUIUtility.ExitGUI(); }
        if (GUILayout.Button("+ 새 대화")) { NewFile(); GUIUtility.ExitGUI(); }
        EditorGUILayout.Space(4);

        listScroll = EditorGUILayout.BeginScrollView(listScroll);
        foreach (string f in files)
        {
            GUI.backgroundColor = (f == filePath) ? Color.cyan : Color.white;
            if (GUILayout.Button(Path.GetFileNameWithoutExtension(f)))
            {
                LoadFile(f);
                GUIUtility.ExitGUI();
            }
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        // ===== 오른쪽: 편집 =====
        EditorGUILayout.BeginVertical();
        DrawEditor();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    void DrawEditor()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("저장")) { Save(); GUIUtility.ExitGUI(); }
        if (GUILayout.Button("삭제")) { Delete(); GUIUtility.ExitGUI(); }
        EditorGUILayout.EndHorizontal();

        if (data == null) { EditorGUILayout.LabelField("왼쪽에서 대화를 선택하세요."); return; }

        data.storyId    = EditorGUILayout.TextField("챕터 이름", data.storyId);
        data.background = EditorGUILayout.TextField("시작 배경", data.background);
        EditorGUILayout.HelpBox("맨 위 대사부터 순서대로 진행됩니다. 선택지가 있으면 목적지로 갈라집니다. 무대는 바뀌는 배우만 적으면 유지됩니다.", MessageType.None);
        EditorGUILayout.Space(4);

        // ===== 이번 프레임에 할 변경들을 '예약'만 =====
        int moveIndex = -1, moveDir = 0;
        int deleteNode = -1;
        int addChoiceNode = -1;
        int removeChoiceNode = -1, removeChoiceIdx = -1;
        int addActorNode = -1;
        int removeActorNode = -1, removeActorIdx = -1;
        bool addNode = false;

        scroll = EditorGUILayout.BeginScrollView(scroll);
        for (int i = 0; i < data.nodes.Count; i++)
        {
            var n = data.nodes[i];
            EditorGUILayout.BeginVertical("box");

            // 헤더
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"대사 {i + 1}", EditorStyles.boldLabel);
            if (GUILayout.Button("▲", GUILayout.Width(28))) { moveIndex = i; moveDir = -1; }
            if (GUILayout.Button("▼", GUILayout.Width(28))) { moveIndex = i; moveDir = 1; }
            if (GUILayout.Button("삭제", GUILayout.Width(44))) deleteNode = i;
            EditorGUILayout.EndHorizontal();

            n.speaker  = EditorGUILayout.TextField("화자", n.speaker);
            EditorGUILayout.LabelField("대사");
            n.text     = EditorGUILayout.TextArea(n.text, GUILayout.MinHeight(40));
            n.portrait = EditorGUILayout.TextField("초상화(선택)", n.portrait);

            // ----- 선택지 -----
            EditorGUILayout.Space(2);
            if (n.choices.Count == 0)
            {
                n.endHere = EditorGUILayout.Toggle("여기서 대화 종료", n.endHere);
                if (!n.endHere)
                {
                    string flow = (i + 1 < data.nodes.Count) ? "→ 다음 대사로 진행" : "→ 대화 종료(마지막)";
                    EditorGUILayout.LabelField(flow, EditorStyles.miniLabel);
                }
            }
            else
            {
                EditorGUILayout.LabelField("선택지", EditorStyles.boldLabel);
                for (int j = 0; j < n.choices.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    n.choices[j].text = EditorGUILayout.TextField(n.choices[j].text);
                    int sel = TargetToIndex(n.choices[j].next);
                    int newSel = EditorGUILayout.Popup(sel, NodeOptions(), GUILayout.Width(220));
                    n.choices[j].next = IndexToTarget(newSel);
                    if (GUILayout.Button("x", GUILayout.Width(20))) { removeChoiceNode = i; removeChoiceIdx = j; }
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (GUILayout.Button("선택지 추가")) addChoiceNode = i;

            // ----- 무대 (배우) -----
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("무대 (배우 / 그림 / 위치)", EditorStyles.boldLabel);
            for (int a = 0; a < n.actors.Count; a++)
            {
                EditorGUILayout.BeginHorizontal();
                n.actors[a].id     = EditorGUILayout.TextField(n.actors[a].id, GUILayout.Width(90));
                n.actors[a].sprite = EditorGUILayout.TextField(n.actors[a].sprite);
                int si  = SlotIndex(n.actors[a].slot);
                int nsi = EditorGUILayout.Popup(si, Slots, GUILayout.Width(70));
                n.actors[a].slot = Slots[nsi];
                if (GUILayout.Button("x", GUILayout.Width(20))) { removeActorNode = i; removeActorIdx = a; }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("배우 추가")) addActorNode = i;

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("＋ 대사 추가", GUILayout.Height(28))) addNode = true;

        // ===== 그리기가 끝난 뒤에만 실제로 변경 =====
        if (moveIndex >= 0) MoveNode(moveIndex, moveDir);
        if (deleteNode >= 0) data.nodes.RemoveAt(deleteNode);
        if (removeChoiceNode >= 0) data.nodes[removeChoiceNode].choices.RemoveAt(removeChoiceIdx);
        if (addChoiceNode >= 0) data.nodes[addChoiceNode].choices.Add(new Choice());
        if (removeActorNode >= 0) data.nodes[removeActorNode].actors.RemoveAt(removeActorIdx);
        if (addActorNode >= 0) data.nodes[addActorNode].actors.Add(new ActorState { slot = "Center" });
        if (addNode) data.nodes.Add(new DialogueNode { id = NewId() });
    }

    // ---------------------------------------------------------------- 헬퍼
    string[] NodeOptions()
    {
        var opts = new string[data.nodes.Count + 1];
        opts[0] = "(대화 종료)";
        for (int k = 0; k < data.nodes.Count; k++)
            opts[k + 1] = $"대사 {k + 1}: {data.nodes[k].speaker} \"{Preview(data.nodes[k].text)}\"";
        return opts;
    }

    int TargetToIndex(string id)
    {
        if (string.IsNullOrEmpty(id)) return 0;
        for (int k = 0; k < data.nodes.Count; k++)
            if (data.nodes[k].id == id) return k + 1;
        return 0;
    }

    string IndexToTarget(int index)
        => (index <= 0) ? null : data.nodes[index - 1].id;

    static string Preview(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        s = s.Replace("\n", " ");
        return s.Length > 14 ? s.Substring(0, 14) + "…" : s;
    }

    int SlotIndex(string s)
    {
        for (int k = 0; k < Slots.Length; k++)
            if (Slots[k] == s) return k;
        return 1; // 기본 Center
    }

    // ---------------------------------------------------------------- 동작
    void MoveNode(int i, int dir)
    {
        int j = i + dir;
        if (j < 0 || j >= data.nodes.Count) return;
        (data.nodes[i], data.nodes[j]) = (data.nodes[j], data.nodes[i]);
    }

    void NewFile()
    {
        data = new DialogueData();
        filePath = null;
        GUI.FocusControl(null);
    }

    void LoadFile(string path)
    {
        filePath = path;
        data = JsonConvert.DeserializeObject<DialogueData>(File.ReadAllText(path)) ?? new DialogueData();
        foreach (var n in data.nodes)
            if (string.IsNullOrEmpty(n.id)) n.id = NewId();
        GUI.FocusControl(null);
    }

    void Save()
    {
        if (string.IsNullOrEmpty(data.storyId))
        {
            EditorUtility.DisplayDialog("저장 불가", "챕터 이름을 먼저 입력하세요.", "확인");
            return;
        }

        AssignLinks();

        if (!Directory.Exists(DialogueFolder)) Directory.CreateDirectory(DialogueFolder);
        if (string.IsNullOrEmpty(filePath))
            filePath = Path.Combine(DialogueFolder, data.storyId + ".json");

        File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
        AssetDatabase.Refresh();
        RefreshFileList();
    }

    void Delete()
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            EditorUtility.DisplayDialog("삭제 불가", "왼쪽에서 파일을 먼저 선택하세요.", "확인");
            return;
        }
        if (!EditorUtility.DisplayDialog("파일 삭제",
            $"'{Path.GetFileName(filePath)}' 을(를) 삭제할까요?", "삭제", "취소")) return;

        File.Delete(filePath);
        if (File.Exists(filePath + ".meta")) File.Delete(filePath + ".meta");
        filePath = null;
        data = new DialogueData();
        AssetDatabase.Refresh();
        RefreshFileList();
        GUI.FocusControl(null);
    }

    // 순서 = 흐름. 저장 시 id/next/startNodeId 자동 계산
    void AssignLinks()
    {
        foreach (var n in data.nodes)
            if (string.IsNullOrEmpty(n.id)) n.id = NewId();

        data.startNodeId = data.nodes.Count > 0 ? data.nodes[0].id : null;

        for (int i = 0; i < data.nodes.Count; i++)
        {
            var n = data.nodes[i];

            if (n.choices != null)
                foreach (var c in n.choices)
                    if (TargetToIndex(c.next) == 0) c.next = null;

            if (n.choices != null && n.choices.Count > 0)
                n.next = null;
            else if (n.endHere)
                n.next = null;
            else
                n.next = (i + 1 < data.nodes.Count) ? data.nodes[i + 1].id : null;
        }
    }

    static string NewId() => "n_" + System.Guid.NewGuid().ToString("N").Substring(0, 8);
}