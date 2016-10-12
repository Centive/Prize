using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class playerBehindIndicate : MonoBehaviour
{

    // Use this for initialization

    public Player p1;
    public Player p2;
    public Text text;

    private float position1;
    private float position2;

    private float distance;

    void Start()
    {
        text.gameObject.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        indicate();
    }

    void indicate()
    {
        position1 = p1.transform.position.x;
        position2 = p2.transform.position.x;

        distance = position1-position2;

        if (distance > 15f)
     {
            text.gameObject.SetActive(true);
            text.text = "Player2 is too far behind! ";

       }
        if (distance < - 15f)
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
