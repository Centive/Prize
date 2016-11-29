using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour
{
    // FadeInOut

    public Texture2D fadeTexture;
    public float fadeSpeed = 0.6f;
    public int drawDepth = -1000;

    private float alpha = 1.0f;
    private float fadeDir = -1f;
    private bool fadeNow = false;
    //fade in -1 or out is 1

    void OnGUI()
    {
        if (fadeNow)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.depth = drawDepth;

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        }
    }

    public void BeginFade()
    {
        alpha = 1.0f;
        fadeNow = true;
    }
}
