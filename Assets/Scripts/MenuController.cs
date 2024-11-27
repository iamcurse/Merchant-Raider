using PixelCrushers.DialogueSystem;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private PlayerController _playerController;
    private GameObject _pauseUI;
    private GameObject _menuUI;
    private GameObject _inventoryUI;
    private bool _isDialogActive;

    private void Awake()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _menuUI = gameObject.transform.GetChild(0).gameObject;
        _inventoryUI = gameObject.transform.GetChild(1).gameObject;
        _pauseUI = gameObject.transform.GetChild(2).gameObject;
    }

    private void Update()
    {
        _isDialogActive = DialogueManager.isConversationActive;
        
        if (_isDialogActive || _pauseUI.activeSelf)
        {
            DisableMenuButtons();
            if (_inventoryUI.activeSelf)
            {
                _inventoryUI.SetActive(false);
            }
        } else
        {
            EnableMenuButtons();
        }
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
    
    private void DisableMenuButtons()
    {
        _menuUI.transform.GetChild(0).gameObject.SetActive(false);
        _menuUI.transform.GetChild(1).gameObject.SetActive(false);
        _menuUI.transform.GetChild(2).gameObject.SetActive(false);
    }
    
    private void EnableMenuButtons()
    {
        _menuUI.transform.GetChild(0).gameObject.SetActive(true);
        _menuUI.transform.GetChild(1).gameObject.SetActive(true);
        _menuUI.transform.GetChild(2).gameObject.SetActive(true);
        
    }
    
    public void InventoryControl()
    {
        _inventoryUI.SetActive(!_inventoryUI.activeSelf);
    }
}

