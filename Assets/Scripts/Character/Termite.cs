using UnityEngine;

public class Termite : MonoBehaviour
{
    private TermiteSpawnInfo info;

    public void Init(TermiteSpawnInfo i) => info = i;

    void OnMouseDown()
    {
        if(info != null && info.isReal)
            Debug.Log("흰개미 발견!");
        else
        {
            Debug.Log("흰개미가 아니에요!");
        }
    }
}
