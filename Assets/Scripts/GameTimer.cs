using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Text textTimer;
    public GameObject gameHUD;

    public float inGameTime = 60 * 3;

    public bool isGameActive;

    private void Update()
    {
        if (inGameTime > 1 && gameHUD.activeSelf)
        {
            isGameActive = true;

            inGameTime -= Time.deltaTime;

            float minutes = Mathf.FloorToInt(inGameTime / 60);
            float seconds = Mathf.FloorToInt(inGameTime % 60);

            textTimer.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
        else if (inGameTime <= 1)
        {
            isGameActive = false;
        }
    }
}