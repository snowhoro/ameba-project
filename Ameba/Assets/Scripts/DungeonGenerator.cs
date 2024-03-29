﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
public class DungeonGenerator : MonoBehaviour
{
    public int maxRooms = 10;

    private int dungeonHeight = 80;
    private int dungeonWidth = 50;
    
    private int roomMinSize = 6;
    private int roomMaxSize = 12;

    public GameObject prefab_wall;
    public GameObject prefab_floor;
    public GameObject prefab_door;
	public GameObject prefab_stairs;
    public GameObject prefab_player;

    public enum TileType
    {
        None,
        Floor,
        Wall,
        Door
    }

    public enum Size
    {
        Small = 1,
        Medium,
        Large,
        Huge,
    }

    public enum TunnelType
    {
        Errant = 50,
        Normal = 70,
        Straight = 90
    }


    public Size dungeonSize;
    public Size roomSize;
    public TunnelType tunnelType;

    private GameObject player;
	private GameObject stairs;

    public List<GameObject> tileList;

    private Tile[,] dungeonMatrix;
    private Rect[] rooms;
    private List<Vector2> doors;
    private List<Vector2> itunnel;

    void Start()
    {
        GenerateDungeon();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.R))
        {
            GenerateDungeon();
            CameraFade.StartAlphaFade(Color.black, true, 1f, 2f);            
        }
        if (Input.GetKey(KeyCode.F))
        {
            CameraFade.StartAlphaFade(Color.black, false, 0.5f, 1f, () => { GenerateDungeon(); });            
        }
    }

    public void GenerateDungeon()
    {
        CleanMap();
        CalcSize();
        CreateDungeonMatrix();
        CreateRooms();
        Open_tunnel();
        DrawDungeon();
		SetWalls ();
		SetFloor();
        SetPlayer();
        Pathfinding.instance.LoadPathfinder(dungeonMatrix, dungeonWidth, dungeonHeight);
        GenerateEnemies();

        CameraFade.StartAlphaFade(Color.black, true, 0.5f, 1f);
    }

    private void CalcSize()
    {
        dungeonHeight *= (int)dungeonSize;
        dungeonWidth *= (int)dungeonSize;

        //print(dungeonHeight + " " + dungeonWidth);

        roomMaxSize *= (int)roomSize;
        roomMinSize *= (int)roomSize;

        //print(roomMaxSize + " " + roomMinSize);

        maxRooms = ((int)dungeonSize * maxRooms) / (int)roomSize;

        //print(maxRooms);
    }

    private void CreateDungeonMatrix()
    {
        dungeonMatrix = new Tile[dungeonWidth, dungeonHeight];
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                dungeonMatrix[x, y] = new Tile();
            }
        }
    }

    private void CreateRooms()
    {
        rooms = new Rect[maxRooms];
        doors = new List<Vector2>();

        for (int roomIndex = 0; roomIndex < maxRooms; )
        {
            int roomX, roomY, roomWidth, roomHeight;
            roomX = roomY = roomWidth = roomHeight = 0;

            roomWidth = Random.Range(roomMinSize, roomMaxSize);
            roomHeight = Random.Range(roomMinSize, roomMaxSize);

            roomX = Random.Range(2, dungeonWidth - roomWidth - 2);
            roomY = Random.Range(2, dungeonHeight - roomHeight - 2);

            Rect newRoom = new Rect(roomX, roomY, roomWidth + 1, roomHeight + 1);
            bool overlaps = false;

            foreach (Rect rect in rooms)
            {
                if (newRoom.Overlaps(rect))
                {
                    //print("Overlaps Room " + roomIndex + ": " + roomX + "," + roomY + " - " + roomWidth + "," + roomHeight);
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                //print("Room " + roomIndex + ": " + roomX + "," + roomY + " - " + roomWidth + "," + roomHeight);
                rooms[roomIndex] = newRoom;
                PlaceRoom(newRoom);
                AddDoors(newRoom);
                roomIndex++;
            }
        }
    }

    private void PlaceRoom(Rect rect)
    {
        for (int x = (int)rect.xMin; x < (int)rect.xMax - 1; x++)
        {
            for (int y = (int)rect.yMin; y < (int)rect.yMax - 1; y++)
            {
                if (x == rect.xMin || x == (rect.xMax - 2) || y == rect.yMin || y == (rect.yMax - 2))
                {
                    dungeonMatrix[x, y].type = (short)TileType.Wall;
                }
                else
                {
                    dungeonMatrix[x, y].type = (short)TileType.Floor;
                }
            }
        }
    }

    private void AddDoors(Rect rect)
    {
        int numDoors = Random.Range(2, 4);
        for (int i = 0; i < numDoors; )
        {
            int x = (int)Random.Range(rect.xMin, rect.xMax - 1);
            int y = (int)Random.Range(rect.yMin, rect.yMax - 1);

            if ((x == rect.xMin || x == (rect.xMax - 2) || y == rect.yMin || y == (rect.yMax - 2))
                && !IsCorner(rect, x, y) && !IsNextTo(TileType.Door,x, y))
            {
                dungeonMatrix[x, y].type = (short)TileType.Door;
                doors.Add(new Vector2(x, y));
                i++;
            }
        }
    }

    private bool IsCorner(Rect rect, int x, int y)
    {
        if (rect.min.Equals(new Vector2(x, y))
            || (x == rect.xMax - 2 && y == rect.yMax - 2)
            || (x == rect.xMax - 2 && y == rect.yMin)
            || (x == rect.xMin && y == rect.yMax - 2)
           )
            return true;
        return false;
    }

    private bool IsNextTo(TileType search, int x, int y)
    {
        if (SearchFor(search,x,y)
            || SearchFor(search, x + 1, y)
            || SearchFor(search, x, y + 1)
            || SearchFor(search, x - 1, y)
            || SearchFor(search, x, y - 1)
           )
            return true;
        return false;
    }

    private bool SearchFor(TileType search, int x, int y) 
    {
        if (0 < x && x < dungeonWidth && 0 < y && y < dungeonHeight)
        {
            if (dungeonMatrix[x, y].type == (short)search)
                return true;
        }
        return false;
    }

    private bool SearchFor(TileType search, Vector2 vec)
    {
        if (0 < vec.x && vec.x < dungeonWidth -1 && 0 < vec.y && vec.y < dungeonHeight -2)
        {
            if (dungeonMatrix[(int)vec.x, (int)vec.y].type == (short)search)
                return true;
        }
        return false;
    }

    private bool IsFloor(Vector2 vec)
    {
        if (0 <= vec.x && vec.x < dungeonWidth -1 && 0 < vec.y && vec.y < dungeonHeight -2)
        {
            if (dungeonMatrix[(int)vec.x, (int)vec.y].type == (short)TileType.Floor 
                || dungeonMatrix[(int)vec.x, (int)vec.y].type == (short)TileType.Door)
                return true;
        }
        return false;
    }
    private bool IsWall(Vector2 vec)
    {
        if (0 <= vec.x && vec.x < dungeonWidth && 0 < vec.y && vec.y < dungeonHeight - 1)
        {
            if (dungeonMatrix[(int)vec.x, (int)vec.y].type == (short)TileType.Wall)
                return true;
        }
        return false;
    }

    private Vector2 begin_tunnel(Vector2 vec)
    {
        if (SearchFor(TileType.None, (int)vec.x + 1, (int)vec.y))
            return new Vector2(vec.x + 1, vec.y);
        else if (SearchFor(TileType.None, (int)vec.x - 1, (int)vec.y))
            return new Vector2(vec.x - 1, vec.y);
        else if (SearchFor(TileType.None, (int)vec.x, (int)vec.y + 1))
            return new Vector2(vec.x, vec.y + 1);
        else if (SearchFor(TileType.None, (int)vec.x, (int)vec.y - 1))
            return new Vector2(vec.x, vec.y - 1);
        else
            return Vector2.zero;
    }

    private void Open_tunnel()
    {
        foreach (Vector2 door in doors)
        {
            itunnel = new List<Vector2>();
            Vector2 tunnelStart = begin_tunnel(door);
            if (tunnelStart != Vector2.zero)
            {
                itunnel.Add(tunnelStart);
                s_Tunnel(tunnelStart, door);
            }
        }
    }

    private void s_Tunnel(Vector2 vec, Vector2 vecAnt)
    {
        Vector2 dir = vec - vecAnt;

        dungeonMatrix[(int)vec.x, (int)vec.y].type = (short)TileType.Floor;

        Vector2 nextDir = NextDir(vec, vecAnt, ref dir);
        if (nextDir == Vector2.zero)
            return;

        vecAnt = vec;
        vec += nextDir;

        dungeonMatrix[(int)vec.x, (int)vec.y].type = (short)TileType.Floor;

        itunnel.Add(vec);

        if (IsNextToFloor(vec, nextDir,dir))
            return;

        recursive_Tunnel(vec, vecAnt,ref dir);
    }
    
    private void recursive_Tunnel(Vector2 vec, Vector2 vecAnt,ref Vector2 dir)
    {
        if (IsNextTo(TileType.Door, (int)vec.x, (int)vec.y))
        {
            //print("NEXT TO DOOR");
            return;
        }

        Vector2 nextDir = NextDir(vec, vecAnt, ref dir);
        if (nextDir == Vector2.zero)
        {
            //print("NO MORE DIR " + vec);
            return;
        }

        vecAnt = vec;
        vec += nextDir;
        
        dungeonMatrix[(int)vec.x, (int)vec.y].type = (short)TileType.Floor;

        itunnel.Add(vec);

        if (IsNextToFloor(vec, nextDir,dir))
            return;
        recursive_Tunnel(vec, vecAnt,ref dir);
    }

    private bool IsNextToFloor(Vector2 vec, Vector2 nextDir, Vector2 dir)
    {
        int modif = 1;
        if (SearchFor(TileType.Floor, vec + nextDir))
        {
            //print("NEXT TO FLOOR");
            return true;
        }
        else if (nextDir.x == 0)
        {
            Vector2 tmpDir = new Vector2(modif, 0);
            if (SearchFor(TileType.Floor, vec + tmpDir))
                return true;
            else
            {
                tmpDir = new Vector2(modif * -1, 0);
                if (SearchFor(TileType.Floor, vec + tmpDir))
                    return true;
            }
        }
        else
        {
            Vector2 tmpDir = new Vector2(0, modif);
            if (SearchFor(TileType.Floor, vec + tmpDir))
                return true;
            else
            {
                tmpDir = new Vector2(0, modif * -1);
                if (SearchFor(TileType.Floor, vec + tmpDir))
                    return true;
            }
        }
        return false;
 
    }

    private Vector2 NextDir(Vector2 vec, Vector2 vecAnt, ref Vector2 dir)
    {

        int rnd = Random.Range(0, 100);
        Vector2 nextDir;


        if (IsOutOfBounds(vec + dir))
            dir = vec - vecAnt;

        if (rnd <= (int)tunnelType && SearchFor(TileType.None, vec + dir) && !IsNextToItself(vec, dir))
        {
            nextDir = dir;
        }
        else
        {
            rnd = Random.Range(0, 100);

            int modif = 0;
            if (rnd <= 50)
                modif = -1;
            else
                modif = 1;

            if (dir.x == 0)
                nextDir = new Vector2(modif, 0);
            else
                nextDir = new Vector2(0, modif);

            if (SearchFor(TileType.None, vec + nextDir) && !IsNextToItself(vec,nextDir))
            {
                return nextDir;
            }
            else
            {
                if (   (SearchFor(TileType.None, vec + dir) && !IsNextToItself(vec, dir))
                    || (SearchFor(TileType.None, vec + (nextDir * -1)) && !IsNextToItself(vec, nextDir * -1)))
                {
                    nextDir = NextDir(vec, vecAnt, ref dir);
                }
                else if (SearchFor(TileType.None, vec + (dir * -1)) && !IsNextToItself(vec, dir * -1))
                {
                    nextDir = dir * -1;
                }
                else
                    nextDir = Vector2.zero;
            }
        }
        return nextDir;
    }

    private bool IsOutOfBounds(Vector2 vec)
    {
        if (0 < vec.x && vec.x < dungeonWidth && 0 < vec.y && vec.y < dungeonHeight)
            return false;
        return true;
    }

    private bool IsNextToItself(Vector2 vec, Vector2 nextDir)
    {
        Vector2 tmp;
        if (nextDir.x == 0)
            tmp = new Vector2(1, 0);
        else
            tmp = new Vector2(0, 1);

        if (itunnel.Contains(vec + nextDir + tmp) || itunnel.Contains(vec + nextDir + (tmp * -1)))
            return true;
        return false;
    }

    private void DrawDungeon()
    {    
        tileList = new List<GameObject>();
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                //Vector2 translation = new Vector2();
                GameObject tileObject = null;
                if (dungeonMatrix[x, y].type == (short)TileType.Floor)
                {
                    tileObject = (GameObject)GameObject.Instantiate(prefab_floor, new Vector3(x, y, 2), Quaternion.identity);
                }
                else if (dungeonMatrix[x, y].type == (short)TileType.Wall || dungeonMatrix[x, y].type == (short)TileType.None)
                {
                    tileObject = (GameObject)GameObject.Instantiate(prefab_wall, new Vector3(x, y, 2), Quaternion.identity);
                    dungeonMatrix[x, y].blocked = true;
                }
                else if (dungeonMatrix[x, y].type == (short)TileType.Door)
                {
                    tileObject = (GameObject)GameObject.Instantiate(prefab_door, new Vector3(x, y, 2), Quaternion.identity);
                }

                tileList.Add(tileObject);
                tileObject.transform.parent = transform;
                /* SI CAMBIO EL TAMAÑO DE LOS TILES
                 * if (dungeonMatrix[x, y] != 0)
                {
                    translation.x = x;
                    translation.y = y;
                    tileObject.transform.Translate(translation);
                }*/
            }
        }
    }


	public GameObject floor_topLeftRight;
	public GameObject floor_botLeftRight;
	public GameObject floor_topBotLeft;
	public GameObject floor_topBotRight;

	public GameObject floor_cornerRightBot;
	public GameObject floor_cornerRightTop;	
	public GameObject floor_cornerLeftBot;
	public GameObject floor_cornerLeftTop;


	public GameObject floor_topBot;
	public GameObject floor_leftRight;
	public GameObject floor_topLeft;
	public GameObject floor_topRight;
	public GameObject floor_botRight;
	public GameObject floor_botLeft;

	public GameObject floor_left;
	public GameObject floor_right;
	public GameObject floor_bot;
	public GameObject floor_top;

    public GameObject wall_topLeftRight;
    public GameObject wall_botLeftRight;
    public GameObject wall_topBotLeft;
    public GameObject wall_topBotRight;
                      
    public GameObject wall_cornerRightBot;
    public GameObject wall_cornerRightTop;
    public GameObject wall_cornerLeftBot;
    public GameObject wall_cornerLeftTop;
                      
    public GameObject wall_topBot;
    public GameObject wall_leftRight;
    public GameObject wall_topLeft;
    public GameObject wall_topRight;
    public GameObject wall_botRight;
    public GameObject wall_botLeft;
                      
    public GameObject wall_left;
    public GameObject wall_right;
    public GameObject wall_bot;
    public GameObject wall_top;

    public GameObject[] prefab_wall2;

	
	public void SetWalls()
	{
		for (int x = 0; x < dungeonWidth; x++)
		{
			for (int y = 0; y < dungeonHeight; y++)
			{
				if( dungeonMatrix[x,y].type == (short)TileType.None )
					dungeonMatrix[x,y].type = (short)TileType.Wall;
			}
		}
	}

	public void SetFloor()
	{
		for (int x = 0; x < dungeonWidth; x++)
		{
			for (int y = 0; y < dungeonHeight; y++)
			{
                #region FloorTiles
                if (IsFloor(new Vector2(x,y)))
				{                    

					//CUATRO
					if(IsWall(new Vector2(x,y+1)) 
					   && IsWall(new Vector2(x,y-1))
					   && IsWall(new Vector2(x+1,y))
					   && IsWall(new Vector2(x-1,y)))
					{
						//NORMAL FLOOR
					}    
					// TRES 
					else if(IsWall(new Vector2(x,y+1)) 
					   && IsWall(new Vector2(x+1,y))
					   && IsWall(new Vector2(x-1,y)))
					{
						//TOP LEFT RIGHT
						tileList.Add((GameObject)Instantiate(floor_topLeftRight, new Vector2(x,y),Quaternion.identity));
					}
					else if( IsWall(new Vector2(x,y-1))
					 && IsWall(new Vector2(x+1,y))
					 && IsWall(new Vector2(x-1,y)))
					{
						//BOT LEFT RIGHT
						tileList.Add((GameObject)Instantiate(floor_botLeftRight, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x,y+1)) 
					   && IsWall(new Vector2(x,y-1))
					   && IsWall(new Vector2(x-1,y))
					   )
					{
						// TOP BOT LEFT
						tileList.Add((GameObject)Instantiate(floor_topBotLeft, new Vector2(x,y),Quaternion.identity));
					}
					else if(IsWall(new Vector2(x,y+1)) 
					   && IsWall(new Vector2(x,y-1))
					   && IsWall(new Vector2(x+1,y)))
					{
						//TOP BOT RIGHT
						tileList.Add((GameObject)Instantiate(floor_topBotRight, new Vector2(x,y),Quaternion.identity));
						
					}


					//DOS + ESQUINAS
					else if(IsWall(new Vector2(x,y+1)) 
					   && IsWall(new Vector2(x-1,y))
					   && IsWall(new Vector2(x+1,y-1)))
					{
						//TOP LEFT    CORNER RIGHT BOT
						tileList.Add((GameObject)Instantiate(floor_cornerRightBot, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x,y+1)) 
					        && IsWall(new Vector2(x+1,y))
					        && IsWall(new Vector2(x-1,y-1)))
					{
						//TOP RIGHT   CORNER LEFT BOT
						tileList.Add((GameObject)Instantiate(floor_cornerLeftBot, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x,y-1)) 
					        && IsWall(new Vector2(x+1,y))
					        && IsWall(new Vector2(x-1,y+1)))
					{
						//BOT RIGHT   CORNER LEFT TOP
						tileList.Add((GameObject)Instantiate(floor_cornerLeftTop, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x,y-1)) 
					        && IsWall(new Vector2(x-1,y))
					        && IsWall(new Vector2(x+1,y+1)))
					{
						//BOT LEFT   CORNER RIGHT TOP
						tileList.Add((GameObject)Instantiate(floor_cornerRightTop, new Vector2(x,y),Quaternion.identity));
						
					}



					//DOS
					else if(IsWall(new Vector2(x,y+1)) 
					        && IsWall(new Vector2(x,y-1)))
					{
						//TOP BOT
						tileList.Add((GameObject)Instantiate(floor_topBot, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x+1,y)) 
					        && IsWall(new Vector2(x-1,y)))
					{
						//LEFT RIGHT
						tileList.Add((GameObject)Instantiate(floor_leftRight, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x,y+1)) 
					        && IsWall(new Vector2(x-1,y)))
					{
						//TOP LEFT
						tileList.Add((GameObject)Instantiate(floor_topLeft, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x,y+1)) 
					        && IsWall(new Vector2(x+1,y)))
					{
						//TOP RIGHT
						tileList.Add((GameObject)Instantiate(floor_topRight, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x,y-1)) 
					        && IsWall(new Vector2(x+1,y)))
					{
						//BOT RIGHT
						tileList.Add((GameObject)Instantiate(floor_botRight, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x,y-1)) 
					        && IsWall(new Vector2(x-1,y)))
					{
						//BOT LEFT
						tileList.Add((GameObject)Instantiate(floor_botLeft, new Vector2(x,y),Quaternion.identity));
						
					}

					// UNO

					else if(IsWall(new Vector2(x,y+1)))
					{
						//TOP
						tileList.Add((GameObject)Instantiate(floor_top, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x,y-1)))
					{
						//BOT
						tileList.Add((GameObject)Instantiate(floor_bot, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x+1,y)))
					{
						//RIGHT
						tileList.Add((GameObject)Instantiate(floor_right, new Vector2(x,y),Quaternion.identity));
						
					}
					else if(IsWall(new Vector2(x-1,y)))
					{
						//LEFT
						tileList.Add((GameObject)Instantiate(floor_left, new Vector2(x,y),Quaternion.identity));
					}
                }

                #endregion
                #region WallTiles
                else if (dungeonMatrix[x, y].type == (short) TileType.Wall)
                {
                    //CUATRO
                    if (SearchFor(TileType.Floor, new Vector2(x, y + 1))
                       && SearchFor(TileType.Floor, new Vector2(x, y - 1))
                       && SearchFor(TileType.Floor, new Vector2(x + 1, y))
                       && SearchFor(TileType.Floor, new Vector2(x - 1, y)))
                    {
                        //NORMAL wall
                    }

					else if (IsFloor(new Vector2(x, y - 1)) /*&& !IsFloor(new Vector2(x, y + 1))*/)
                    {
                        //WALL
                        tileList.Add((GameObject)Instantiate(prefab_wall2[Random.Range(0,prefab_wall2.Length)], new Vector2(x,y),Quaternion.identity));

                    }



                    else if (IsWall( new Vector2(x, y - 1))
                            && IsFloor( new Vector2(x, y - 2))
                            && (IsFloor( new Vector2(x + 1, y))
                            || (IsWall( new Vector2(x + 1, y))
                            && IsFloor( new Vector2(x + 1, y -1))))
                            && (IsFloor( new Vector2(x - 1, y))
                            || (IsWall( new Vector2(x - 1, y))
                            && IsFloor( new Vector2(x - 1, y - 1)))))
                    {
                        // BOT LEFT RIGHT
                        tileList.Add((GameObject)Instantiate(wall_botLeftRight, new Vector2(x, y), Quaternion.identity));
                    }

                    else if ((IsFloor( new Vector2(x - 1, y))
                            || (IsWall( new Vector2(x - 1, y))
                            && IsFloor( new Vector2(x - 1, y - 1))))
                            && (IsFloor( new Vector2(x + 1, y))
                            || (IsWall( new Vector2(x + 1, y))
                            && IsFloor( new Vector2(x + 1, y - 1))))
                            && IsFloor( new Vector2(x, y + 1)))
                    {
                        //TOP LEFT RIGHT
                        tileList.Add((GameObject)Instantiate(wall_topLeftRight, new Vector2(x, y), Quaternion.identity));
                    }


                    else if (IsFloor( new Vector2(x, y + 1))
                            && (IsFloor(new Vector2(x - 1, y))
                            || (IsWall( new Vector2(x - 1, y))
                            && IsFloor(new Vector2(x - 1, y - 1))))
                            && IsWall( new Vector2(x, y - 1))
                            && IsFloor( new Vector2(x , y - 2))
                            )
                    {
                        //TOP BOT LEFT
                        tileList.Add((GameObject)Instantiate(wall_topBotLeft, new Vector2(x, y), Quaternion.identity));
                    }

                    else if (IsFloor( new Vector2(x, y + 1))
                            && (IsFloor(new Vector2(x + 1, y))
                            || (IsWall( new Vector2(x + 1, y))
                            && IsFloor(new Vector2(x + 1, y - 1))))
                            && IsWall( new Vector2(x, y - 1))
                            && IsFloor( new Vector2(x, y - 2))
                            )
                    {
                        //TOP BOT RIGHT
                        tileList.Add((GameObject)Instantiate(wall_topBotRight, new Vector2(x, y), Quaternion.identity));
                    }

                    else if (IsWall( new Vector2(x, y - 1))
                        && IsFloor( new Vector2(x, y - 2))
                        && IsFloor( new Vector2(x, y + 1)))
                    {
                        // BOT TOP
                        tileList.Add((GameObject)Instantiate(wall_topBot, new Vector2(x, y), Quaternion.identity));
                    }


                    else if ((IsFloor( new Vector2(x + 1, y))
                        || (IsWall( new Vector2(x + 1, y))
                        && IsFloor( new Vector2(x + 1, y - 1))))
                        && (IsFloor( new Vector2(x - 1, y))
                        || (IsWall( new Vector2(x - 1, y))
                        && IsFloor( new Vector2(x - 1, y - 1))))
                        )
                    {
                        // LEFT RIGHT
                        tileList.Add((GameObject)Instantiate(wall_leftRight, new Vector2(x, y), Quaternion.identity));
                    }


                    else if (IsWall( new Vector2(x, y - 1))
                        && IsFloor( new Vector2(x, y - 2))
                        && (IsFloor( new Vector2(x + 1, y))
                        || (IsWall( new Vector2(x + 1, y))
                        && IsFloor( new Vector2(x + 1, y - 1))))
                        )
                    {
                        // BOT RIGHT
                        tileList.Add((GameObject)Instantiate(wall_botRight, new Vector2(x, y), Quaternion.identity));
                    }

                    else if (IsWall( new Vector2(x, y - 1))
                        && IsFloor( new Vector2(x, y - 2))
                        && (IsFloor( new Vector2(x - 1, y))
                        || (IsWall( new Vector2(x - 1, y))
                        && IsFloor( new Vector2(x - 1, y - 1)))))
                    {
                        // BOT LEFT
                        tileList.Add((GameObject)Instantiate(wall_botLeft, new Vector2(x, y), Quaternion.identity));
                    }

                    else if (IsFloor( new Vector2(x, y + 1))
                        && (IsFloor( new Vector2(x + 1, y))
                        || (IsWall( new Vector2(x + 1, y))
                        && IsFloor( new Vector2(x + 1, y - 1)))))
                    {
                        // TOP RIGHT
                        tileList.Add((GameObject)Instantiate(wall_topRight, new Vector2(x, y), Quaternion.identity));
                    }

                    else if (IsFloor( new Vector2(x, y + 1))
                    && (IsFloor( new Vector2(x - 1, y))
                    || (IsWall( new Vector2(x - 1, y))
                    && IsFloor( new Vector2(x - 1, y - 1)))))
                    {
                        // TOP LEFT
                        tileList.Add((GameObject)Instantiate(wall_topLeft, new Vector2(x, y), Quaternion.identity));
                    }


                    //// UNO
                    else if (IsWall( new Vector2(x, y - 1))
                    && IsFloor( new Vector2(x, y - 2)))
                    {
                        // BOT 
                        tileList.Add((GameObject)Instantiate(wall_bot, new Vector2(x, y), Quaternion.identity));
                    }
                    else if (IsWall( new Vector2(x + 1 , y))
                    && IsFloor( new Vector2(x +1, y - 1 )))
                    {
                        //RIGHT
                        tileList.Add((GameObject)Instantiate(wall_right, new Vector2(x, y), Quaternion.identity));
                    }

                    else if (IsWall( new Vector2(x - 1, y))
                             && IsFloor( new Vector2(x - 1, y - 1)))
                    {
                        //LEFT
                        tileList.Add((GameObject)Instantiate(wall_left, new Vector2(x, y), Quaternion.identity));
                    }



                    // TRES 
                    else if (IsFloor( new Vector2(x, y + 1))
                       && IsFloor( new Vector2(x + 1, y))
                       && IsFloor( new Vector2(x - 1, y)))
                    {
                        //TOP LEFT RIGHT
                        tileList.Add((GameObject)Instantiate(wall_topLeftRight, new Vector2(x, y), Quaternion.identity));
                    }
                    else if (IsFloor( new Vector2(x, y - 1))
                     && IsFloor( new Vector2(x + 1, y))
                     && IsFloor( new Vector2(x - 1, y)))
                    {
                        //BOT LEFT RIGHT
                        tileList.Add((GameObject)Instantiate(wall_botLeftRight, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x, y + 1))
                       && IsFloor( new Vector2(x, y - 1))
                       && IsFloor( new Vector2(x - 1, y))
                       )
                    {
                        // TOP BOT LEFT
                        tileList.Add((GameObject)Instantiate(wall_topBotLeft, new Vector2(x, y), Quaternion.identity));
                    }
                    else if (IsFloor( new Vector2(x, y + 1))
                       && IsFloor( new Vector2(x, y - 1))
                       && IsFloor( new Vector2(x + 1, y)))
                    {
                        //TOP BOT RIGHT
                        tileList.Add((GameObject)Instantiate(wall_topBotRight, new Vector2(x, y), Quaternion.identity));

                    }



                    //DOS + ESQUINAS
                    else if (IsFloor( new Vector2(x, y + 1))
                       && IsFloor( new Vector2(x - 1, y))
                       && IsFloor( new Vector2(x + 1, y - 1)))
                    {
                        //TOP LEFT    CORNER RIGHT BOT
                        tileList.Add((GameObject)Instantiate(wall_cornerRightBot, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x, y + 1))
                            && IsFloor( new Vector2(x + 1, y))
                            && IsFloor( new Vector2(x - 1, y - 1)))
                    {
                        //TOP RIGHT   CORNER LEFT BOT
                        tileList.Add((GameObject)Instantiate(wall_cornerLeftBot, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x, y - 1))
                            && IsFloor( new Vector2(x + 1, y))
                            && IsFloor( new Vector2(x - 1, y + 1)))
                    {
                        //BOT RIGHT   CORNER LEFT TOP
                        tileList.Add((GameObject)Instantiate(wall_cornerLeftTop, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x, y - 1))
                            && IsFloor( new Vector2(x - 1, y))
                            && IsFloor( new Vector2(x + 1, y + 1)))
                    {
                        //BOT LEFT   CORNER RIGHT TOP
                        tileList.Add((GameObject)Instantiate(wall_cornerRightTop, new Vector2(x, y), Quaternion.identity));

                    }





                    //DOS
                    else if (IsFloor( new Vector2(x, y + 1))
                            && IsFloor( new Vector2(x, y - 1)))
                    {
                        //TOP BOT
                        tileList.Add((GameObject)Instantiate(wall_topBot, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x + 1, y))
                            && IsFloor( new Vector2(x - 1, y)))
                    {
                        //LEFT RIGHT
                        tileList.Add((GameObject)Instantiate(wall_leftRight, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x, y + 1))
                            && IsFloor( new Vector2(x - 1, y)))
                    {
                        //TOP LEFT
                        tileList.Add((GameObject)Instantiate(wall_topLeft, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x, y + 1))
                            && IsFloor( new Vector2(x + 1, y)))
                    {
                        //TOP RIGHT
                        tileList.Add((GameObject)Instantiate(wall_topRight, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x, y - 1))
                            && IsFloor( new Vector2(x + 1, y)))
                    {
                        //BOT RIGHT
                        tileList.Add((GameObject)Instantiate(wall_botRight, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x, y - 1))
                            && IsFloor( new Vector2(x - 1, y)))
                    {
                        //BOT LEFT
                        tileList.Add((GameObject)Instantiate(wall_botLeft, new Vector2(x, y), Quaternion.identity));

                    }

                    // UNO

                    else if (IsFloor( new Vector2(x, y + 1)))
                    {
                        //TOP
                        tileList.Add((GameObject)Instantiate(wall_top, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x, y - 1)))
                    {
                        //BOT
                        tileList.Add((GameObject)Instantiate(wall_bot, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x + 1, y)))
                    {
                        //RIGHT
                        tileList.Add((GameObject)Instantiate(wall_right, new Vector2(x, y), Quaternion.identity));

                    }
                    else if (IsFloor( new Vector2(x - 1, y)))
                    {
                        //LEFT
                        tileList.Add((GameObject)Instantiate(wall_left, new Vector2(x, y), Quaternion.identity));
                    }
                }
                #endregion
            }
		}
	}

    public void SetPlayer()
    {
        if (player != null)
        {
            player.transform.position = new Vector3((int)rooms[0].center.x, (int)rooms[0].center.y, -1);
            player.GetComponent<Player>().target = player.transform.position;
            player.GetComponent<Player>().Position = player.transform.position;
        }
        else 
        {
            player = (GameObject)GameObject.Instantiate(prefab_player, new Vector3((int)rooms[0].center.x, (int)rooms[0].center.y, -1), Quaternion.identity);
        }

        stairs = (GameObject)GameObject.Instantiate(prefab_stairs, new Vector3((int)rooms[rooms.Length - 1].center.x, (int)rooms[rooms.Length - 1].center.y, -1), Quaternion.identity);

    }

    //public bool IsBlocked(Vector2 vec)
    //{
    //    if (0 < vec.x && vec.x < dungeonWidth && 0 < vec.y && vec.y < dungeonHeight) 
    //    {
    //        return dungeonMatrix[(int) vec.x , (int) vec.y ].blocked;
    //    }
    //    return true;
    //}

    public void GenerateEnemies()
    {
       EnemyGenerator enemGen = GameObject.FindGameObjectWithTag("Enemies").GetComponent<EnemyGenerator>();
       enemGen.GenerateEnemies(rooms);
    }

    private void CleanMap()
    {
        GameObject.FindGameObjectWithTag("Enemies").GetComponent<EnemyGenerator>().DeleteEnemies();
        Turns.instance.CleanTurnList();
		Destroy (stairs);

        if (tileList != null)
        {
            foreach (GameObject tile in tileList)
                Destroy(tile);
        }
    }
}
