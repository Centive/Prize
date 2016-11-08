using UnityEngine;
using System.Collections;

public class Player_Shield : MonoBehaviour
{
    //Variables
    public float rotateSpeed = 50;

    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}