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

        var sTT = GameObject.FindGameObjectWithTag("SpeedText");

        if(sTT != null)
        {
            speedTimerText = sTT.GetComponent<Text>();
        }

        var vTT = GameObject.FindGameObjectWithTag("VisionText");

        if (vTT != null)
        {
            visionTimerText = vTT.GetComponent<Text>();
        }
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
        {
            speedTimer -= Time.deltaTime;
            speedTimerText.text = speedTimer.ToString("0");
        }  
        
        if (isVisionBonus)
        {
            visionTimer -= Time.deltaTime;
            visionTimerText.text = visionTimer.ToString("0");
        }   
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
                VisionBonus();
                break;
            case 1:
                VisionBonus();
                break;
            case 2:
                VisionBonus();
                break;
        }
    }

    #region Speed Bonus
    bool isSpeedBonus = false;
    float speedTimer = 10f;
    public Text speedTimerText;

    private void SpeedBonus()
    {
        CancelInvoke("ReturnToBasicSpeed");

        isSpeedBonus = true;
        speedTimer = 10f;
        moveSpeed = 12f;

        Invoke("ReturnToBasicSpeed", 10f);
    }

    private void ReturnToBasicSpeed()
    {
        isSpeedBonus = false;
        speedTimer = 10f;
        moveSpeed = 8f;
        speedTimerText.text = "No Speed Bonus in use";
    }
    #endregion

    #region Vision Bonus
    bool isVisionBonus = false;
    float visionTimer = 10f;
    public Text visionTimerText;

    private void VisionBonus()
    {
        CancelInvoke("ReturnToBasicVision");

        isVisionBonus = true;
        visionTimer = 10f;
        //TODO : Récupérer dans le start light et modifier ici son rayon

        Invoke("ReturnToBasicVision", 10f);
    }

    private void ReturnToBasicVision()
    {
        isVisionBonus = false;
        visionTimer = 10f;
        //TODO : annuler les effets d'au dessus
        visionTimerText.text = "No Vision Bonus in use";
    }
    #endregion


    private void TimeBonus()
    {
        throw new NotImplementedException();
    }
    #endregion

    private float Distance(float xPlayer, float xClick, float yPlayer, float yClick)
    {
        float distance;

        return distance = Mathf.Sqrt(Mathf.Pow(xClick - xPlayer, 2) + Mathf.Pow(yClick - yPlayer, 2));
    }
}