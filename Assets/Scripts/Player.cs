using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    //Role
    public enum Role
    {
        Runner,
        Chaser
    }

    //
    public enum PowerUp_State
    {
        None,
        Darkball,
        DropObstacle,
        Shield,
        Speed
    }

    //Variables
    private float slope;
    public float maxSpeed = 0;
    public float movSpeed = 0;
    public float jumpPower = 0;
    public float myPoints = 0;
    public bool isGrounded;
    public Role myRole;
    public PowerUp_State myPowerUp;

    //Components
    public Rigidbody myRigidbody;
    private Animator myAnimator;
    private TrailRenderer myTrail;

    //Models
    public GameObject modelDagger;
    public GameObject modelSlash;
    public GameObject prefabShield_PowerUp;

    //Controls
    public KeyCode jump;
    public KeyCode slide;
    public KeyCode usePowerUp;

    //power-ups
    private bool isShielded = false;
    public GameObject darkBallPrefab;
    public GameObject dropObstaclePrefab;

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
        myPowerUp = PowerUp_State.None;
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
    //void Cheating()
    //{
    //    //if (Input.GetKeyDown(KeyCode.Alpha1))
    //    //{
    //    //    myPowerUp = PowerUp_State.Darkball;
    //    //}
    //    //if (Input.GetKeyDown(KeyCode.Alpha2))
    //    //{
    //    //    myPowerUp = PowerUp_State.DropObstacle;
    //    //}
    //    //if (Input.GetKeyDown(KeyCode.Alpha3))
    //    //{
    //    //    myPowerUp = PowerUp_State.Shield;
    //    //}
    //    //if (Input.GetKeyDown(KeyCode.Alpha4))
    //    //{
    //    //    myPowerUp = PowerUp_State.Speed;
    //    //}
    //}
    //Update
    void Update()
    {
        //Cheating();
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
        //Use powerup
        if (Input.GetKeyDown(usePowerUp))
        {
            switch (myPowerUp)
            {
                case PowerUp_State.Darkball:
                    {
                        PowerUpDropObstacle();
                        break;
                    }
                case PowerUp_State.DropObstacle:
                    {
                        PowerUpDarkBall();
                        break;
                    }
                case PowerUp_State.Shield:
                    {
                        StartCoroutine(PowerUpShield());
                        break;
                    }
                case PowerUp_State.Speed:
                    {
                        StartCoroutine(PowerUpSpeed());
                        break;
                    }
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
        //OBSTACLES////////////////////////////////////
        //Slow
        if (col.gameObject.tag == "obSlow")
        {
            if (!isShielded)
            {
                StartCoroutine(Obstacle_Slow());
            }
        }

        //Stun
        if (col.gameObject.tag == "obStun")
        {
            if (!isShielded)
            {
                StartCoroutine(Obstacle_Stun());
                Destroy(col.gameObject, 0.5f);
            }
            Destroy(col.gameObject);
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
    void PowerUpDropObstacle()
    {
        Instantiate(dropObstaclePrefab, new Vector3(transform.position.x - 2.5f, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        myPowerUp = PowerUp_State.None;
    }
    void PowerUpDarkBall()
    {
        Instantiate(darkBallPrefab, new Vector3(transform.position.x + 3f, transform.position.y + 1.5f, transform.position.z), Quaternion.identity);
        myPowerUp = PowerUp_State.None;
    }

    //Speed
    IEnumerator PowerUpSpeed()
    {
        float prevSpeed = movSpeed;
        movSpeed += 5f;
        yield return new WaitForSeconds(2f);
        movSpeed = prevSpeed;
        myPowerUp = PowerUp_State.None;
    }
    IEnumerator PowerUpShield()
    {
        isShielded = true;
        yield return new WaitForSeconds(2f);
        isShielded = false;
        myPowerUp = PowerUp_State.None;
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