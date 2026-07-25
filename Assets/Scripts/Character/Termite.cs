using UnityEngine;

public class Termite : MonoBehaviour
{
    private TermiteSpawnInfo info;

    public void Init(TermiteSpawnInfo i) => info = i;

    public void Judge()
    {
        if (info != null && info.isReal)
        {
            AntCounter.instance?.AddFound();
            TutorialController.instance?.NotifyReportAnt();
            Destroy(gameObject);
        }
            
        else
            Debug.Log("흰개미가 아니에요!");
    }
}