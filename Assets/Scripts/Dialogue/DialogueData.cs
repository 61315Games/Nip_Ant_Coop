using System.Collections.Generic;


[System.Serializable]
public class DialogueData
{
    public string storyId;
    public string startNodeId;
    public string background;
    public List<DialogueNode> nodes = new List<DialogueNode>();
}

[System.Serializable]
public class DialogueNode
{
    public string id;
    public string speaker;
    public string text;
    public string portrait;
    public float portraitBrightness = 1f;
    public string next;
    public bool endHere;
    public List<Choice> choices = new List<Choice>();
    public List<ActorState> actors = new List<ActorState>();
    public bool shake;
}

[System.Serializable]
public class Choice
{
    public string text;
    public string next;
}

[System.Serializable]
public class ActorState
{
    public string id;
    public string sprite;
    public float brightness = 1f;
    public string slot;
    public bool fadeIn;
    public bool flip;
}