using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRender : MonoBehaviour
{
    [SerializeField] MazeGenerator mazegenerator;
    [SerializeField] GameObject MazeCellPrefab;

    // This is the physical size of out maze cells. getting this wrong will result in overlapping
    // or visible gaps between each cell.

    public float CellSize = 1f;

    private void Start()
    {
        // Get out Mazegenerator script to make us a maze
        MazeCell[,] maze = mazegenerator.GetMaze();

        // Loop through every cell in the maze.
        for(int x = 0; x < mazegenerator.mazeWidth; x++)
        {
            for(int y = 0; y < mazegenerator.mazeHeight; y++)
            {
                // Instantiate a new maze cell prefab as a child of the MazeRenderer object.
                GameObject newCell = Instantiate(MazeCellPrefab, new Vector3((float)x * CellSize, 0f, (float)y * CellSize), Quaternion.identity, transform);

                // Get a reference to the cell's MazeCellPrefab script.
                MazeCellObject mazeCell = newCell.GetComponent<MazeCellObject>();

                //Determine which walls need to be active
                bool top = maze[x,y].topWall;
                bool left = maze[x,y].leftWall;

                // Determine which walls are deactivated by default unless we are at the bottom or right
                // edge of the maze
                bool right = false;
                bool bottom = false;
                bool endpoint = false;
                if (x == mazegenerator.mazeWidth - 1) right = true; 
                if (y == 0) bottom = true;

                mazeCell.Init(top, bottom, right, left, endpoint);

                
                if(x == mazegenerator.mazeWidth -1 && y == mazegenerator.mazeHeight - 1)
                {
                    endpoint = true;
                    mazeCell.InitEndPoint(endpoint);
                }
            }
        }
    }
}
