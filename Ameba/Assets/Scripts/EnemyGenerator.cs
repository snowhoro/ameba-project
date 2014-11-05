using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGenerator : MonoBehaviour
{
    public List<GameObject> enemies;
    public List<GameObject> enemyType;

    public void GenerateEnemies(Rect[] rooms)
    {
        enemies = new List<GameObject>();
        foreach(Rect room in rooms)
        {
            /*int rnd = Random.Range(0, 5);
            while()*/
            int posX = (int)Random.Range(room.xMin+1, room.xMax-2);
            int posY = (int)Random.Range(room.yMin+1, room.yMax-2);
           
            GameObject enemy = (GameObject) GameObject.Instantiate(enemyType[Random.Range(0,enemyType.Count)], new Vector3(posX,posY, -1), Quaternion.identity);
            enemies.Add(enemy);
            enemy.GetComponent<Enemy>().Position = new Vector2(posX, posY);

            //print(enemies[0].Position);
            break;
        }

        
    }

    public void DeleteEnemies()
    {
        foreach(GameObject enem in enemies)
        {
            Destroy(enem);
        }
        enemies.Clear();
    }

}