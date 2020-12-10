using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int maxRaycastDistance = 1000;

    [SerializeField]
    private LayerMask layerMaskForSolid;

    [SerializeField]
    private string groundTag;

    private GameController gameController;

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
    }

    void Update()
    {
        // Mouse click
        if (Input.GetMouseButtonDown(0) && null != gameController)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Vector3 temp = new Vector3(transform.position.x, ray.origin.y, transform.position.z);

            Debug.Log(ray.origin.x - transform.position.x);
            
            if (Physics.Raycast(ray, out hit, maxRaycastDistance, layerMaskForSolid))
            {
                //Debug.Log("Raycasting : " + hit.point + " on " + hit.collider.gameObject.tag);

                float distance = Distance(transform.position.x, ray.origin.x, transform.position.z, ray.origin.z);
                Debug.Log(distance);

                if (groundTag == hit.collider.gameObject.tag && distance <= 5f)
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
        if (collide.gameObject.tag == "Collectible")
        {
            Destroy(collide.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 5f);
    }

    private float Distance(float xPlayer, float xClick, float yPlayer, float yClick)
    {
        float distance;

        return distance = Mathf.Sqrt(Mathf.Pow(xClick - xPlayer, 2) + Mathf.Pow(yClick - yPlayer, 2));
    }
}
