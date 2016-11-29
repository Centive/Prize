using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    //Variables
    public float maxSpeed = 0;
    public float movSpeed = 0;
    public float jumpPower = 0;
    public bool isGrounded;

    //Components
    public Rigidbody myRigidbody;
    private Animator myAnimator;
    private TrailRenderer myTrail;
    private PlayerHandler playerHandler;

    //Prefab
    public GameObject shieldPuPrefab;

    //Controls
    public KeyCode jump;
    public KeyCode slide;
    public KeyCode usePowerUp;

    //power-ups
    public GameObject darkBallPrefab;
    public GameObject dropObstaclePrefab;

    //Power-ups audio
    public AudioSource[] powerAudios;
    private AudioSource darkballSfx;
    private AudioSource dropObstacleSfx;
    private AudioSource shieldSfx;
    private AudioSource speedSfx;
    //Controls audio
    public AudioSource[] playerControlAudios;
    //jump slide audio
    private AudioSource jumpSFX;
    private AudioSource slideSFX;
    
    //Start
    void Start()
    {
        //init components/gameobjects
        myRigidbody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<PlayerHandler>().myModel.GetComponent<Animator>();
        myTrail = GetComponent<TrailRenderer>();
        playerHandler = GetComponent<PlayerHandler>();
        powerAudios = GameObject.Find("PlayerSFX").GetComponents<AudioSource>();
        playerControlAudios = GameObject.Find("PlayerControlSFX").GetComponents<AudioSource>();
        //init audio clips
        darkballSfx = powerAudios[0];
        dropObstacleSfx = powerAudios[1];
        shieldSfx = powerAudios[2];
        speedSfx = powerAudios[3];

        //Control Audio
        jumpSFX = playerControlAudios[0];
        slideSFX = playerControlAudios[1];


        //Init variables
        movSpeed = maxSpeed;
        playerHandler.myRole = PlayerHandler.Role.Runner;
        playerHandler.myPowerUp = PlayerHandler.PowerUp_State.None;
    }

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
        myAnimator.SetFloat("MySpeed", myRigidbody.velocity.x);//ANIMATION: Trigger trigger running animation

        if (isGrounded)
        {
            //Jump
            if (Input.GetKeyDown(jump))
            {
                myAnimator.SetTrigger("jump");
                jumpSFX.Play();
                myRigidbody.velocity += Vector3.up * jumpPower;
            }
        }

        //Slide
        if (Input.GetKeyDown(slide) && !myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Sliding"))
        {
            myAnimator.SetTrigger("slide");
            slideSFX.Play();
            StopCoroutine(slideCoroutine());
            StartCoroutine(slideCoroutine());
        }

        //Use powerup
        if (Input.GetKeyDown(usePowerUp))
        {
            switch (playerHandler.myPowerUp)
            {
                case PlayerHandler.PowerUp_State.Darkball:
                    {
                        darkballSfx.Play();
                        PowerUpDarkBall();
                        break;
                    }
                case PlayerHandler.PowerUp_State.DropObstacle:
                    {
                        dropObstacleSfx.Play();
                        PowerUpDropObstacle();
                        break;
                    }
                case PlayerHandler.PowerUp_State.Shield:
                    {
                        shieldSfx.Play();
                        StopCoroutine(PowerUpShield());
                        StartCoroutine(PowerUpShield());
                        break;
                    }
                case PlayerHandler.PowerUp_State.Speed:
                    {
                        speedSfx.Play();
                        StopCoroutine(PowerUpSpeed());
                        StartCoroutine(PowerUpSpeed());
                        break;
                    }
            }
        }
    }

    //Ground checks
    void OnCollisionStay(Collision col)
    {
        //Check if grounded
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Incline")
        {
            isGrounded = true;
        }
    }
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "Incline")
        {
            isGrounded = false;
        }
    }

    //POWER UPS///////////////////////////////////////////////////////
    //drop obs
    void PowerUpDropObstacle()
    {
        Instantiate(dropObstaclePrefab, new Vector3(transform.position.x - 2.5f, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        playerHandler.myPowerUp = PlayerHandler.PowerUp_State.None;
    }
    void PowerUpDarkBall()
    {
        Instantiate(darkBallPrefab, new Vector3(transform.position.x + 3f, transform.position.y + 1.5f, transform.position.z), Quaternion.identity);
        playerHandler.myPowerUp = PlayerHandler.PowerUp_State.None;
    }

    //slide(delay)
    IEnumerator slideCoroutine()
    {
        movSpeed = maxSpeed - 2f;
        yield return new WaitForSeconds(1);
        movSpeed = maxSpeed;
    }

    //Shield
    IEnumerator PowerUpShield()
    {
        playerHandler.isShielded = true;
        yield return new WaitForSeconds(2f);
        playerHandler.isShielded = false;
        playerHandler.myPowerUp = PlayerHandler.PowerUp_State.None;
    }

    //Speed
    IEnumerator PowerUpSpeed()
    {
        movSpeed = maxSpeed + 5f;
        yield return new WaitForSeconds(2f);
        movSpeed = maxSpeed;
        playerHandler.myPowerUp = PlayerHandler.PowerUp_State.None;
    }
}
