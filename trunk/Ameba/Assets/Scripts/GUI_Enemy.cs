using UnityEngine;
using System.Collections;

public class GUI_Enemy : MonoBehaviour {

    private float left;
    private float top;
    public Texture2D blackBar;
    public Texture2D redBar;
    public Texture2D gui_base;
    public float healthBarWidth;
    public float healthBarHeight;

    void OnGUI()
    {
        if (GetComponentInParent<SpriteRenderer>().color.a != 0f)
        {
            Vector3 healthBarWorldPosition = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
            Vector3 healthBarScreenPosition = Camera.main.WorldToScreenPoint(healthBarWorldPosition);

            left = healthBarScreenPosition.x - (healthBarWidth / 2);
            top = Screen.height - (healthBarScreenPosition.y);

            GUI.DrawTexture(new Rect(left + 5, top + 5, healthBarWidth - 10, healthBarHeight - 10), blackBar, ScaleMode.StretchToFill);
            GUI.DrawTexture(new Rect(left + 5, top + 5, (GetComponent<Base>().HitPoints * (healthBarWidth - 10)) / GetComponent<Base>().MaxHitPoints, healthBarHeight - 10), redBar, ScaleMode.StretchToFill);
            GUI.DrawTexture(new Rect(left, top, healthBarWidth, healthBarHeight), gui_base, ScaleMode.StretchToFill);
        }
    }
}
