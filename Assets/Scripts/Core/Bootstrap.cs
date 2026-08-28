using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string firstScene = "MainMenu";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(firstScene);
    }
}
