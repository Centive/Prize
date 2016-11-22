using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    public enum GameState
    {
        None,
        Phase1_Pause,
        Phase1_Start,
        Phase2_Pause,
        Phase2_Start,
        EndScene,
        EndMenu
    }

    //Game UI
    public Text uiCountdown;
    public Text uiPlayerWarning;
    public Text uiGameOver;
    public Image uiInstructions;

    public Canvas startScreen;
    public InputField getP1Name;
    public InputField getP2Name;

    public GameObject playerNameCanvas;
    public Button startButton;
    public Text showP1Name;
    public Text showP2Name;


    //GameOver
    public Image gameOverImg;
    public Text winText;
    public GameObject EndingCutscene1_prefab;
    public GameObject EndingCutscene2_prefab;
    private GameObject endAnimation;
    public Image p1RunnerWin_prefab;
    public Image p1ChaserWin_prefab;
    public Image p2RunnerWin_prefab;
    public Image p2ChaserWin_prefab;

    //SFX
    private AudioSource[] BGMSFX;
    private AudioSource startScreenBGM;
    private AudioSource phase2SFX;
    private AudioSource phase1SFX;
    private AudioSource playerBehindSFX;
    private AudioSource getDaggarSFX;
    private AudioSource pressStartSFX;

    private AudioSource endBGM;
    //player sfx
    private AudioSource dieSFX;
    private AudioSource playerWin;

    //who got the daggar
    public Text daggarText;

    private int flag = 0;


    //gameobjects
    public GameObject[] players;
    private GameObject altar;
    private Transform halfwayPoint;

    //variables
    public GameState curState = GameState.None;
    public bool checkChaserWin = false;
    public bool isPhase2 = false,
                        isPhase1Countdown = false,
                        isPhase2Countdown = false;
    private float player1Speed = 0,
                        player2Speed = 0;
    private float phase1Timer = 6f,
                        phase2Timer = 6f;
    public float runnerEndPosition = 870f;

    void Start()
    {
        //Audio
        BGMSFX = GameObject.Find("BGM").GetComponents<AudioSource>();

        startScreenBGM = BGMSFX[0];
        phase1SFX = BGMSFX[1];
        phase2SFX = BGMSFX[2];
        playerWin = BGMSFX[3];
        dieSFX = BGMSFX[4];
        playerBehindSFX = BGMSFX[5];
        getDaggarSFX = BGMSFX[6];
        pressStartSFX = BGMSFX[7];
        endBGM = BGMSFX[8];
        startScreenBGM.Play();

        //init components/gameobjects
        players = GameObject.FindGameObjectsWithTag("Human");
        altar = GameObject.FindGameObjectWithTag("Altar");

        if (altar != null)//for testing purposes
            halfwayPoint = altar.transform;

        //init variables
        curState = GameState.Phase1_Pause;
        startButton.onClick.AddListener(gameStartClick);

        if (players.Length == 2)
        {
            player1Speed = players[0].GetComponent<PlayerController>().maxSpeed;  //Set player speed
            player2Speed = players[1].GetComponent<PlayerController>().maxSpeed;  //Set player speed
        }

    }

    void Update()
    {
        if (players.Length == 2)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            if (curState == GameState.EndScene)
            {
                //redo Game
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SceneManager.LoadScene("Testing_Area");
                }
            }
            IsPlayerBehind();
            GetState();
            checkWhoGotDaggar();

        }

    }

    //Find what state in the game were at
    void GetState()
    {
        switch (curState)
        {
            case GameState.Phase1_Pause:
                {
                    //Set players to not move
                    players[0].GetComponent<PlayerController>().movSpeed = 0;
                    players[1].GetComponent<PlayerController>().movSpeed = 0;

                    //Start Game
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                       
                        uiInstructions.gameObject.SetActive(false);
                        playerNameCanvas.gameObject.SetActive(true);
                        uiInstructions.gameObject.SetActive(false);
                        getPlayerName();
                        isPhase1Countdown = true;
                    }

                    //Start Countdown
                    if (isPhase1Countdown)
                    {
                        uiCountdown.gameObject.SetActive(true);
                        phase1Timer -= Time.deltaTime;
                        uiCountdown.text = (int)phase1Timer + "";

                        if (phase1Timer < 0)
                        {
                            //Set their speeds
                            players[0].GetComponent<PlayerController>().movSpeed = player1Speed;
                            players[1].GetComponent<PlayerController>().movSpeed = player2Speed;
                            uiCountdown.gameObject.SetActive(false);
                            curState = GameState.Phase1_Start;
                            pressStartSFX.Play();

                            //Audio
                            phase1SFX.Play();
                            startScreenBGM.Pause();
                        }
                    }
                    break;
                }
            case GameState.Phase1_Start:
                {
                    CheckDeath();   //Checks if a player died in the level environment
                    SetPhase2();    //Checks if for phase 2
                    break;
                }
            case GameState.Phase2_Pause:
                {
                    //Set players to not move
                    players[0].GetComponent<PlayerController>().movSpeed = 0;
                    players[1].GetComponent<PlayerController>().movSpeed = 0;

                    //Start Countdown
                    if (isPhase2Countdown)
                    {
                        uiCountdown.gameObject.SetActive(true);
                        phase2Timer -= Time.deltaTime;
                        uiCountdown.text = (int)phase2Timer + "";

                        if (phase2Timer < 0)
                        {
                            //Set their speeds
                            players[0].GetComponent<PlayerController>().movSpeed = player1Speed;
                            players[1].GetComponent<PlayerController>().movSpeed = player2Speed;
                            uiCountdown.gameObject.SetActive(false);
                            curState = GameState.Phase2_Start;
                            //Audio
                            phase2SFX.Play();
                            phase1SFX.Pause();
                        }
                    }
                    break;
                }
            case GameState.Phase2_Start:
                {
                    CheckDeath();   //Checks if a player died in the level environment
                    CheckChaserWin();
                    CheckRunnerWin();
                    break;
                }
            case GameState.EndScene:
                {
                    // uiGameOver.gameObject.SetActive(true);
                    // if (players[0] != null)
                    // {
                    //     if (players[0].GetComponent<PlayerHandler>().myRole == PlayerHandler.Role.Runner)
                    //     {
                    //         uiGameOver.text = "P1 RUNNER WINS\nPress Space to restart or Esc to close";
                    //     }
                    //     if (players[0].GetComponent<PlayerHandler>().myRole == PlayerHandler.Role.Chaser)
                    //     {
                    //         uiGameOver.text = "P1 CHASER WINS\nPress Space to restart or Esc to close";
                    //     }
                    // }
                    //
                    // if (players[1] != null)
                    // {
                    //     if (players[1].GetComponent<PlayerHandler>().myRole == PlayerHandler.Role.Runner)
                    //     {
                    //         uiGameOver.text = "P2 RUNNER WINS\nPress Space to restart or Esc to close";
                    //     }
                    //     if (players[1].GetComponent<PlayerHandler>().myRole == PlayerHandler.Role.Chaser)
                    //     {
                    //         uiGameOver.text = "P2 CHASER WINS\nPress Space to restart or Esc to close";
                    //     }
                    // }
                    // //redo Game
                    // if (Input.GetKeyDown(KeyCode.Space))
                    // {
                    //     SceneManager.LoadScene("Testing_Area");
                    // }
                    //
                    // //Runner stop at end
                    // if (players[0] != null)
                    //     if (players[0].transform.position.x >= 880f)
                    //         players[0].GetComponent<PlayerController>().movSpeed = 0;
                    // if (players[1] != null)
                    //     if (players[1].transform.position.x >= 880f)
                    //         players[1].GetComponent<PlayerController>().movSpeed = 0;

                    /* - Check which player is alive
                     * - Check the player's role
                     * - Depending on the role play that role's win animation
                     * 
                     * Chaser:
                     * - fade 
                     * - disable main camera
                     * - play end animation
                     * 
                     * *Note* new main camera is in the end animation obj's children:: That is the new main camera
                     * 
                     * Runner:
                     * - Nothing
                     */

                    if (players[0] != null)
                    {
                        players[0].GetComponent<PlayerController>().movSpeed = 0;//disable speed
                        switch (players[0].GetComponent<PlayerHandler>().myRole)
                        {
                            //Do chaser win animation
                            case PlayerHandler.Role.Chaser:
                                {
                                    this.GetComponent<Fade>().BeginFade();
                                    if (Camera.main != null)
                                        Camera.main.enabled = false;
                                    endAnimation = GameObject.Instantiate(EndingCutscene2_prefab, transform.position, Quaternion.identity) as GameObject;
                                    break;
                                }
                            //Do runner win animation
                            case PlayerHandler.Role.Runner:
                                {
                                    //nothing
                                    break;
                                }
                        }
                    }
                    if (players[1] != null)
                    {
                        players[1].GetComponent<PlayerController>().movSpeed = 0;//disable speed
                        switch (players[1].GetComponent<PlayerHandler>().myRole)
                        {
                            //Do chaser win animation
                            case PlayerHandler.Role.Chaser:
                                {
                                    this.GetComponent<Fade>().BeginFade();
                                    if (Camera.main != null)
                                        Camera.main.enabled = false;
                                    endAnimation = GameObject.Instantiate(EndingCutscene1_prefab, transform.position, Quaternion.identity) as GameObject;
                                    break;
                                }
                            //Do runner win animation
                            case PlayerHandler.Role.Runner:
                                {
                                    //nothing
                                    break;
                                }
                        }

                    }
                   
                    curState = GameState.EndMenu;
                    break;
                }
            case GameState.EndMenu:
                {
                    /* - Check which player is alive
                     * - Check if that player is a chaser or runner role
                     * - Check if end animation is finished
                     * - Turn on a endScreen prefab depending on role
                     * 
                     * *NOTE* no end animation for runner
                     */
                     //AUDIO
                    startScreenBGM.Pause();
                    phase1SFX.Pause();
                    phase2SFX.Pause();
                    endBGM.Play();

                    if (players[0] != null)
                    {
                        switch (players[0].GetComponent<PlayerHandler>().myRole)
                        {
                            case PlayerHandler.Role.Chaser:
                                {
                                    if (!endAnimation.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EndAnim"))
                                    {
                                        p2ChaserWin_prefab.gameObject.SetActive(true);
                                    }
                                    break;
                                }
                            case PlayerHandler.Role.Runner:
                                {
                                    p2RunnerWin_prefab.gameObject.SetActive(true);
                                    break;
                                }
                        }
                    }
                    if (players[1] != null)
                    {
                        switch (players[1].GetComponent<PlayerHandler>().myRole)
                        {
                            case PlayerHandler.Role.Chaser:
                                {
                                    if (!endAnimation.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("EndAnim"))
                                    {
                                        p1ChaserWin_prefab.gameObject.SetActive(true);
                                    }
                                    break;
                                }
                            case PlayerHandler.Role.Runner:
                                {
                                    p1RunnerWin_prefab.gameObject.SetActive(true);
                                    break;
                                }
                        }
                    }

                    break;
                }
        }
    }

    //Set positions for phase 2
    void SetPhase2()
    {
        //Move the runner 5 units infront of the chaser
        if (isPhase2)
        {
            //Check for other players
            if (players[0].GetComponent<PlayerHandler>().myRole == PlayerHandler.Role.Chaser)
            {
                players[0].transform.position = halfwayPoint.position;
                players[1].transform.position = new Vector3(players[0].transform.position.x + 5f, 0.0f, 0.0f);
            }

            if (players[1].GetComponent<PlayerHandler>().myRole == PlayerHandler.Role.Chaser)
            {
                players[1].transform.position = halfwayPoint.position;
                players[0].transform.position = new Vector3(players[1].transform.position.x + 5f, 0.0f, 0.0f);
            }

            //move to next state
            curState = GameState.Phase2_Pause;
            isPhase2Countdown = true;
            isPhase2 = false;
        }

    }

    //Available in phase2_start
    void CheckChaserWin()
    {
        if (players[0].GetComponent<PlayerHandler>().myRole == PlayerHandler.Role.Chaser)
        {
            //Check if the chaser has won
            if (players[0].transform.InverseTransformPoint(players[1].transform.position).x <= 0)
            {
                curState = GameState.EndScene;
                Destroy(players[1]);
            }
        }

        if (players[1].GetComponent<PlayerHandler>().myRole == PlayerHandler.Role.Chaser)
        {
            //Check if the chaser has won
            if (players[1].transform.InverseTransformPoint(players[0].transform.position).x <= 0)
            {
                curState = GameState.EndScene;
                Destroy(players[0]);
            }
        }
    }
    void CheckRunnerWin()
    {
        if (players[0].GetComponent<PlayerHandler>().myRole == PlayerHandler.Role.Runner)
        {
            if (players[0].transform.position.x >= runnerEndPosition)
            {
                curState = GameState.EndScene;
                Destroy(players[1]);
            }
        }

        if (players[1].GetComponent<PlayerHandler>().myRole == PlayerHandler.Role.Runner)
        {
            if (players[1].transform.position.x >= runnerEndPosition)
            {
                curState = GameState.EndScene;
                Destroy(players[0]);
            }
        }
    }

    //Check if a player is too far behind the other player
    void IsPlayerBehind()
    {
        float distance = 0;
        float pos1 = 0;
        float pos2 = 0;

        if (players[0] != null && players[1] != null)
        {
            pos1 = players[0].transform.position.x;
            pos2 = players[1].transform.position.x;
        }

        distance = pos1 - pos2;             //get distance of the players

        IsPlayerBehindWarnings(distance);   //get warnings


        //Disable ui warning if players are within range
        if (distance > -15f && distance < 15f)
        {
            uiPlayerWarning.gameObject.SetActive(false);
        }
    }
    void IsPlayerBehindWarnings(float distance)
    {
        //Warnings

        //PlayerHandler 1 Check
        if (distance < -32f)
        {
            uiPlayerWarning.text = showP1Name.text + " don't fall too far behind!";
            uiPlayerWarning.gameObject.SetActive(true);
            playerBehindSFX.Play();
        }

        //PlayerHandler 2 Check
        if (distance > 32f)
        {
            uiPlayerWarning.gameObject.SetActive(true);
            uiPlayerWarning.text = showP2Name.text + " Player2 don't fall too far behind!";
            playerBehindSFX.Play();

        }

        //////////////////////////////////////////////////////////////////////
        //Check if players have fell behind too much
        switch (curState)
        {
            case GameState.Phase1_Start:
                {
                    if (distance < -42f)//if player 1 has fell behind
                    {
                        players[0].GetComponent<PlayerHandler>().myRole = PlayerHandler.Role.Runner;
                        players[1].GetComponent<PlayerHandler>().myRole = PlayerHandler.Role.Chaser;
                        isPhase2 = true;
                        Destroy(altar);
                    }

                    if (distance > 42f)//if player 2 has fell behind
                    {
                        players[0].GetComponent<PlayerHandler>().myRole = PlayerHandler.Role.Chaser;
                        players[1].GetComponent<PlayerHandler>().myRole = PlayerHandler.Role.Runner;
                        isPhase2 = true;
                        Destroy(altar);
                    }
                    break;
                }
            case GameState.Phase2_Start:
                {
                    if (distance < -42f)//if player 1 has fell behind
                    {
                        curState = GameState.EndScene;
                        Destroy(players[0]);
                    }

                    if (distance > 42f)//if player 2 has fell behind
                    {
                        curState = GameState.EndScene;
                        Destroy(players[1]);
                    }
                    break;
                }
        }
    }

    //Check if player died from level environments
    void CheckDeath()
    {
        if (players[0] != null)//Check if player 1 has fell from a pit
        {
            if (players[0].transform.position.y <= -10f)
            {
                // gameOverImg.gameObject.SetActive(true);
                winText.text = showP2Name.text + " Won the Game !";
                //   winText.gameObject.SetActive(true);
                // playerWin.Play();
                DeathGUIsfx();

                curState = GameState.EndScene;
                Destroy(players[0]);
            }
        }

        if (players[1] != null)//Check if player 2 has fell from a pit
        {
            if (players[1].transform.position.y <= -10f)
            {
                // gameOverImg.gameObject.SetActive(true);
                winText.text = showP1Name.text + " Won the Game !";
                //  winText.gameObject.SetActive(true);
                //  playerWin.Play();

                DeathGUIsfx();

                curState = GameState.EndScene;
                Destroy(players[1]);
            }
        }
    }

    void DeathGUIsfx()
    {
        playerNameCanvas.gameObject.SetActive(false);
        dieSFX.Play();
        phase1SFX.Pause();
        phase2SFX.Pause();
        playerWin.PlayDelayed(1.2F);
        winText.gameObject.SetActive(true);

        gameOverImg.gameObject.SetActive(true);


    }

    //Daggar
    void checkWhoGotDaggar()
    {
        flag = altar.GetComponent<Altar>().Justflag();
        if (daggarText != null)
        {

            if (flag == 1)
            {
                daggarText.text = showP1Name.text + " got the Daggar!";

                showP1Name.text = showP1Name.text + " Chaser!";

                showP2Name.text = showP2Name.text + " Runner!";
                Destroy(daggarText, 5f);


            }
            else if (flag == 2)
            {
                daggarText.text = showP2Name.text + " got the Daggar!";

                showP2Name.text = showP2Name.text + " Chaser!";

                showP1Name.text = showP1Name.text + " Runner!";

                Destroy(daggarText, 5f);
            }

            getDaggarSFX.Play();
        }
    }

    void gameStartClick()
    {
       

        uiInstructions.gameObject.SetActive(true);
        startScreen.gameObject.SetActive(false);
    }

    void getPlayerName()
    {
        //getPlayerName
        Debug.Log(getP1Name.text);
        Debug.Log(getP2Name.text);

        showP1Name.text = getP1Name.text;
        showP2Name.text = getP2Name.text;

    }

}