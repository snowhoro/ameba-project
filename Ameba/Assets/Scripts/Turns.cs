using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turns : MonoBehaviour {

    private List<Base> nearbyEnemies;
    private List<Base> turnList;

	private static Turns _instance;
 
    public static Turns instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Turns>();
 
                DontDestroyOnLoad(_instance.gameObject);
            }
 
            return _instance;
        }
    }
 
    void Awake() 
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if(this != _instance)
                Destroy(this.gameObject);
        }

        nearbyEnemies = new List<Base>();
        turnList = new List<Base>();
    }

    void Update()
    {
        if (turnList.Count == 0)
        {
            turnList.Clear();
            LoadTurnList();
        }
    }

    private void LoadTurnList()
    {
        turnList.Add(Player.instance);

        if(nearbyEnemies.Count > 0)
            turnList.AddRange(nearbyEnemies);
    }

    public void AddEnemy(Base enemy)
    {
        nearbyEnemies.Add(enemy);
    }

    public void DeleteEnemy(Base enemy)
    {
        nearbyEnemies.Remove(enemy);
        turnList.Remove(enemy);
    }

    public bool StartTurn(Base enemy)
    {
        if(turnList.Count != 0)
        {
            if (turnList[0] == enemy)
                return true;
        }
        return false;
    }

    public void EndTurn(Base enemy)
    {
        turnList.Remove(enemy);
    }

    public void CleanTurnList()
    {
        turnList.Clear();
        nearbyEnemies.Clear();
    }

}
