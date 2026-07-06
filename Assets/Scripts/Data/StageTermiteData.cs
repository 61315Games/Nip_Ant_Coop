using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TermiteSpawnInfo
{
    public Vector3 localPosition;
    public Vector3 surfaceNormal = Vector3.forward;
    public float scale = 1f;
    public bool isReal = true;
}

[CreateAssetMenu(fileName = "StageTermiteData", menuName = "Termite/Stage Data")]
public class StageTermiteData : ScriptableObject
{
    public int stageIndex;
    public List<TermiteSpawnInfo> termites = new List<TermiteSpawnInfo>();
}