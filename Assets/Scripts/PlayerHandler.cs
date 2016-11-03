
using UnityEngine;
using System.Collections;

public class PlayerHandler : MonoBehaviour
{
    //Role
    public enum Role
    {
        Runner,
        Chaser
    }

    //PowerUp Hold
    public enum PowerUp_State
    {
        None,
        Darkball,
        DropObstacle,
        Shield,
        Speed
    }

    //Variables
    public bool isShielded = false;
    public bool rewardIsActive = false;
    public int obAvoids = 0;
    public int maxAvoids = 3;
    public int curAvoids = 0;
    public float maxRewardCountdown;
    public float rewardCountdown;
    public Role myRole;
    public PowerUp_State myPowerUp;

    //Gameobjects
    private GameObject gameManager;

    //Components
    private Rigidbody myRigidbody;
    private PlayerController player;
    private Animator myAnimator;

    //platform
    private Transform NormalPlatform;
    float timer = 0f;

    //Models
    public GameObject modelDagger;
    public GameObject modelLantern;


    //Sounds
    private AudioSource[] collisionSFX;
    private AudioSource[] dodgeAudioSources;

    private AudioSource slopeDownsource;
    private AudioSource slopeUpsource;

    private AudioSource platformSlow;
    private AudioSource platformFast;

    private AudioSource slideObs;
    private AudioSource jumpObs;

    private AudioSource dodgedOb;
    private AudioSource powerUpGet;


    void Start()
    {
        //init components
        myRigidbody = GetComponent<Rigidbody>();
        player = GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game_Manager");
        myAnimator = GetComponent<Animator>();

        //init variables
        rewardCountdown = maxRewardCountdown;

        //sounds
        collisionSFX = GameObject.Find("Player/CollisionSFX").GetComponents<AudioSource>();
        dodgeAudioSources = GameObject.Find("Player/PlayerRewardSFX").GetComponents<AudioSource>();
        slopeDownsource = collisionSFX[0];
        slopeUpsource = collisionSFX[1];
        platformFast = collisionSFX[2];
        platformSlow = collisionSFX[3];
        slideObs = collisionSFX[4];
        jumpObs = collisionSFX[5];
        dodgedOb = dodgeAudioSources[0];
        powerUpGet = dodgeAudioSources[1];

        //Disable models
        modelDagger.GetComponent<MeshRenderer>().enabled = false;
        modelLantern.GetComponent<SkinnedMeshRenderer>().enabled = false;
    }

    //Wall collide detection
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

    void Update()
    {
        CheckPowerUpReward();
        haloforchaser();
        SwitchRole_ItemModels();
    }

    void SwitchRole_ItemModels()
    {
        myAnimator.SetInteger("MyRole", (int)myRole);
        switch (gameManager.GetComponent<Game_Manager>().curState)
        {
            case Game_Manager.GameState.Phase1_Pause:
                {
                    modelDagger.GetComponent<MeshRenderer>().enabled = false;
                    modelLantern.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    break;
                }
            case Game_Manager.GameState.Phase2_Pause:
                {
                    switch (myRole)
                    {
                        case Role.Runner:
                            {
                                modelLantern.GetComponent<SkinnedMeshRenderer>().enabled = true;
                                break;
                            }
                        case Role.Chaser:
                            {
                                modelDagger.GetComponent<MeshRenderer>().enabled = true;
                                break;
                            }
                    }
                    break;
                }
        }
    }

    void CheckPowerUpReward()
    {
        if (rewardIsActive)
        {
            rewardCountdown -= Time.deltaTime;
            if (rewardCountdown < 0)
            {
                //Reset
                rewardIsActive = false;
            }
        }
        else
        {
            curAvoids = 0;
            rewardCountdown = maxRewardCountdown;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //OBSTACLES////////////////////////////////////
        //Slow
        if (col.gameObject.tag == "obSlow")
        {
            if (!isShielded)
            {
                //reset
                rewardIsActive = false;
                StartCoroutine(Obstacle_Slow());
            }
        }

        //Stun
        if (col.gameObject.tag == "obStun")
        {
            if (!isShielded)
            {
                //reset
                rewardIsActive = false;
                StartCoroutine(Obstacle_Stun());
                Destroy(col.gameObject, 0.5f);
            }
            Destroy(col.gameObject);
        }

        //Check if avoided obstacles
        if (col.gameObject.tag == "Avoided")
        {
            //Check if reward timer is not active
            if (!rewardIsActive && myPowerUp == PowerUp_State.None)
            {
                rewardIsActive = true;
            }

            //Check if reward timer is active
            if (rewardIsActive)
            {
                curAvoids++;                //increment the number of avoided obstacles
                if (curAvoids >= maxAvoids) //if the number of avoided obstacles have past the max
                {
                    myPowerUp = (PowerUp_State)Random.Range(1, 4);
                    powerUpGet.Play();//play powerup get
                    curAvoids = 0;
                    Debug.Log("duude you got a powerup. NICE!");

                    //reset
                    rewardIsActive = false;
                }
            }

            if (curAvoids != maxAvoids - 1)
            {
                dodgedOb.Play();//play dodge ob
            }
        }

        //platform(ground) slow and fast
        if (col.gameObject.tag == "Platformslow")

        {
            if (player.movSpeed > 0f)

            {
                Debug.Log("slow");
                platformSlow.Play();
                platformSlow.Play();
                player.movSpeed -= 1f;
            }
        }

        if (col.gameObject.tag == "Platformreallyslow")
        {
            if (player.movSpeed > 0f)
            {
                Debug.Log("slow");

                platformSlow.Play();

                player.movSpeed -= 5f;
            }
        }
        if (col.gameObject.tag == "Platformfast")
        {
            if (player.movSpeed > 0f)
            {
                Debug.Log("fast");

                platformFast.Play();
                player.movSpeed += 3f;
            }
        }
        if (col.gameObject.tag == "Platformreallyfast")
        {
            if (player.movSpeed > 0f)
            {
                Debug.Log("fast");
                platformFast.Play();

                platformFast.Play();
                player.movSpeed += 5f;
            }
        }
    }

    //stay on platform
    void OnCollisionStay(Collision col)
    {
        //Handle platforms
        if (col.gameObject.tag == "Platformmoving")
        {
            if (timer <= 2f)
            {
                this.transform.position = col.transform.position;
            }
        }
    }

    //handle Incline
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Incline")
        {
            CheckGroundIncline(col.gameObject);
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Incline")
        {
            StartCoroutine(NormGround());
        }
    }
    
    //incline check
    void CheckGroundIncline(GameObject ground)
    {
        //get angle from game obj
        Vector3 angle = ground.transform.eulerAngles;
        Vector3 ParentAngle = ground.transform.parent.gameObject.transform.eulerAngles;
        
        //check for a slow inc
        if(ParentAngle.y <= 1f)
            if (angle.z >= 5f && angle.z <= 45f && ParentAngle.y == 0f)
            {
                slopeUpsource.Play();
                StartCoroutine(SlowGround());

            }
        //check for a fast inc
        if(ParentAngle.y >= 179f)
        {
            if (angle.z >= 5f && angle.z <= 45f)
            {
                slopeDownsource.Play();
                StartCoroutine(FastGround());
            }
        }
    }

    IEnumerator SlowGround()//Incline slow check
    {
        Debug.Log("SLOW INCLINE: " + player.movSpeed);
        player.movSpeed -= 4f;
        yield return null;
    }

    IEnumerator FastGround()//Incline fast check
    {
        Debug.Log("FAST INCLINE: " + player.movSpeed);
        player.movSpeed += 4f;
        yield return null;
    }

    IEnumerator NormGround()//Flat ground check
    {
        player.movSpeed = player.maxSpeed;
        yield return null;
    }
    
    //Obstacles

    //Slow
    IEnumerator Obstacle_Slow()
    {
        float prevSpeed = player.movSpeed;
        player.movSpeed -= 5f;
        yield return new WaitForSeconds(2f);
        player.movSpeed = prevSpeed;
    }

    //Stun
    IEnumerator Obstacle_Stun()
    {
        float prevSpeed = player.movSpeed;
        player.movSpeed = 0f;
        yield return new WaitForSeconds(0.5f);
        player.movSpeed = prevSpeed;
    }

    void haloforchaser()
    {

        if (myRole == Role.Chaser)
        {
            Behaviour h = (Behaviour)GetComponent("Halo");
            h.enabled = true;
        }


    }
}

//private AudioSource[]       audioSources;
//private AudioSource         dodgedOb;//john
//private AudioSource         powerUpGet; //john mate! fuuuuuuuuuu
//
//
//void Start()
//{
//    //init components
//    myRigidbody     = GetComponent<Rigidbody>();
//    player          = GetComponent<PlayerController>();
//    audioSources    = GameObject.Find("PlayerRewardSFX").GetComponents<AudioSource>();
//
//    //init audio clips
//    dodgedOb        = audioSources[0];
//    powerUpGet      = audioSources[1];
//
//
//
//    //init variables
//    rewardCountdown = maxRewardCountdown;
//
//    //sounds
//    slopeDownsource = GetComponent<AudioSource>();
//
//}
//
////Wall collide detection
//void FixedUpdate()
//{
//    // Get the velocity
//    Vector3 horizontalMove = myRigidbody.velocity;
//    // Don't use the vertical velocity
//    horizontalMove.y = 0;
//    // Calculate the approximate distance that will be traversed
//    float distance = horizontalMove.magnitude * Time.fixedDeltaTime;
//    // Normalize horizontalMove since it should be used to indicate direction
//    horizontalMove.Normalize();
//    RaycastHit hit;
//
//    // Check if the body's current velocity will result in a collision
//    if (myRigidbody.SweepTest(horizontalMove, out hit, distance) && hit.transform.gameObject.tag == "Wall")
//    {
//        // If so, stop the movement
//        myRigidbody.velocity = new Vector3(0, myRigidbody.velocity.y, 0);
//    }
//
//
//}
//
//void Update()
//{
//    CheckPowerUpReward();
//}
//
//void CheckPowerUpReward()
//{
//    if (rewardIsActive)
//    {
//        rewardCountdown -= Time.deltaTime;
//        if (rewardCountdown < 0)
//        {
//            //Reset
//            rewardIsActive = false;
//        }
//    }
//    else
//    {
//        curAvoids = 0;
//        rewardCountdown = maxRewardCountdown;
//    }
//}
//
//void OnTriggerEnter(Collider col)
//{
//    //OBSTACLES////////////////////////////////////
//    //Slow
//    if (col.gameObject.tag == "obSlow")
//    {
//        if (!isShielded)
//        {
//            //reset
//            rewardIsActive = false;
//            StartCoroutine(Obstacle_Slow());
//        }
//    }
//
//    //Stun
//    if (col.gameObject.tag == "obStun")
//    {
//        if (!isShielded)
//        {
//            //reset
//            rewardIsActive = false;
//            StartCoroutine(Obstacle_Stun());
//            Destroy(col.gameObject, 0.5f);
//        }
//        Destroy(col.gameObject);
//    }
//
//    //Check if avoided obstacles
//    if (col.gameObject.tag == "Avoided")
//    {
//        //Check if reward timer is not active
//        if (!rewardIsActive && myPowerUp == PowerUp_State.None)
//        {
//            rewardIsActive = true;
//        }
//
//        //Check if reward timer is active
//        if (rewardIsActive)
//        {
//            curAvoids++;                //increment the number of avoided obstacles
//            if (curAvoids >= maxAvoids) //if the number of avoided obstacles have past the max
//            {
//                myPowerUp = (PowerUp_State)Random.Range(1, 4);
//                powerUpGet.Play();//play powerup get
//                curAvoids = 0;
//                Debug.Log("duude you got a powerup. NICE!");
//
//                //reset
//                rewardIsActive = false;
//            }
//        }
//
//        if(curAvoids != maxAvoids - 1)
//        {
//          dodgedOb.Play();//play dodge ob
//          Debug.Log("WHAT A DODGE");
//        }
//
//    }
//
//    //platform(ground) slow and fast
//    if (col.gameObject.tag == "Platformslow")
//
//    {
//        if (player.movSpeed > 0f)
//
//        {
//            Debug.Log("slow");
//
//
//            player.movSpeed -= 1f;
//        }
//    }
//
//    if (col.gameObject.tag == "Platformreallyslow")
//    {
//        if (player.movSpeed > 0f)
//
//        {
//            Debug.Log("slow");
//
//
//            player.movSpeed -= 5f;
//        }
//    }
//    if (col.gameObject.tag == "Platformfast")
//    {
//        if (player.movSpeed > 0f)
//
//        {
//            Debug.Log("fast");
//
//
//            player.movSpeed += 3f;
//        }
//    }
//    if (col.gameObject.tag == "Platformreallyfast")
//    {
//        if (player.movSpeed > 0f)
//
//        {
//            Debug.Log("fast");
//
//
//            player.movSpeed += 5f;
//        }
//    }
//}
//
////stay on platform
//void OnCollisionStay(Collision col)
//{
//    if (col.gameObject.tag == "Platformmoving")
//
//    {
//       
//        if (timer <=2f)
//        {
//            this.transform.position = col.transform.position;
//        }
//    }
//}
//
////Incline
//void OnCollisionEnter(Collision col)
//{
//
//    if (col.gameObject.tag == "Incline")
//    {
//        inclineslowfast(col.gameObject);
//    }
//}
//
//void inclineslowfast(GameObject incline)
//{
//    Vector3 angleInclie = incline.transform.eulerAngles;
//    Debug.Log(angleInclie);
//    if (angleInclie.z >= 45f && angleInclie.z <= 315f)
//    {
//        player.movSpeed -= 4f;
//        //Debug.Log("slow");
//        //Debug.Log(player.movSpeed);
//    }
//    else if (angleInclie.z >= 315f)
//    {
//        slopeDownsource.PlayOneShot(slopeDown, 0.5f);
//
//        player.movSpeed += 4f;
//        //Debug.Log("fast");
//        //Debug.Log(player.movSpeed);
//    }
//}
//
//void OnCollisionExit(Collision col)
//{
//    if (col.gameObject.tag == "Incline")
//    {
//        player.movSpeed = 15f;
//
//    }
//}
//
////Obstacles
//
////Slow
//IEnumerator Obstacle_Slow()
//{
//    float prevSpeed = player.movSpeed;
//    player.movSpeed -= 5f;
//    yield return new WaitForSeconds(5f);
//    player.movSpeed = prevSpeed;
//}
//
////Stun
//IEnumerator Obstacle_Stun()
//{
//    float prevSpeed = player.movSpeed;
//    player.movSpeed = 0f;
//    yield return new WaitForSeconds(0.5f);
//    player.movSpeed = prevSpeed;
//}