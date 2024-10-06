using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }

    public float sensitivity = 1;
}
