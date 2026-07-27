using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private TMP_Text tipText;
    [SerializeField] private LoadingTipData tipData;
    [SerializeField] private float minDisplayTime = 2.5f;
    [SerializeField] private string fallbackScene = SceneRouter.StoryScene;

    IEnumerator Start()
    {
        string target = SceneRouter.NextScene;
        if (string.IsNullOrEmpty(target))
            target = fallbackScene;

        if (tipText != null && tipData != null)
            tipText.text = tipData.GetRandom(target);
        
        float minTime = Mathf.Max(minDisplayTime, SceneRouter.MinLoadingTime);
        var op = SceneManager.LoadSceneAsync(target);
        op.allowSceneActivation = false;

        float t = 0f;
        while (op.progress < 0.9f || t < minTime)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        op.allowSceneActivation = true;
    }
}
