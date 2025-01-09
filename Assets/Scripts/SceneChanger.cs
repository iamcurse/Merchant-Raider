using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ReturnHome()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void ChangeScene(int indexNumber)
    {
        SceneManager.LoadScene(indexNumber);
    }
}


