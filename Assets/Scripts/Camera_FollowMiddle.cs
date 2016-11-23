using UnityEngine;
using System.Collections;
public class Camera_FollowMiddle : MonoBehaviour
{

    // Use this for initialization

    public GameObject p1;
    public GameObject p2;

    private Transform cameraT;
    private Transform p1T;
    private Transform p2T;

    void Start()
    {
        cameraT = this.transform;

        //start position
        cameraT.position = new Vector3(-30.02f, 8.54f, -29.78f);

        p1T = p1.transform;
        p2T = p2.transform;
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();
    }

    void FindTarget()
    {
        float distanceX;
        float distanceY;
        
        if (p1 != null && p2 != null)
        {
            cameraT.position = this.transform.position;

            //average of x&y position
            distanceX = Mathf.Abs((p1T.position.x + p2T.position.x) / 2);
            distanceY = Mathf.Abs((p1T.position.y + p2T.position.y) / 2 + 1f);
            cameraT.position = new Vector3(distanceX, distanceY, -29.78f);
        }
    }
}