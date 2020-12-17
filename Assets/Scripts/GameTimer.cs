using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    private Text textTimer;
    public float inGameTime = 60 * 3;

    private void Awake()
    {
        textTimer = GetComponent<Text>();
    }

    private void Update()
    {
        if (inGameTime > 0)
        {
            inGameTime -= Time.deltaTime;

            float minutes = Mathf.FloorToInt(inGameTime / 60);
            float seconds = Mathf.FloorToInt(inGameTime % 60);

            textTimer.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
        else
        {

        }
    }
}