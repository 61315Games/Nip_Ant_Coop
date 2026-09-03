using UnityEngine;

public class Termite : MonoBehaviour
{
    private TermiteSpawnInfo info;
    private AntMonologue monologue;
    
    void Awake() => monologue = GetComponent<AntMonologue>();

    public void Init(TermiteSpawnInfo i) => info = i;

    public void Judge()
    {
        if (info != null && info.isReal)
        {
            InteractionPopup.instance?.ShowTermiteFound();
            AntCounter.instance?.AddFound();
            TutorialController.instance?.NotifyReportAnt();
            Destroy(gameObject);
        }
            
        else
            InteractionPopup.instance?.ShowWrongTarget(monologue);
    }
}