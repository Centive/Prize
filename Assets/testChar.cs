using UnityEngine;
using System.Collections;

public class testChar : MonoBehaviour
{
    public enum ThisIsAnnoying
    {
        death,
        despair
    }

    public ThisIsAnnoying why;

    private Animator DoAbsolutelyNothing;

    void Start()
    {
        DoAbsolutelyNothing = GetComponent<Animator>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DoAbsolutelyNothing.SetFloat("MySpeed", 10.0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            why = ThisIsAnnoying.death;
            DoAbsolutelyNothing.SetInteger("MyRole", (int)why);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            why = ThisIsAnnoying.despair;
            DoAbsolutelyNothing.SetInteger("MyRole", (int)why);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            DoAbsolutelyNothing.SetTrigger("jump");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            DoAbsolutelyNothing.SetTrigger("slide");
        }
    }
}
