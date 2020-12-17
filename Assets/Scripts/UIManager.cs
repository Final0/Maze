using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private GameTimer gameTimer;
    private PlayerController playerController;
    public GameObject loseHUD, victoryHUD;

    private void Awake()
    {
        gameTimer = FindObjectOfType<GameTimer>();
    }

    private void Update()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        DetectEndGame();
        DetectVictory();
    }

    private void DetectEndGame()
    {
        if(gameTimer.inGameTime <= 1)
        {
            gameTimer.gameHUD.SetActive(false);
            loseHUD.SetActive(true);
        }
    }

    private void DetectVictory()
    {
        if (playerController.treasureLeft == 0)
        {
            gameTimer.gameHUD.SetActive(false);
            victoryHUD.SetActive(true);
        }
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(0);
    }
}
