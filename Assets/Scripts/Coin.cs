using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    //Variables
    public float        rotateSpeed = 50;

    //Components
    private AudioSource itemGet;
    private Renderer    myRenderer;
    private BoxCollider myCollider;

    void Start()
    {
        //init components
        itemGet     = GetComponent<AudioSource>();
        myRenderer  = GetComponent<Renderer>();
        myCollider  = GetComponent<BoxCollider>();
    }
    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider col)
    {
        //Check if hit coin
        if (col.gameObject.tag == "Player")
        {
            itemGet.Play();
            myRenderer.enabled = false;
            myCollider.enabled = false;
            Destroy(this.gameObject, itemGet.clip.length);
        }
    }
}