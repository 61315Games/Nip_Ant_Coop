using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BgmPlaylist", menuName = "Audio/BGM Playlist")]
public class BgmPlaylist : ScriptableObject
{
    [SerializeField] private AudioClip[] clips;

    [Range(0f, 1f)] public float volume = 1f;

    public bool avoidRepeatOnRefill = true;
    public float gapBetweenTracks = 0.5f;
    public AudioClip[] Clips => clips;
}
