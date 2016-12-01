using UnityEngine;
using System.Collections;

public class EffectPosition : MonoBehaviour {
    public int xPos;
    public int yPos;
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {

        this.transform.position = new Vector3(Camera.main.transform.position.x+xPos, Camera.main.transform.position.y+yPos,0);


    }
}
