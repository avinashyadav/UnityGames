using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

    public int ROWS = 8;

    public int COLUMNS = 6;

    public float TILE_SIZE = 0.68f;

    public GameObject gridBallGO;

    [HideInInspector]
    public float GRID_OFFSET_X = 0;

    [HideInInspector]
    public float GRID_OFFSET_Y = 0;


    [HideInInspector]
    public List<List<Ball>> gridBalls;

    void Start()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        gridBalls = new List<List<Ball>>();


        GRID_OFFSET_X = (COLUMNS * TILE_SIZE) * 0.5f;
        GRID_OFFSET_Y = (ROWS * TILE_SIZE) * 0.5f;

        Debug.Log(GRID_OFFSET_X);
        Debug.Log(GRID_OFFSET_Y);


        GRID_OFFSET_X -= TILE_SIZE * 0.5f;
        GRID_OFFSET_Y -= TILE_SIZE * 0.5f;

        for (int column = 0; column < COLUMNS; column++)
        {

            var columnBalls = new List<Ball>();

            for (int row = 0; row < ROWS; row++)
            {


                var item = Instantiate(gridBallGO) as GameObject;
                var ball = item.GetComponent<Ball>();

                ball.SetBallPosition(this, column, row);
                ball.transform.parent = gameObject.transform;
                columnBalls.Add(ball);
            }
            gridBalls.Add(columnBalls);
        }

    }

}