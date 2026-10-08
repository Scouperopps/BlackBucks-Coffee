using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "EscenaDiego";

    [Header("Loading Indicator")]
    [SerializeField] private Transform loadingImage;
    [SerializeField] private float rotationSpeed = 180f;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private void Update()
    {
        if (loadingImage != null)
        {
            loadingImage.Rotate(
                0f,
                0f,
                -rotationSpeed * Time.deltaTime
            );
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}