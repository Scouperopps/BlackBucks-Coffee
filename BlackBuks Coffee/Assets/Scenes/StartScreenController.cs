using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] private string nextScene = "MainMenu";

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}