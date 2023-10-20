using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    private static string s_nextSceneName;
    [SerializeField] private Image _progressbarImage;

    public static void Load(string nextSceneName)
    {
        s_nextSceneName = nextSceneName;
        SceneManager.LoadScene(StringStatic.SceneLoading);
    }

    void Start()
    {
        StartCoroutine(ProcessLoading());
    }

    private IEnumerator ProcessLoading()
    {
        Debug.Log("ProcessLoading LoadingSceneController");
        AsyncOperation op = SceneManager.LoadSceneAsync(s_nextSceneName);
        op.allowSceneActivation = false;

        float guaranteedTimeAtLast = 1f;
        float rateToAllowSceneActivation = 0.9f;
        float stackingTime = 0f;

        while(!op.isDone)
        {
            yield return null;

            if(op.progress < rateToAllowSceneActivation)
            {
                _progressbarImage.fillAmount = op.progress;
            }
            else
            {
                stackingTime += Time.unscaledDeltaTime;
                _progressbarImage.fillAmount = Mathf.Lerp(rateToAllowSceneActivation, 1f, stackingTime / guaranteedTimeAtLast);
                if(stackingTime >= guaranteedTimeAtLast)
                {
                    Debug.Log("Progress End LoadingSceneController");
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
