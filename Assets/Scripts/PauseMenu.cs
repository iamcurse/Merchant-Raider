using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private GameObject _pauseUI;
    private PlayerController _playerController;

    private void Awake()
    {
        _pauseUI = this.gameObject.transform.GetChild(1).gameObject;
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
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

