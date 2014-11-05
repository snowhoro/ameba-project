using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

    private List<Node> nodeList;

    private static Pathfinding _instance;   
    public static Pathfinding instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Pathfinding>();

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }

    
	void Start () 
    {
        Reload();
	}

    public void Reload()
    {
        nodeList = new List<Node>();

        //CAMBIAR ESTO!
        GameObject.FindGameObjectWithTag("Dungeon").GetComponent<DungeonGenerator>().LoadNodes(nodeList);
    }

    public Vector2 NextStep(Vector2 target, Vector2 origin)
    {
        List<Node> tmp = CalculatePath(target, origin);

        if (tmp.Count > 1)
        {
            if(tmp[tmp.Count-2].position != target && tmp[tmp.Count-2].position != origin)
            {
                return tmp[tmp.Count - 2].position;
            }
            return Vector2.zero;
        }
        return Vector2.zero;
    }

    public List<Node> CalculatePath(Vector2 target, Vector2 origin)
    {
        List<Node> tmp = new List<Node>();
        List<Node> path = Path(target, origin);

        Node node = path[path.Count-1];
        tmp.Add(node);
        while(node.prev != null)
        {
            node = node.prev;
            tmp.Add(node);
        }
        return tmp;
    }


    public List<Node> Path(Vector2 target, Vector2 origin)
    {
        List<Node> path = new List<Node>();
        Node nodeOrigin = new Node();
        for(int i=0; i < nodeList.Count;i++)
        {
            nodeList[i].Reset();

            if (nodeList[i].position == origin)
                nodeOrigin = nodeList[i];
        }
        nodeOrigin.distance = 0;

        while(nodeList.Count != 0)
        {
            foreach(Node node in nodeOrigin.nearNodes)
            {
                if ((node.distance == -1 || node.distance > (nodeOrigin.distance + 1)) && !node.visited)
                {
                    node.distance = nodeOrigin.distance + 1;
                    path.Add(nodeOrigin);
                    node.prev = nodeOrigin;
                }
            }
            nodeOrigin.visited = true;

            int min = 0;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].distance != -1 && !nodeList[i].visited)
                {
                    if(min == 0)
                    {
                        min = nodeList[i].distance;
                        nodeOrigin = nodeList[i];
                    }
                    else if(min > nodeList[i].distance)
                    {
                        min = nodeList[i].distance;
                        nodeOrigin = nodeList[i];
                    }
                }                    
            }

            if (target == nodeOrigin.position)
            {
                path.Add(nodeOrigin);
                return path;
            }
        }
        return path;
	}
}
