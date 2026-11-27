using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    [SerializeField] private float splashDuration = 3f;
    [SerializeField] private string nextScene = "StartScreen";

    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(splashDuration);

        SceneManager.LoadScene(nextScene);
    }
}