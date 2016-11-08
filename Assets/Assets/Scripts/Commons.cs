using UnityEngine;
using System.Collections;

public class Commons : MonoBehaviour
{
    void Start()
    {

    }
    
    void Update()
    {

    }

    void RotateMe()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }
}
