using PixelCrushers.DialogueSystem;
using UnityEngine.InputSystem;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private GameObject _pauseUI;
    private GameObject _menuUI;
    private GameObject _inventoryUI; 
    private PlayerController _playerController;
    private bool _isDialogActive;
    private bool _isInventoryActive; 
    private void Awake()
    {
        _pauseUI = gameObject.transform.GetChild(2).gameObject;
        _menuUI = gameObject.transform.GetChild(0).gameObject;
        _inventoryUI = gameObject.transform.GetChild(1).gameObject; 
        _playerController = FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        _isDialogActive = DialogueManager.isConversationActive;
        
        if (_isDialogActive || _pauseUI.activeSelf)
        {
            DisableMenuButtons();
        }
        else
        {
            EnableMenuButtons();
        }

        if (Keyboard.current.iKey.wasPressedThisFrame && !_pauseUI.activeSelf && !_isDialogActive)
        {
            ToggleInventory();
        }
    }

    public void PauseScript()
    {
        if (_pauseUI.activeSelf)
        {
            Resume();
        }
        else
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
        _pauseUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start_Screen");
    }

    public void ToggleInventory()
    {
        _isInventoryActive = !_isInventoryActive; 
        _inventoryUI.SetActive(_isInventoryActive); 
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
}
