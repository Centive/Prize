using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class playerBehindIndicate : MonoBehaviour
{

    // Use this for initialization

    public GameObject[] players;
    public Text text;

    private float[] position = new float[2];

    private float distance;

    void Start()
    {
        //init components/gameobjects
        players = GameObject.FindGameObjectsWithTag("Player");

        //init variables
        text.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        indicate();
    }

    void indicate()
    {
        position[0] = players[0].transform.position.x;
        position[1] = players[1].transform.position.x;

        distance = position[0] - position[1];

        if (distance > 15f)
        {
            text.gameObject.SetActive(true);
            text.text = "Player2 is too far behind! ";
        }
        if (distance < -15f)
        {
            text.text = "Player1 is too far behind";
            text.gameObject.SetActive(true);
        }
        else
        {
            text.gameObject.SetActive(false);
        }
    }
}
