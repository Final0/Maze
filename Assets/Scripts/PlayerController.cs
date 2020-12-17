using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int maxRaycastDistance = 1000;

    [SerializeField]
    private LayerMask layerMaskForSolid;

    [SerializeField]
    private string groundTag;

    private GameController gameController;
    private Light gameLight;
    private GameTimer gameTimer;
    private BonusUI bonusUI;

    private float distanceToMove = 5f;

    private void Awake()
    {
        var gc = GameObject.FindWithTag("GameController");

        if (null == gc)
        {
            Debug.LogError("[PlayerController] GameController missing");
        }
        else
        {
            gameController = gc.GetComponent<GameController>();
        }

        gameLight = GetComponentInChildren<Light>();

        gameTimer = FindObjectOfType<GameTimer>();
        bonusUI = FindObjectOfType<BonusUI>();
    }

    void Update()
    {
        // Mouse click
        if (Input.GetMouseButtonDown(0) && null != gameController)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, maxRaycastDistance, layerMaskForSolid))
            {
                float distance = Distance(transform.position.x, ray.origin.x, transform.position.z, ray.origin.z);

                if (groundTag == hit.collider.gameObject.tag && distance <= distanceToMove)
                {
                    gameController.MovePlayer(hit.point);
                }
            }
        }

        if (moving)
        {
            if (transform.position != nextPointInPath)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPointInPath, moveSpeed * Time.deltaTime);
            }
            else
            {
                moving = false;
            }
        }

        if (!moving && otherPointsInPath.Count > 0)
        {
            nextPointInPath = otherPointsInPath[0];
            otherPointsInPath.RemoveAt(0);
            moving = true;
        }

        if (isSpeedBonus)
            speedTimer -= Time.deltaTime; 
        
        if (isVisionBonus)
            visionTimer -= Time.deltaTime;
    }

    [SerializeField]
    private float moveSpeed = 8;

    private bool moving = false;
    private Vector3 nextPointInPath;
    private List<Vector3> otherPointsInPath = new List<Vector3>();

    public void Move(List<Vector3> newPath)
    {
        otherPointsInPath.Clear();
        otherPointsInPath.AddRange(newPath);
    }

    private void OnTriggerEnter(Collider collide)
    {
        if (collide.gameObject.tag == "Treasure")
        {
            Destroy(collide.gameObject);
        }

        if (collide.gameObject.tag == "Bonus")
        {
            Destroy(collide.gameObject);
            RandomBonus();
        }
    }

    #region Bonus
    private void RandomBonus()
    {
        int randomNumber = UnityEngine.Random.Range(0, 3);

        switch (randomNumber)
        {
            case 0:
                SpeedBonus();
                break;
            case 1:
                VisionBonus();
                break;
            case 2:
                TimeBonus();
                break;
        }
    }

    #region Speed Bonus
    public bool isSpeedBonus = false;
    float speedTimer = 10f;

    private const float MOVE_SPEED_BONUS = 12f;
    private const float MOVE_SPEED_BASIC = 8f;

    private void SpeedBonus()
    {
        CancelInvoke("ReturnToBasicSpeed");

        isSpeedBonus = true;
        speedTimer = 10f;
        moveSpeed = MOVE_SPEED_BONUS;

        bonusUI.bonusSpeedIcon.fillAmount = 1f;

        Invoke("ReturnToBasicSpeed", speedTimer);
    }

    private void ReturnToBasicSpeed()
    {
        isSpeedBonus = false;
        speedTimer = 10f;
        moveSpeed = MOVE_SPEED_BASIC;
    }
    #endregion

    #region Vision Bonus
    public bool isVisionBonus = false;
    float visionTimer = 10f;

    private const float LIGHT_VISION_BONUS = 78 * 1.5f;
    private const float LIGHT_VISION_BASIC = 78f;

    private const float DISTANCE_MOVE_BONUS = 5 * 1.5f;
    private const float DISTANCE_MOVE_BASIC = 5;

    private void VisionBonus()
    {
        CancelInvoke("ReturnToBasicVision");

        isVisionBonus = true;
        visionTimer = 10f;

        gameLight.spotAngle = LIGHT_VISION_BONUS;
        distanceToMove = DISTANCE_MOVE_BONUS;

        bonusUI.bonusVisionIcon.fillAmount = 1f;

        Invoke("ReturnToBasicVision", visionTimer);
    }

    private void ReturnToBasicVision()
    {
        isVisionBonus = false;
        visionTimer = 10f;

        gameLight.spotAngle = LIGHT_VISION_BASIC;
        distanceToMove = DISTANCE_MOVE_BASIC;
    }
    #endregion

    private void TimeBonus()
    {
        gameTimer.inGameTime += 15f;
    }
    #endregion

    private float Distance(float xPlayer, float xClick, float yPlayer, float yClick)
    {
        float distance;

        return distance = Mathf.Sqrt(Mathf.Pow(xClick - xPlayer, 2) + Mathf.Pow(yClick - yPlayer, 2));
    }
}