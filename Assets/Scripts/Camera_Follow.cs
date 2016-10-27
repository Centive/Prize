using UnityEngine;
using System.Collections;

public class Camera_Follow : MonoBehaviour
{
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
    private Camera myCamera;
    private GameObject gameManager;
    public GameObject[] players;

    void Start()
    {
        //init components/game objects
        myCamera = GetComponent<Camera>();
        players = GameObject.FindGameObjectsWithTag("Player");
        gameManager = GameObject.Find("Game_Manager");
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();

        if (target)
        {
            Vector3 point = myCamera.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - myCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.4f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            Vector3 destination = transform.position + delta;

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
    }


    //old code
    void FindTarget()
    {
        float distance = 0;
        float pos1 = 0;
        float pos2 = 0;

        if (players.Length == 2)
        { 
            if (players[0] != null && players[1] != null)
            {
                pos1 = players[0].transform.position.x;
                pos2 = players[1].transform.position.x;
            }
        }
    distance = pos1 - pos2;

        if (distance< -1)
        {
            target = players[1].transform;
        }
        else if (distance > 0)
        {
            target = players[0].transform;
        }
    }
}