using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BgmDatabase", menuName = "Audio/BGM Database")]
public class BgmDatabase : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string name;
        public BgmPlaylist playlist;
    }

    public List<Entry> entries = new List<Entry>();

    private Dictionary<string, BgmPlaylist> map;

    public BgmPlaylist Get(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        if (map == null)
        {
            map = new Dictionary<string, BgmPlaylist>();
            foreach(var e in entries)
                if (!string.IsNullOrEmpty(e.name))
                    map[e.name] = e.playlist;
        }

        return map.TryGetValue(key, out var p) ? p : null;
    }
}
