using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Toggle gameModeToggle;
    [SerializeField] private Button Play, Settings, Back, Exit;
    [SerializeField] private GameObject settingsMenu;
    private void Awake()
    {
        gameModeToggle.onValueChanged.AddListener((value) =>
        {
            GameMode.gameMode = value ? GameMode.Mode.VR : GameMode.Mode.PC;
        });
        gameModeToggle.isOn = false;

        Play.onClick.AddListener(HandlePlay);
        Settings.onClick.AddListener(HandleSettings);
        Back.onClick.AddListener(HandleBack);
        Exit.onClick.AddListener(HandleExit);
    }

    private void HandleExit()
    {
        Application.Quit();
    }

    private void HandleBack()
    {
        settingsMenu.SetActive(false);
    }

    private void HandleSettings()
    {
        settingsMenu.SetActive(true);
    }

    private void HandlePlay()
    {
        SceneManager.LoadScene("GameScene");
    }
}
