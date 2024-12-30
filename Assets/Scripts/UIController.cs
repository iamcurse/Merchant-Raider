using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    
    private PlayerController _playerController;
    private GameObject _pauseUI;
    private GameObject _gameUI;
    private GameObject _inventoryUI;
    private GameObject _gameOverUI;
    private Transform _healthParent;
    private TMPro.TextMeshProUGUI _moneyText;
    [SerializeField] private GameObject healthPrefab;
    
    private bool _isDialogActive;
    
    private PlayerInfo _playerInfoBackup;
    private Inventory _inventoryBackup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogError("Multiple UIController instances found! Destroying duplicate.");
            Destroy(gameObject);
        }
        
        _gameUI = gameObject.transform.GetChild(0).gameObject;
        _inventoryUI = gameObject.transform.GetChild(1).gameObject;
        _pauseUI = gameObject.transform.GetChild(2).gameObject;
        _gameOverUI = gameObject.transform.GetChild(3).gameObject;
        _healthParent = _gameUI.transform.GetChild(0).transform.GetChild(2).transform;
        _moneyText = _gameUI.transform.GetChild(0).transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
    }
    
    private void Start()
    {
        _playerController = PlayerController.Instance;
        BackupPlayerInfo();
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
        SceneManager.LoadScene("StartScreen");
    }
    
    private void DisableMenuButtons()
    {
        _gameUI.transform.GetChild(0).gameObject.SetActive(false);
        _gameUI.transform.GetChild(1).gameObject.SetActive(false);
        _gameUI.transform.GetChild(2).gameObject.SetActive(false);
    }
    
    private void EnableMenuButtons()
    {
        _gameUI.transform.GetChild(0).gameObject.SetActive(true);
        _gameUI.transform.GetChild(1).gameObject.SetActive(true);
        _gameUI.transform.GetChild(2).gameObject.SetActive(true);
        
    }
    
    public void InventoryControl()
    {
        _inventoryUI.SetActive(!_inventoryUI.activeSelf);
    }

    private void UpdateHealthMoney(PlayerInfo playerInfo)
    {
        UpdateHealth(playerInfo.Health);
        UpdateMoney(playerInfo.Money);
    }

    public void UpdateHealth(int health)
    {
        foreach (Transform child in _healthParent)
        {
            Destroy(child.gameObject);
        }

        for (var i = 0; i < health; i++)
        {
            Instantiate(healthPrefab, _healthParent);
        }
    }

    public void UpdateMoney(int money)
    {
        _moneyText.text = money.ToString();
    }
    
    public bool IsInventoryActive()
    {
        return _inventoryUI.activeSelf;
    }
    
    public void GameOver()
    {
        Time.timeScale = 0f;
        _gameOverUI.SetActive(true);
    }
    
    public void Restart()
    {
        RestorePlayerInfo();
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void BackupPlayerInfo()
    {
        _playerInfoBackup = _playerController.playerInfo.Clone();
        _inventoryBackup = _playerController.inventory.Clone();
    }
    
    private void RestorePlayerInfo()
    {
        if (_playerInfoBackup == null || _inventoryBackup == null)
        {
            Debug.LogError("PlayerInfoBackup or InventoryBackup is null!");
            return;
        }
        
        _playerController.inventory.items = new List<ItemData>(_inventoryBackup.items);
        _playerController.playerInfo.SetMaxHealth(_playerInfoBackup.MaxHealth);
        
        UpdateHealthMoney(_playerController.playerInfo);
    }
}



