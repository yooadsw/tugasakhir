using UnityEngine;
using UnityEngine.SceneManagement; // Penting untuk mengelola scene

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "GameScene";

    public void StartGame()
    {
        Debug.Log("Mulai Game...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Keluar Game...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}