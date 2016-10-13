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
    public Text uiCountdown;
    public Image uiInstructions;

    //gameobjects
    public GameObject prefabPlayer;
    public GameObject[] players;
    private GameObject altar;

    //variables
    public GameState curState;
    public bool phase2 = false;
    public bool checkChaserWin = false;
    private bool StartPhase2 = false;
    private float player1Speed, player2Speed;
    private float phase1Timer = 6f, phase2Timer = 5f;

    void Start()
    {
        //init components/gameobjects
        players = GameObject.FindGameObjectsWithTag("Player");
        altar = GameObject.FindGameObjectWithTag("Altar");

        //init variables
        curState = GameState.Phase1_Pause;
        player1Speed = players[0].GetComponent<Player>().movSpeed;  //Set player speed
        player2Speed = players[1].GetComponent<Player>().movSpeed;  //Set player speed

        if (players.Length == 2)
        {
            SetControls();
        }
    }

    void Update()
    {
        if (players.Length == 2)
        {
            GetState();
        }
        //if (players.Length == 2)
        //{
        //    //CheckChaserWin();
        //    CheckPhase2();
        //}
    }

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
                        uiInstructions.enabled = false;
                        StartPhase2 = true;
                    }

                    //Start Countdown
                    if (StartPhase2)
                    {
                        phase1Timer -= Time.deltaTime;
                        uiCountdown.text = (int)phase1Timer + "";

                        if (phase1Timer < 0)
                        {
                            curState = GameState.Phase1_Start;
                            uiCountdown.enabled = false;
                        }
                    }
                    break;
                }
            case GameState.Phase1_Start:
                {
                    //Set their speeds
                    players[0].GetComponent<Player>().movSpeed = player1Speed;
                    players[1].GetComponent<Player>().movSpeed = player2Speed;

                    //Check for phase 2
                    CheckPhase2();
                    break;
                }
            case GameState.Phase2_Pause:
                {

                    break;
                }
            case GameState.Phase2_Start:
                {
                    CheckChaserWin();
                    break;
                }
            case GameState.End:
                {
                    break;
                }
        }
    }

    void CheckPhase2()
    {
        //Move the runner 5 units infront of the chaser
        if (phase2)
        {
            if (players[0].GetComponent<Player>().myRole == Player.Role.Chaser)//check if p1 is chaser
            {
                players[1].transform.position = new Vector3(players[0].transform.position.x + 5f, 0.0f, 0.0f);//move player 2 infront of chaser
            }

            if (players[1].GetComponent<Player>().myRole == Player.Role.Chaser)
            {
                players[0].transform.position = new Vector3(players[1].transform.position.x + 5f, 0.0f, 0.0f);
            }

            //move to next state
            curState = GameState.Phase2_Pause;
            phase2 = false;
        }

    }

    //Set player controls when the game starts
    void SetControls()
    {
        players[0].GetComponent<Player>().jump = KeyCode.W;
        players[0].GetComponent<Player>().slide = KeyCode.S;

        players[1].GetComponent<Player>().jump = KeyCode.I;
        players[1].GetComponent<Player>().slide = KeyCode.K;
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
}
