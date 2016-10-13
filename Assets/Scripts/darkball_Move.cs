using UnityEngine;
using System.Collections;

public class darkballMove : MonoBehaviour
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
        toObject.y = transform.position.y + 2.0f;
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
        platformRigidBody.MovePosition(platformRigidBody.position + direction * 1f * Time.deltaTime);

        transform.position = Vector3.MoveTowards(pos, towards, .1f);

    }

    public void onColliderPoint(Collider other)
    {
        if (other.gameObject.tag == "Reachpoint")
        {
            reached = true;
        }

    }
}
