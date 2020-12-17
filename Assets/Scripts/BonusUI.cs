using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusUI : MonoBehaviour
{
    private PlayerController playerController;

    public Image bonusVisionIcon, bonusSpeedIcon;

    private void Start()
    {
        bonusVisionIcon.fillAmount = 0;
        bonusSpeedIcon.fillAmount = 0;
    }

    private void Update()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        SpeedIcon();
        VisionIcon();
    }

    void SpeedIcon()
    {
        if (playerController.isSpeedBonus)
        {
            bonusSpeedIcon.fillAmount -= 1 / 10f * Time.deltaTime;

            if (bonusSpeedIcon.fillAmount <= 0)
                bonusSpeedIcon.fillAmount = 0;
        }
    }

    void VisionIcon()
    {
        if (playerController.isVisionBonus)
        {
            bonusVisionIcon.fillAmount -= 1 / 10f * Time.deltaTime;

            if (bonusVisionIcon.fillAmount <= 0)
                bonusVisionIcon.fillAmount = 0;
        }
    }
}
