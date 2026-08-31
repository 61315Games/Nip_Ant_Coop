using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AntMonologueData", menuName = "Termite/Ant Monologue Data")]
public class AntMonologueData : ScriptableObject
{
    [TextArea] public List<string> lines = new List<string>();
}
