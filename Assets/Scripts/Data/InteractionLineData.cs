using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InteractionLineData", menuName = "Termite/Interaction Line Data")]
public class InteractionLineData : ScriptableObject
{
    [Header("흰 개미를 찾았을 때")]
    [TextArea] public List<string> correctLines = new List<string>();
    
    [Header("흰 개미가 아닌 대상을 클릭했을 때")]
    [TextArea] public List<string> wrongLines = new List<string>();

    [Header("잘못 신고한 검은 개미 대사")]
    [TextArea] public List<string> wrongBubbleLines = new List<string>();

    [Header("표정")]
    public Sprite correctFace;
    public Sprite wrongFace;
}