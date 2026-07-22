using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteDatabase", menuName = "Dialogue/Sprite Database")]
public class SpriteDatabase : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string name;
        public Sprite sprite;
    }
    
    public List<Entry> entries = new List<Entry>();

    private Dictionary<string, Sprite> map;

    public Sprite Get(string key)
    {
        if(string.IsNullOrEmpty(key)) return null;
        if (map == null)
        {
            map = new Dictionary<string, Sprite>();
            foreach (var e in entries)
                if(!string.IsNullOrEmpty(e.name)) map[e.name] = e.sprite;
        }

        return map.TryGetValue(key, out var s) ? s : null;
    }
}
