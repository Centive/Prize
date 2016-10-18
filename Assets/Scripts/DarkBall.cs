using UnityEngine;
using System.Collections;

public class DarkBall : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(Vector3.right * 20f * Time.deltaTime);
        Destroy(this.gameObject, 2f);
        //transform.Translate(Vector3.up * Time.deltaTime, Space.World);

    }
}
