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

    public void LoadPathfinder(Tile[,] dungeonMatrix, int dungeonWidth, int dungeonHeight)
    {
        nodeList = new List<Node>();
        LoadNodes(dungeonMatrix,dungeonWidth,dungeonHeight);
    }

    public void LoadNodes(Tile[,] dungeonMatrix, int dungeonWidth,int dungeonHeight)
    {
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                if (!dungeonMatrix[x, y].blocked)
                {
                    Node node = new Node();
                    node.position = new Vector2(x, y);
                    nodeList.Add(node);
                }
            }
        }

        foreach (Node node in nodeList)
        {
            node.nearNodes = new List<Node>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].position == node.position + new Vector2(1, 0))
                    node.nearNodes.Add(nodeList[i]);
                if (nodeList[i].position == node.position + new Vector2(-1, 0))
                    node.nearNodes.Add(nodeList[i]);
                if (nodeList[i].position == node.position + new Vector2(0, 1))
                    node.nearNodes.Add(nodeList[i]);
                if (nodeList[i].position == node.position + new Vector2(0, -1))
                    node.nearNodes.Add(nodeList[i]);
            }
        }
    }

    public bool CheckEmpty(Vector2 target)
    {
        Node nodoTarget = new Node();
        foreach(Node node in nodeList)
        {
            if (node.position == target)
                nodoTarget = node;
        }

        foreach(Node node in nodoTarget.nearNodes)
        {
            if (!node.visited)
                return true;
        }
        return false;
    }


    public Vector2 NextStep(Vector2 target, Vector2 origin)
    {
        //Node nodeOrigin = new Node();  
        //for (int i = 0; i < nodeList.Count; i++)
        //{
        //    nodeList[i].Reset();

        //    if (nodeList[i].position == origin)
        //        nodeOrigin = nodeList[i];
        //}
        //nodeOrigin.distance = 0;

        //LoadEnemiesPosition(GameObject.FindGameObjectWithTag("Enemies").GetComponent<EnemyGenerator>().enemies);

        //if (!CheckEmpty(target))
        //{
        //    print("TARGET");
        //    return Vector2.zero; 
        //}

        //if (!CheckEmpty(origin))
        //{
        //    print("ORIGIN");
        //    return Vector2.zero;
        //}


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
        if (path.Count != 0)
        {
            Node node = path[path.Count - 1];
            tmp.Add(node);
            while (node.prev != null)
            {
                node = node.prev;
                tmp.Add(node);
            }
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

        LoadEnemiesPosition(GameObject.FindGameObjectWithTag("Enemies").GetComponent<EnemyGenerator>().enemies);

        int attemp = 50;
        while (attemp != 0)
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

            attemp--;
        }
        path.Clear();
        return path;
	}

    public void LoadEnemiesPosition(List<Base> enemies)
    {
        foreach(Base enemy in enemies)
        {
            foreach(Node node in nodeList)
            {
                if (enemy.Position == node.position)
                {
                    node.visited = true;
                }
            }
        }
    }
}
