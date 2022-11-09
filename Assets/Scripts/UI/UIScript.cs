using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Text scoreText;
    private int score = 0;

    public static string GameChallenge = "Normal";

    void Awake()
    {
        EventManager.OnAddScore.AddListener(AddScore);
        EventManager.OnGameOver.AddListener(GameOver);
    }

    private void AddScore(int number)
    {
        score += number;
        scoreText.text = "—чет: " + score.ToString();
    }

    private void GameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
        Debug.Log("GameOver");
    }

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    public void OnNormalGameButtonClick()
    {
        GameChallenge = "Normal";
        SceneManager.LoadScene("SimpleScene");
    }
    public void OnHardGameButtonClick()
    {
        GameChallenge = "Hard";
        SceneManager.LoadScene("HardScene");
    }

    public void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void OnSettingsButtonClick()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0;
        EventManager.SendGamePaused();
    }

    public void OnCloseSettingsButtonClick()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1;
        EventManager.SendGamePaused();
    }
}
