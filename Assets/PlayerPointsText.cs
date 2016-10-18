using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlayerPointsText : MonoBehaviour {

    public Player p1;
    public Player p2;
    public Text textP1;
    public Text textP2;
    // Use this for initialization
    void Start () {
        textP1.text = "Player 1 Points: ";
        textP2.text = "Player 1 Points: ";
    }
	
	// Update is called once per frame
	void Update () {

        textP1.text = "Player 1 Points: "+p1.myPoints.ToString();
        textP2.text = "Player 1 Points: "+ p1.myPoints.ToString();
    }
}
