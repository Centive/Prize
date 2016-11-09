using UnityEngine;
using System.Collections;

public class Altar : MonoBehaviour
{
    public GameObject[] players;
    private GameObject gameManager;

    private int flag = 0;

    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        gameManager = GameObject.Find("Game_Manager");
    }

    void OnTriggerEnter(Collider col)
    {
        if (players.Length == 2)
        {
            if (col.gameObject.tag == "Player")
            {
                if (col.gameObject == players[0])
                {
                    players[0].transform.parent.GetComponent<PlayerHandler>().myRole = PlayerHandler.Role.Chaser;
                    players[1].transform.parent.GetComponent<PlayerHandler>().myRole = PlayerHandler.Role.Runner;
                    gameManager.GetComponent<Game_Manager>().isPhase2 = true;
             
                    flag = 1;
                    //Destroy(this.gameObject);
                }
                if (col.gameObject == players[1])
                {
                    players[0].transform.parent.GetComponent<PlayerHandler>().myRole = PlayerHandler.Role.Runner;
                    players[1].transform.parent.GetComponent<PlayerHandler>().myRole = PlayerHandler.Role.Chaser;
                    gameManager.GetComponent<Game_Manager>().isPhase2 = true;

                    flag = 2;
                    //Destroy(this.gameObject);
                }
            }
        }
    }

    public int Justflag()
    {
        return flag;
    }
}
