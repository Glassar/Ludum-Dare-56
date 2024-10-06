using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TooltipHandler : MonoBehaviour
{
    public static TooltipHandler instance;
    [SerializeField] public TMP_Text text1;
    [SerializeField] public TMP_Text text2;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void UpdateTooltip(string text)
    {
        text1.text = text;
        text2.text = text;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
