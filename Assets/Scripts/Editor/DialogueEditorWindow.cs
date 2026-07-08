using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public class DialogueEditorWindow : EditorWindow
{
    private DialogueData data = new DialogueData();
    private string filePath;
    private Vector2 scroll;

    [MenuItem("Tools/Dialogue Editor")]
    static void Open() => GetWindow<DialogueEditorWindow>("Dialogue Editor");

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("열기")) Load();
        if (GUILayout.Button("저장")) Save();
        EditorGUILayout.EndHorizontal();

        data.storyId     = EditorGUILayout.TextField("Story ID", data.storyId);
        data.startNodeId = EditorGUILayout.TextField("시작 노드 ID", data.startNodeId);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        for (int i = 0; i < data.nodes.Count; i++)
        {
            var n = data.nodes[i];
            EditorGUILayout.BeginVertical("box");
            n.id      = EditorGUILayout.TextField("ID", n.id);
            n.speaker = EditorGUILayout.TextField("화자", n.speaker);
            n.text    = EditorGUILayout.TextField("대사", n.text);
            n.next    = EditorGUILayout.TextField("다음(next)", n.next);

            EditorGUILayout.LabelField("선택지");
            for (int j = 0; j < n.choices.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                n.choices[j].text = EditorGUILayout.TextField(n.choices[j].text);
                n.choices[j].next = EditorGUILayout.TextField(n.choices[j].next);
                if (GUILayout.Button("x", GUILayout.Width(20))) { n.choices.RemoveAt(j); break; }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("선택지 추가")) n.choices.Add(new Choice());
            if (GUILayout.Button("이 노드 삭제")) { data.nodes.RemoveAt(i); break; }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("노드 추가")) data.nodes.Add(new DialogueNode());
    }

    void Load()
    {
        filePath = EditorUtility.OpenFilePanel("대화 열기", Application.dataPath, "json");
        if (string.IsNullOrEmpty(filePath)) return;
        data = JsonConvert.DeserializeObject<DialogueData>(File.ReadAllText(filePath));
    }

    void Save()
    {
        if (string.IsNullOrEmpty(filePath))
            filePath = EditorUtility.SaveFilePanel("대화 저장", Application.dataPath, data.storyId, "json");
        if (string.IsNullOrEmpty(filePath)) return;
        File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
        AssetDatabase.Refresh();
    }
}