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
    public bool                 isShielded          = false;
    public bool                 rewardIsActive      = false;
    public int                  obAvoids            = 0;
    public int                  maxAvoids           = 3;
    public int                  curAvoids = 0;
    public float                maxRewardCountdown;
    public float                rewardCountdown;
    public Role                 myRole;
    public PowerUp_State        myPowerUp;

    //Components
    private Rigidbody           myRigidbody;
    private PlayerController    player;

    void Start()
    {
        //init components
        myRigidbody         = GetComponent<Rigidbody>();
        player              = GetComponent<PlayerController>();

        //init variables
        rewardCountdown     = maxRewardCountdown;
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
    }

    void CheckPowerUpReward()
    {
        if(rewardIsActive)
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
                curAvoids++;//increment the number of avoided obstacles
                if(curAvoids >= maxAvoids)//if the number of avoided obstacles have past the max
                {
                    myPowerUp = (PowerUp_State)Random.Range(1, 4);

                    //reset
                    rewardIsActive  = false;
                }
            }
        }
    }

    //Obstacles

    //Slow
    IEnumerator Obstacle_Slow()
    {
        float prevSpeed = player.movSpeed;
        player.movSpeed -= 5f;
        yield return new WaitForSeconds(5f);
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
}
