using PixelCrushers.DialogueSystem;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    
    private PlayerController _playerController;
    private GameObject _pauseUI;
    private GameObject _gameUI;
    private GameObject _inventoryUI;
    private Transform _healthParent;
    private TMPro.TextMeshProUGUI _moneyText;
    [SerializeField] private GameObject healthPrefab;

    
    private bool _isDialogActive;

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
        _healthParent = _gameUI.transform.GetChild(0).transform.GetChild(2).transform;
        _moneyText = _gameUI.transform.GetChild(0).transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
    }
    
    private void Start()
    {
        _playerController = PlayerController.Instance;
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

    public void UpdateHealthUI(int health)
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

    public void UpdateMoneyUI(int money)
    {
        _moneyText.text = money.ToString();
    }
    
    public bool IsInventoryActive()
    {
        return _inventoryUI.activeSelf;
    }
}



