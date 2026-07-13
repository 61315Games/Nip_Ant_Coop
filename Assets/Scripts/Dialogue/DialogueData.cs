using System.Collections.Generic;


[System.Serializable]
public class DialogueData
{
    public string storyId;
    public string startNodeId;
    public List<DialogueNode> nodes = new List<DialogueNode>();
}

[System.Serializable]
public class DialogueNode
{
    public string id;
    public string speaker;
    public string text;
    public string portrait;
    public string next;
    public List<Choice> choices = new List<Choice>();
    public bool endHere;
}

[System.Serializable]
public class Choice
{
    public string text;
    public string next;
}