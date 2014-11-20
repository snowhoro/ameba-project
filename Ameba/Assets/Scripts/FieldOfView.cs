using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FieldOfView : MonoBehaviour 
{

    public int range = 5;
	private Rect fov;
    
    void Start () 
    {
        FoV();
	}    

    public void FoV(Vector2 pos)
    {
        Vector2 tmp = pos;
        tmp.x = tmp.x - range / 2f;
        tmp.y = tmp.y - range / 2f;

        fov = new Rect(tmp.x, tmp.y, range, range);

        ShowTiles();
    }

    public void FoV()
    {
        Vector2 tmp = transform.position;
        tmp.x = tmp.x - range / 2f;
        tmp.y = tmp.y - range / 2f;

        fov = new Rect(tmp.x, tmp.y, range, range);

        ShowTiles();
    }

    private void ShowTiles()
    {

        List<GameObject> tileList = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<DungeonGenerator>().tileList;

        foreach (GameObject tile in tileList)
        {
            if (fov.Contains(tile.transform.position) && tile.GetComponent<SpriteRenderer>().color.a != 1.0f)
            {
                Color fullcolor = tile.GetComponent<SpriteRenderer>().color;
                fullcolor.a = 1.0f;
                tile.GetComponent<SpriteRenderer>().color = fullcolor;
            }
        }

        List<Base> enemies = GameObject.FindGameObjectWithTag("Enemies").GetComponent<EnemyGenerator>().enemies;
        foreach (Base enemy in enemies)
        {
            if (fov.Contains(enemy.transform.position) && enemy.GetComponent<SpriteRenderer>().color.a != 1.0f)
            {
                Color fullcolor = enemy.gameObject.GetComponent<SpriteRenderer>().color;
                fullcolor.a = 1.0f;
                enemy.gameObject.GetComponent<SpriteRenderer>().color = fullcolor;
                enemy.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "Enemy" && !coll.isTrigger && coll.gameObject.GetComponent<SpriteRenderer>().color.a != 1.0f)
        {
            Color fullcolor = coll.gameObject.GetComponent<SpriteRenderer>().color;
            fullcolor.a = 1.0f;
            coll.gameObject.GetComponent<SpriteRenderer>().color = fullcolor;

            coll.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
