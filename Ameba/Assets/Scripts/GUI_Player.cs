using UnityEngine;
using System.Collections;

public class GUI_Player : MonoBehaviour {

    public float gui_Width = 300f;
    public float gui_Height = 100f;

    public float leftMargin = 0f;
    public float topMargin = 0f;


    public Texture2D gui_Base;
    public Texture2D gui_Background1;
    public Texture2D gui_Background2;
    public Texture2D gui_eye;
    public Texture2D gui_hp;

    void OnGUI()
    {

        float left = 0f;
        float top = 0f;

        GUI.DrawTexture(new Rect(left + 8, top + 5, gui_Width - 210, gui_Height - 10), gui_Background2);
        GUI.DrawTexture(new Rect(left + 100, top + 25, gui_Width - 105, gui_Height - 55), gui_Background1);
        if (Player.instance.HitPoints > 0)
            GUI.DrawTexture(new Rect(left + 100, top + 25, (Player.instance.HitPoints * (gui_Width - 105)) / Player.instance.MaxHitPoints, gui_Height - 55), gui_hp);
        GUI.DrawTexture(new Rect(left + 30, top + 30, gui_Width - 255, gui_Height - 60), gui_eye);
        GUI.DrawTexture(new Rect(left, top, gui_Width, gui_Height), gui_Base);

    }
}
