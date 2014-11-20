using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGenerator : MonoBehaviour
{
    public List<Base> enemies;
    public List<GameObject> enemyType;

    /*void Update()
    {
        if (enemies.Count != 0)
        {
            print("DELETE NODES");
            Pathfinding.instance.LoadEnemiesPosition(enemies);
        }
    }*/

    public void GenerateEnemies(Rect[] rooms)
    {
        enemies = new List<Base>();
        foreach(Rect room in rooms)
        {
            int nroEnemies = Random.Range(5, 5);
            nroEnemies = 1;
            for (int i = 0; i < nroEnemies; i++)
            {
                int posX;
                int posY;
                bool repeat = false;
                do
                {
                    posX = (int)Random.Range(room.xMin + 1, room.xMax - 2);
                    posY = (int)Random.Range(room.yMin + 1, room.yMax - 2);

                    foreach(Base en in enemies)
                    {
                        if ((Vector2)en.transform.position == new Vector2(posX, posY))
                        {
                            repeat = true;
                            break;
                        }
                        else
                            repeat = false;
                    }

                    if (Player.instance.Position == new Vector2(posX, posY))
                        repeat = true;

                } while(repeat);

                

                GameObject enemy = (GameObject) GameObject.Instantiate(enemyType[Random.Range(0, enemyType.Count)], new Vector3(posX, posY, -1), Quaternion.identity);
                enemies.Add(enemy.GetComponent<Base>());
                enemy.GetComponent<Enemy>().Position = new Vector2(posX, posY);
            }
            break;
        }

        
    }

    public void DeleteEnemies()
    {
        foreach(Base enem in enemies)
        {
            Destroy(enem.gameObject);
        }
        enemies.Clear();
    }

    public void DeleteEnemy(Base enemy)
    {
        enemies.Remove(enemy);
    }

}