using UnityEngine;
using System.Collections;

public class Camera_Follow : MonoBehaviour
{
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
    private Camera myCamera;
    public GameObject[] players;

    void Start()
    {
        myCamera = GetComponent<Camera>();
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();

        if(target)
        {
            Vector3 point = myCamera.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - myCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.4f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            Vector3 destination = transform.position + delta;

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
        
    }
    //old code
  /*  void FindTarget()
    {
        if (players.Length == 2 && players[0] != null && players[1] != null)
        {
            if (players[0].transform.position.x - players[1].transform.position.x > 0)
            {
                target = players[0].transform;
            }
            else
            {
                target = players[1].transform;
            }

            if (players[0] != null)
            {
                target = players[0].transform;
            }
            else if (players[1] != null)
            {
                target = players[1].transform;
            }
        }
        if(players[0] != null && players[1] == null)
        {
            target = players[0].transform;
        }
        if (players[1] != null && players[0] == null)
        {
            target = players[1].transform;
        }
    }*/
    
    //camera follow the middle of 2 players
        void FindTarget()
    {
        if (players.Length == 2 && players[0] != null && players[1] != null)
        {
            float distance;

           
               if (players[0].transform.position.x - players[1].transform.position.x > 0)
            {
                distance = Mathf.Abs((players[0].transform.position.x - players[1].transform.position.x) / 2);

                target.position = new Vector3(distance, players[0].transform.position.y, 0);
                target = players[0].transform;


            }

            
            if (players[0] != null)
            {
                target = players[0].transform;
            }
            else if (players[1] != null)
            {
                target = players[1].transform;
            }
        }
        if (players[0] != null && players[1] == null)
        {
            target = players[0].transform;
        }
        if (players[1] != null && players[0] == null)
        {
            target = players[1].transform;
        }
    }

}
