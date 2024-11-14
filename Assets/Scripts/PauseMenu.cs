using PixelCrushers.DialogueSystem;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private GameObject _pauseUI;
    private GameObject _pauseButton;
    private PlayerController _playerController;

    private void Awake()
    {
        _pauseUI = gameObject.transform.GetChild(1).gameObject;
        _playerController = FindAnyObjectByType<PlayerController>();
        _pauseButton = gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
    }

    private void Update()
    {
        _pauseButton.SetActive(!DialogueManager.isConversationActive);
    }

    public void PauseScript()
    {
        if (_pauseUI.activeSelf)
        {
            Resume();
        } else
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (DialogueManager.isConversationActive) return;
        _playerController.isPause = true;
        _pauseUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        _playerController.isPause = false;
        _pauseUI. SetActive(false);
        Time.timeScale = 1f;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start_Screen");
    }
}

