using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Inventory : MonoBehaviour 
{
    public Texture2D background;
    public float inv_windowWidth;
    public float inv_windowHeight;
    public List<Item> itemList;

    private float item_width = 50f;
    private float item_height = 50f;

    private float item_space = 20f;

    private Rect windowRect;

    public bool show_inv = false;
    

	void Start () 
    {
        windowRect = new Rect(Screen.width / 2 - inv_windowWidth/2, Screen.height / 2 - inv_windowHeight / 2, inv_windowWidth, inv_windowHeight);
        
        if(itemList.Count == 0)
            itemList = new List<Item>();	
	}
	
	void OnGUI() 
    {
        if (show_inv)
            windowRect = GUI.Window (0, windowRect, WindowFunction, "Inventory");
    }
    
    void WindowFunction (int windowID) 
    {
        float top = 30f;
        float left = 10f;

        GUIStyle labelstyle = new GUIStyle();
        labelstyle.normal.textColor = Color.white;
        labelstyle.fontSize = 10;

        GUI.Label(new Rect(left, top + item_height + 10f, item_width, item_height), itemList[0].itemName + " - " + itemList[0].number, labelstyle);
        if (GUI.Button(new Rect(left, top, item_width, item_height), itemList[0].img))
        {
            itemList[0].Use();
        }

        left += item_space + item_width;
        GUI.Label(new Rect(left, top + item_height + 10f, item_width, item_height), "asdf - 4"/*itemList[1].itemName + " - " + itemList[1].number*/, labelstyle);
        if (GUI.Button(new Rect(left, top, item_width, item_height), ""/*itemList[1].img*/))
        {
            itemList[1].Use();
        }

        left += item_space + item_width;
        GUI.Label(new Rect(left, top + item_height + 10f, item_width, item_height), "asdf - 4"/*itemList[1].itemName + " - " + itemList[1].number*/, labelstyle);
        if (GUI.Button(new Rect(left, top, item_width, item_height), background))
        {
            itemList[2].Use();
        }

        left = 10f;
        top += item_space + item_height + 20f;
        GUI.Label(new Rect(left, top + item_height + 10f, item_width, item_height), "asdf - 4"/*itemList[1].itemName + " - " + itemList[1].number*/, labelstyle);
        if (GUI.Button(new Rect(left, top, item_width, item_height), background))
        {
            itemList[3].Use();
        }

        left += item_space + item_width;
        GUI.Label(new Rect(left, top + item_height + 10f, item_width, item_height), "asdf - 4"/*itemList[1].itemName + " - " + itemList[1].number*/, labelstyle);
        if (GUI.Button(new Rect(left, top, item_width, item_height), background))
        {
            itemList[4].Use();
        }

        left += item_space + item_width;
        GUI.Label(new Rect(left, top + item_height + 10f, item_width, item_height), "asdf - 4"/*itemList[1].itemName + " - " + itemList[1].number*/, labelstyle);
        if (GUI.Button(new Rect(left, top, item_width, item_height), background))
        {
            itemList[5].Use();
        }
    } 
}
