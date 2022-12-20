using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject _options;
    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject _soundOn;
    [SerializeField] GameObject _soundOff;
    [SerializeField] GameObject _vibrationOn;
    [SerializeField] GameObject _vibrationOff;
    [SerializeField] AudioSource _menuBtnSound;
    [SerializeField] float soundOffset = .2f;
    private void Start()
    {
        AudioListener.volume = 0.6f;
        _soundOn.SetActive(SoundManager.SoundIsOn ? true : false);
        _soundOff.SetActive(SoundManager.SoundIsOn ? false : true);
        _vibrationOn.SetActive(SoundManager.VibrationIsOn ? true : false);
        _vibrationOff.SetActive(SoundManager.VibrationIsOn ? false : true);
    }

    private void Update()
    {
        AudioListener.pause = SoundManager.SoundIsOn ? false : true;
    }

    public void ControlSound()
    {
        if(SoundManager.SoundIsOn)
        {
            _soundOn.SetActive(false);
            _soundOff.SetActive(true);
            SoundManager.SoundIsOn = false;
            _menuBtnSound.time = soundOffset;
            _menuBtnSound.Play();
        }
        else
        {
            _soundOn.SetActive(true);
            _soundOff.SetActive(false);
            SoundManager.SoundIsOn = true;
            _menuBtnSound.time = soundOffset;
            _menuBtnSound.Play();
        }
    }

    public void ControlVibration()
    {
        if(SoundManager.VibrationIsOn)
        {
            _vibrationOn.SetActive(false);
            _vibrationOff.SetActive(true);
            SoundManager.VibrationIsOn = false;
            _menuBtnSound.time = soundOffset;
            _menuBtnSound.Play();

        }
        else
        {
            _vibrationOn.SetActive(true);
            _vibrationOff.SetActive(false);
            SoundManager.VibrationIsOn = true;
            Handheld.Vibrate();
            _menuBtnSound.time = soundOffset;
            _menuBtnSound.Play();
        }
    }

    public void StartGame()
    {
        _menuBtnSound.time = soundOffset;
        _menuBtnSound.Play();
        SceneManager.LoadScene("SampleScene");
    }
    public void Options()
    {
        _menuBtnSound.time = soundOffset;
        _menuBtnSound.Play();
        _options.SetActive(true);
        _mainMenu.SetActive(false);
    }

    public void Back()
    {
        _menuBtnSound.time = soundOffset;
        _menuBtnSound.Play();
        _mainMenu.SetActive(true);
        _options.SetActive(false);
    }

    public void Quit()
    {
        _menuBtnSound.time = soundOffset;
        _menuBtnSound.Play();
        print("Simulating Quit on PC");
        Application.Quit();
    }
}
