using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game_Manager : MonoBehaviour
{
    public enum GameState
    {
        Phase1_Pause,
        Phase1_Start,
        Phase2_Pause,
        Phase2_Start,
        End
    }

    //Game UI
    public Text         uiCountdown;
    public Text         uiPlayerWarning;
    public Text         uiGameOver;
    public Image        uiInstructions;

    //gameobjects
    public GameObject   prefabPlayer;
    public GameObject[] players;
    private GameObject  altar;

    //variables
    public GameState    curState;
    public bool         checkChaserWin      = false;
    public bool         isPhase2            = false,
                        isPhase1Countdown   = false,
                        isPhase2Countdown   = false;
    private float       player1Speed,
                        player2Speed;

    private float       phase1Timer         = 6f,
                        phase2Timer         = 6f;

    void Start()
    {
        //init components/gameobjects
        players = GameObject.FindGameObjectsWithTag("Player");
        altar = GameObject.FindGameObjectWithTag("Altar");

        //init variables
        curState = GameState.Phase1_Pause;
        uiInstructions.gameObject.SetActive(true);

        if (players.Length == 2)
        {
            player1Speed = players[0].GetComponent<Player>().movSpeed;  //Set player speed
            player2Speed = players[1].GetComponent<Player>().movSpeed;  //Set player speed
        }
    }

    void Update()
    {
        if (players.Length == 2)
        {
            IsPlayerBehind();
            GetState();
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
                    players[0].GetComponent<Player>().movSpeed = 0;
                    players[1].GetComponent<Player>().movSpeed = 0;

                    //Start Game
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        uiInstructions.gameObject.SetActive(false);
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
                            players[0].GetComponent<Player>().movSpeed = player1Speed;
                            players[1].GetComponent<Player>().movSpeed = player2Speed;
                            uiCountdown.gameObject.SetActive(false);
                            curState = GameState.Phase1_Start;
                        }
                    }
                    break;
                }
            case GameState.Phase1_Start:
                {
                    //Check for phase 2
                    SetPhase2();
                    break;
                }
            case GameState.Phase2_Pause:
                {
                    //Set players to not move
                    players[0].GetComponent<Player>().movSpeed = 0;
                    players[1].GetComponent<Player>().movSpeed = 0;

                    //Start Countdown
                    if (isPhase2Countdown)
                    {
                        uiCountdown.gameObject.SetActive(true);
                        phase2Timer -= Time.deltaTime;
                        uiCountdown.text = (int)phase2Timer + "";

                        if (phase2Timer < 0)
                        {
                            //Set their speeds
                            players[0].GetComponent<Player>().movSpeed = player1Speed;
                            players[1].GetComponent<Player>().movSpeed = player2Speed;
                            uiCountdown.gameObject.SetActive(false);
                            curState = GameState.Phase2_Start;
                        }
                    }
                    break;
                }
            case GameState.Phase2_Start:
                {
                    CheckChaserWin();
                    break;
                }
            case GameState.End:
                {
                    uiGameOver.gameObject.SetActive(true);
                    if (players[0] != null)
                    {
                        if(players[0].GetComponent<Player>().myRole == Player.Role.Runner)
                        {
                            uiGameOver.text = "P1 RUNNER WINS";
                        }
                        if (players[0].GetComponent<Player>().myRole == Player.Role.Chaser)
                        {
                            uiGameOver.text = "P1 CHASER WINS";
                        }
                    }

                    if (players[1] != null)
                    {
                        if (players[1].GetComponent<Player>().myRole == Player.Role.Runner)
                        {
                            uiGameOver.text = "P2 RUNNER WINS";
                        }
                        if (players[1].GetComponent<Player>().myRole == Player.Role.Chaser)
                        {
                            uiGameOver.text = "P2 CHASER WINS";
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
            if (players[0].GetComponent<Player>().myRole == Player.Role.Chaser)
            {
                players[1].transform.position = new Vector3(players[0].transform.position.x + 5f, 0.0f, 0.0f);
            }

            if (players[1].GetComponent<Player>().myRole == Player.Role.Chaser)
            {
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
        if (players[0].GetComponent<Player>().myRole == Player.Role.Chaser)
        {
            if (players[0].transform.InverseTransformPoint(players[1].transform.position).x >= 0)
            {
                curState = GameState.End;
                Destroy(players[1]);
            }
        }

        if (players[1].GetComponent<Player>().myRole == Player.Role.Chaser)
        {
            if (players[1].transform.InverseTransformPoint(players[0].transform.position).x >= 0)
            {
                curState = GameState.End;
                Destroy(players[0]);
            }
        }
    }

   //Check if a player is too far behind the other player
    void IsPlayerBehind()
    {
        float distance = 0;
        if (players[0] != null && players[1] != null)
            distance = Vector3.Distance(players[0].transform.position, players[1].transform.position);

        if (distance > 15f)
        {
            uiPlayerWarning.gameObject.SetActive(true);
            uiPlayerWarning.text = "Player1 is too far behind! ";
        }

        if (distance < -15f)
        {
            uiPlayerWarning.text = "Player2 is too far behind";
            uiPlayerWarning.gameObject.SetActive(true);
        }

        if(distance > -15f && distance < 15f)
        {
            uiPlayerWarning.gameObject.SetActive(false);
        }
    }
}
