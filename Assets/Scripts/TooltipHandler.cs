using TMPro;
using UnityEngine;

public class TooltipHandler : MonoBehaviour
{
    public static TooltipHandler instance;
    [SerializeField] private TMP_Text text1;
    [SerializeField] private TMP_Text text2;

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
}
