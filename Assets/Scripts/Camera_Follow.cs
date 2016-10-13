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
        if(players.Length == 2)
            //FindTarget();

        if(target)
        {
            Vector3 point = myCamera.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - myCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.4f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            Vector3 destination = transform.position + delta;

            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
        
    }
    void FindTarget()
    {
        if(players[0].transform.position.x - players[1].transform.position.x > 0)
        {
            target = players[0].transform;
        }

        else
        {
            target = players[1].transform;
        }
    }
    //public Transform target;
    //public GameObject[] players;
    //public float dampTime = 0.15f;
    //public float yPos = 0;
    //private Vector3 velocity = Vector3.zero;
    //private Camera myCamera;
    //
    //void Start()
    //{
    //    myCamera = GetComponent<Camera>();
    //    players = GameObject.FindGameObjectsWithTag("Player");
    //}
    //
    //void Update()
    //{
    //    //FindTarget();
    //    //if (target)
    //    //{
    //        Vector3 point = myCamera.WorldToViewportPoint(target.position);
    //        Vector3 delta = target.position - myCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
    //        Vector3 destination = new Vector3(transform.position.x, transform.position.y + yPos, transform.position.z) + delta;
    //        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
    //    //}
    //}
    //
    ////Camera will follow who is first
    //void FindTarget()
    //{
    //    //if(players[0].transform.InverseTransformPoint(players[1].transform.position).x < 0)
    //    //{
    //    //    target = players[0].transform;
    //    //}
    //    //else
    //    //{
    //    //    target = players[1].transform;
    //    //}
    //}
}
