using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

//Legend
// x - unused space
// w - wall
// c - wall candidate 

public class DungeonGenerator : MonoBehaviour {

    public int width = 50;
    public int height = 50;

    private char[,] map;

    void Start ()
    {
        GenerateMap();
    }
	
    void GenerateMap()
    {
        map = new char[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = 'x';
            }
        }

        //Load and add starting room
        var startRoom = FileToArray("Assets/Resources/StartingRoom.txt");
        ForeachChar(startRoom, (x, y) => map[x, y] = startRoom[x, y]);

        //Calculate "BEAN BAG"
        TilesOfType(map, 'w', (x, y) => 
        {
            if (GoodCandidate(map, x, y))
            {
                map[x, y] = 'c';
            }
        });

        var randomRoom = FileToArray("Assets/Resources/Room1.txt");
    }

    void TilesOfType(char[,] array, char testedChar, System.Action<int, int> action)
    {
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                if (array[x,y] == testedChar)
                {
                    action(x, y);
                }
            }
        }
    }

    bool GoodCandidate(char[,] array, int x, int y)
    {
        bool horizontalNeighboursAvailable = 0 < x && x < width - 1;
        if (horizontalNeighboursAvailable)
        {
            bool haveWallNeighbours = array[x - 1, y] == 'w' && array[x + 1, y] == 'w';
            if (haveWallNeighbours)
            {
                bool unusedSpaceUp = y < height - 1 && array[x, y + 1] == 'x';
                if (unusedSpaceUp)
                {
                    return true;
                }

                bool unusedSpaceDown = 0 < y && array[x, y - 1] == 'x';
                if (unusedSpaceDown)
                {
                    return true;
                }
            }
        }

        bool verticalNeighboursAvailable = 0 < y && y < height - 1;
        if (verticalNeighboursAvailable)
        {
            bool haveWallNeighbours = array[x, y - 1] == 'w' && array[x, y + 1] == 'w';
            if (haveWallNeighbours)
            {
                bool unusedSpaceRight = x < width - 1 && array[x + 1, y] == 'x';
                if (unusedSpaceRight)
                {
                    return true;
                }

                bool unusedSpaceLeft = 0 < x && array[x - 1, y] == 'x';
                if (unusedSpaceLeft)
                {
                    return true;
                }
            }
        }
        return false;
    }

    List<Tuple<int, int>> GetNeighbours(char[,] array, int x, int y)
    {
        var result = new List<Tuple<int, int>>();
        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    result.Add(new Tuple<int, int>(neighbourX, neighbourY));
                }
            }
        }
        return result;
    }

    void ForeachNeighbour(char[,] array, int x, int y, System.Action<int, int> action)
    {
        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    action(x, y);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (map[x, y])
                    {
                        case 'c':
                            Gizmos.color = Color.red;
                            break;
                        case 'w':
                            Gizmos.color = Color.black;
                            break;

                        default:
                            Gizmos.color = Color.white;
                            break;
                    }
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, -height / 2 + y + .5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

    void ForeachChar(char[,] array, System.Action<int, int> action)
    {
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                action(x, y);
            }
        }
    }

    char[,] FileToArray(string path)
    {
        StreamReader reader = new StreamReader("Assets/Resources/StartingRoom.txt");
        var allLines = reader.ReadToEnd().Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        reader.Close();
        char[,] result = new char[allLines[0].Length, allLines.Length];
        for (int y = 0; y < allLines.Length; y++)
        {
            for (int x = 0; x < allLines[y].Length; x++)
            {
                result[x, y] = allLines[y][x];
            }
        }
        return result;
    }
    char[,] SurroundWithWalls(char[,] array)
    {
        var result = new char[array.GetLength(0) + 2, array.GetLength(1) + 2];

        for (int x = 0; x < result.GetLength(0); x++)
        {
            for (int y = 0; y < result.GetLength(1); y++)
            {
                bool isBorderTile = x == 0 || x == result.GetLength(0) - 1 || y == 0 || y == result.GetLength(1) - 1;
                result[x, y] = isBorderTile ? 'w' : array[x - 1, y - 1];
            }
        }

        return result;
    }
}
