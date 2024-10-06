using System;
using Rellac.Audio;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject title;
    private Boolean inSettings = false;
    [SerializeField] private GameObject settings;
    [SerializeField] private SoundManager soundManager;

    [SerializeField] private Slider music;
    [SerializeField] private Slider sound;
    [SerializeField] private Slider sensitivity;

    [SerializeField] private AudioMixer mixer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            soundManager.PlayLoopingAudio("Ambient1", transform);
        }

        float tmp;
        mixer.GetFloat("MusicVolume", out tmp);
        music.value = tmp;

        mixer.GetFloat("SFXVolume", out tmp);
        sound.value = tmp;

        sensitivity.value = Settings.instance.sensitivity;
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
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

    public void SettingsPage()
    {
        settings.SetActive(!inSettings);
        title.SetActive(inSettings);
        inSettings = !inSettings;
    }

    void Update()
    {
        mixer.SetFloat("MusicVolume", music.value);
        mixer.SetFloat("SFXVolume", sound.value);
        Settings.instance.sensitivity = sensitivity.value;
    }
}
