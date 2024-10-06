using Rellac.Audio;
using UnityEngine;

using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private SoundManager soundManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundManager.PlayLoopingAudio("Ambient1", transform);
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
        //SceneManager.UnloadSceneAsync(0);
    }

    public void Quit()
    {
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
}
