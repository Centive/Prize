using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Manager : MonoBehaviour
{
    public GameObject prefabPlayer;
    public GameObject[] players;
    private GameObject altar;
    public bool phase2 = false;
    public bool checkChaserWin = false;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        altar = GameObject.FindGameObjectWithTag("Altar");

        if (players.Length == 2)
        {
            SetControls();
        }
    }

    void Update()
    {
        if (players.Length == 2)
        {
            //CheckChaserWin();
            CheckPhase2();
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
            phase2 = false;
            checkChaserWin = true;
        }

    }

    void SetControls()
    {
        players[0].GetComponent<Player>().jump = KeyCode.W;
        players[0].GetComponent<Player>().slide = KeyCode.S;

        players[1].GetComponent<Player>().jump = KeyCode.I;
        players[1].GetComponent<Player>().slide = KeyCode.K;
    }

    void CheckChaserWin()
    {
        if (checkChaserWin)
        {
            if (players[0].GetComponent<Player>().myRole == Player.Role.Chaser)
            {
                if (players[0].transform.InverseTransformPoint(players[1].transform.position).x >= 0)
                {
                    Destroy(players[1]);
                }
            }

            if (players[1].GetComponent<Player>().myRole == Player.Role.Chaser)
            {
                if (players[1].transform.InverseTransformPoint(players[0].transform.position).x >= 0)
                {
                    Destroy(players[0]);
                }
            }
        }
    }
}
