using UnityEngine;
using System.Collections;

public class platform_up_down : MonoBehaviour
{


    private Vector3 origPoint;
    private Vector3 toObject;
    float distance;
    bool reached = false;
    Rigidbody platformRigidBody;
    public float moveSpeed=0.8f;

    bool flag = false;
    public void Start()
    {
        origPoint = transform.position;
        platformRigidBody = GetComponent<Rigidbody>();
        toObject.y = transform.position.y + 15.0f;    // position 
        toObject.x = transform.position.x;

    }

    public void FixedUpdate()
    {
        if (flag)
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

    }

    void move(Vector3 pos, Vector3 towards)
    {
        Vector3 direction = (towards - pos).normalized;
        platformRigidBody.MovePosition(platformRigidBody.position + direction * moveSpeed * Time.deltaTime);

        transform.position = Vector3.MoveTowards(pos, towards, .1f);

    }



    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            flag = true;
        }

    }
}
