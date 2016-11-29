using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayPhase2Cutscene : MonoBehaviour
{
    public MovieTexture scene;

    void Start()
    {
        GetComponent<RawImage>().texture = scene as MovieTexture;
        scene.Play();
    }

    public bool CheckIfPlaying()
    {
        return scene.isPlaying;
    }
}