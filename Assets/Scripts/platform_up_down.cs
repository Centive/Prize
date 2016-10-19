using UnityEngine;
using System.Collections;

public class platform_up_down : MonoBehaviour
{


    private Vector3 origPoint;
    private Vector3 toObject;
    float distance;
    bool reached = false;
    Rigidbody platformRigidBody;

    public void Start()
    {
        origPoint = transform.position;
        platformRigidBody = GetComponent<Rigidbody>();
        toObject.y = transform.position.y + 5.0f;    // position 
        toObject.x = transform.position.x;

    }

    public void FixedUpdate()
    {
        if (!reached)
        {
            move(transform.position, toObject);

            if (transform.position == toObject)
            {
                reached = true;
            }

        }
        else
        {
            distance = Vector3.Distance(transform.position, origPoint);

            move(transform.position, origPoint);

            if (transform.position == origPoint)
            {
                reached = false;
            }

        }
    }

    void move(Vector3 pos, Vector3 towards)
    {
        Vector3 direction = (towards - pos).normalized;
        platformRigidBody.MovePosition(platformRigidBody.position + direction * 0.5f * Time.deltaTime);

        transform.position = Vector3.MoveTowards(pos, towards, .1f);

    }

    

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            print("No longer in contact with " + col.transform.name);
        }

    }
}
