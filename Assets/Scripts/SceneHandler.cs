using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private string sceneToLoad;

    private void Start()
    {
        Time.timeScale = 1;
    }

    public void SceneChanger(string sceneName)
    {
        sceneToLoad = sceneName;
        StartCoroutine(AsyncSceneLoad());
    }

    IEnumerator AsyncSceneLoad()
    {
        animator.SetTrigger("Start");

        yield return new WaitForSecondsRealtime(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!asyncLoad.isDone)
        {
            yield return null; 
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
