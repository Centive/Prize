﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    //Role
    public enum Role
    {
        Runner,
        Chaser
    }

    //Variables
    public float maxSpeed = 0;
    public float movSpeed = 0;
    public float jumpPower = 0;
    public float myPoints = 0;
    public bool isGrounded;
    public Role myRole;


    //Components
    public Rigidbody myRigidbody;
    private Animator myAnimator;
    private TrailRenderer myTrail;

    //Models
    public GameObject modelDagger;
    public GameObject modelSlash;

    //Controls
    public KeyCode jump;
    public KeyCode slide;

    public KeyCode throwDarkBall;
    public KeyCode dropObstacleKey;

    //power-ups
    private int shield = 0;
    private int dropObstacle = 0;
    private int ball = 0;
    public GameObject ballPrefab;
    public GameObject obstacle;

    //Start
    void Start()
    {
        //init components/gameobjects
        myRigidbody = GetComponent<Rigidbody>();
        myAnimator = GetComponentInChildren<Animator>();
        myTrail = GetComponent<TrailRenderer>();

        //Disable models
        modelDagger.GetComponent<MeshRenderer>().enabled = false;
        modelSlash.GetComponent<MeshRenderer>().enabled = false;

        //Init variables
        movSpeed = maxSpeed;
        myRole = Role.Runner;
    }

    //Ground fixes
    void FixedUpdate()
    {
        // Get the velocity
        Vector3 horizontalMove = myRigidbody.velocity;
        // Don't use the vertical velocity
        horizontalMove.y = 0;
        // Calculate the approximate distance that will be traversed
        float distance = horizontalMove.magnitude * Time.fixedDeltaTime;
        // Normalize horizontalMove since it should be used to indicate direction
        horizontalMove.Normalize();
        RaycastHit hit;

        // Check if the body's current velocity will result in a collision
        if (myRigidbody.SweepTest(horizontalMove, out hit, distance) && hit.transform.gameObject.tag == "Wall")
        {
            // If so, stop the movement
            myRigidbody.velocity = new Vector3(0, myRigidbody.velocity.y, 0);
        }
    }

    //Update
    void Update()
    {
        Controls();
    }

    //Controls
    void Controls()
    {
        //Auto-Run
        myRigidbody.velocity = new Vector3(1 * movSpeed, myRigidbody.velocity.y);

        if (isGrounded)
        {
            //Jump
            if (Input.GetKeyDown(jump))
            {
                myAnimator.SetTrigger("Jump");
                myRigidbody.velocity += Vector3.up * jumpPower;
            }
        }

        //Slide
        if (Input.GetKeyDown(slide) && !myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Sliding"))
        {
            myAnimator.SetTrigger("Slide");
            myAnimator.SetBool("isSliding", true);
            StartCoroutine(slideCoroutine());
        }
        //Slide
        if (Input.GetKeyUp(slide))
        {
            myAnimator.SetBool("isSliding", false);
        }

        if (Input.GetKeyDown(throwDarkBall))
        {
            // if (myRole ==Role.Chaser)
            // {
            if (ball > 0)
            {
                dark_ball();
                ball--;

            }
            // }


        }
        if (Input.GetKeyDown(dropObstacleKey))
        {
            if (dropObstacle > 0)
            {
                drop_obstacle();
                dropObstacle--;
            }

        }
    }

    IEnumerator slideCoroutine()
    {
        movSpeed -= 2f;
        yield return new WaitForSeconds(1);
        movSpeed += 2f;
    }

    //Check for obstacles/powerups/coins
    void OnTriggerEnter(Collider col)
    {
        //Coin collecting
        if (col.gameObject.tag == "Coin")
        {
            myPoints++;
            if ((myPoints % 5) == 0)
            {
                StartCoroutine(PowerUp_Speed());
            }
        }

        //POWER UPS////////////////////////////////////
        //speed
        if (col.gameObject.tag == "PowerUp_Speed")
        {
            Destroy(col.gameObject);
            StartCoroutine(PowerUp_Speed());
        }

        //shield
        if (col.gameObject.tag == "Shield")
        {
            shield += 1;
            Destroy(col.gameObject);

        }




        //OBSTACLES////////////////////////////////////
        //Slow
        if (col.gameObject.tag == "obSlow")
        {
            if (shield == 0)
            {
                StartCoroutine(Obstacle_Slow());
            }
            shield -= 1;

        }

        //Stun
        if (col.gameObject.tag == "obStun")

        {
            if (shield == 0)
            {
                StartCoroutine(Obstacle_Stun());
                Destroy(col.gameObject, 0.5f);
            }
            shield -= 1;
        }

        //Power-ups////////////////////////////////////

        if (col.gameObject.tag == "Dropobstacle")
        {
            dropObstacle++;
            Destroy(col.gameObject);

        }
        if (col.gameObject.tag == "Darkball")
        {
                      
                ball++;
               
                Destroy(col.gameObject);
            if (ballPrefab.transform.position == this.transform.position)
            {
                movSpeed -= 2f;
            }

        }
    }

    //Ground checks
    void OnCollisionStay(Collision col)
    {
        //Check if grounded
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    //POWER UPS///////////////////////////////////////////////////////
    //drop obs
    void drop_obstacle()
    {
        Instantiate(obstacle, new Vector3(transform.position.x - 2.5f, transform.position.y + 1f, transform.position.z), Quaternion.identity);
    }

    void dark_ball()
    {
        Instantiate(ballPrefab, new Vector3(transform.position.x + 3f, transform.position.y + 1.5f, transform.position.z), Quaternion.identity);

    }

    //Speed
    IEnumerator PowerUp_Speed()
    {
        float prevSpeed = movSpeed;
        movSpeed += 5f;
        yield return new WaitForSeconds(2f);
        movSpeed = prevSpeed;
    }

    //OBSTACLES///////////////////////////////////////////////////////

    //Slow
    IEnumerator Obstacle_Slow()
    {
        float prevSpeed = movSpeed;
        movSpeed -= 5f;
        yield return new WaitForSeconds(5f);
        movSpeed = prevSpeed;
    }

    //Stun
    IEnumerator Obstacle_Stun()
    {
        float prevSpeed = movSpeed;
        movSpeed = 0f;
        yield return new WaitForSeconds(0.5f);
        movSpeed = prevSpeed;
    }



}
