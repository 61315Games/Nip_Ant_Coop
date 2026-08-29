using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SfxLibrary", menuName = "Audio/SFX Library")]
public class SfxLibrary : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string id;
        public AudioClip[] clips;
        [Range(0f, 1f)] public float volume = 1f;
        public Vector2 pitchRange = new Vector2(1f, 1f);
    }

    [SerializeField] private Entry[] entries;

    public Entry Find(string id)
    {
        if (entries == null) return null;
        for(int i = 0; i < entries.Length; i++)
            if (entries[i] != null && entries[i].id == id)
                return entries[i];
        return null;
    }
}