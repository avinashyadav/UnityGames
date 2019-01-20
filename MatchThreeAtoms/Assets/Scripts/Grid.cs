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

    private List<Ball> collapseTweens;

    void Start()
    {
        //matchList = new List<Ball>();
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

        //build unique grid: no matches
        for(var c = 0; c < COLUMNS; ++c)
        {
            for(var r = 0; r < ROWS; ++r)
            {
                if(c < 3)
                {
                    gridBalls[c][r].SetType(GetVerticalUnique(c, r));
                }
                else
                {
                    gridBalls[c][r].SetType(GetVerticalHorizontalUnique(c, r));
                }
            }
        }
    }

    void MakeGridUnique()
    {
        for (int column = 0; column < COLUMNS; ++column)
        {
            for (int row = 0; row < ROWS; ++row)
            {
                var ball = gridBalls[column][row];
                ball.SetType(Ball.BALL_TYPE.NONE);
            }
        }

        for(var c = 0; c < COLUMNS; ++c)
        {
            for(var r = 0; r < ROWS; ++r)
            {
                if(c < 2)
                {
                    gridBalls[c][r].SetType(GetVerticalUnique(c, r));
                }
                else
                {
                    gridBalls[c][r].SetType(GetVerticalHorizontalUnique(c, r));
                }
            }
        }
    }

    Ball.BALL_TYPE GetVerticalUnique(int col, int row)
    {
        var type = Random.Range(0, 20);
        var ballType = (Ball.BALL_TYPE)type;

        if (row - 2 >= 0 && gridBalls[col][row - 1].type == ballType && gridBalls[col][row - 2].type == ballType)
        {
            type = (type + 1) % 20;
        }

        return (Ball.BALL_TYPE)type;
    }

    Ball.BALL_TYPE GetVerticalHorizontalUnique(int col, int row)
    {
        var type = GetVerticalUnique(col, row);

        if(gridBalls[col - 1][row].type == type && gridBalls[col - 2][row].type == type)
        {
            var unique = false;
            while (!unique)
            {
                type = GetVerticalUnique(col, row);
                if(gridBalls[col - 1][row].type == type && gridBalls[col - 2][row].type == type)
                {

                }
                else
                {
                    unique = true;
                }
            }
        }

        return type;
    }
    
    public void CollapseGrid()
    {
        collapseTweens = new List<Ball>();

        for(var c = 0; c < COLUMNS; ++c)
        {
            CollapseColumn(gridBalls[c]);
        }
    }

    private void CollapseColumn(List<Ball> column)
    {
        var index = 0;

        for(var i = column.Count - 1; i >= 0; --i)
        {
            var ball = column[i];

            if (!ball.gameObject.activeSelf)
            {
                //find next visible
                var newIndex = NextVisibleBall(column, i);
                Debug.Log(newIndex);
                if(newIndex == -1)
                {
                    index--;
                    ball.SetType((Ball.BALL_TYPE) Random.Range(0, 20));
                    ball.SetTempPosition(index);
                }
                else
                {
                    var nextBall = column[newIndex];
                    ball.MatchBallType(nextBall);
                    ball.SetTempPosition(newIndex);
                    nextBall.gameObject.SetActive(false);
                }
                collapseTweens.Add(ball);
            }
        }

        for(var i =column.Count - 1; i >= 0; --i)
        {
            var ball = column[i];
            ball.Drop(CollapseTweenDone);
        }
    }

    private void CollapseTweenDone(Ball ball)
    {
        if (collapseTweens.Contains(ball))
        {
            collapseTweens.Remove(ball);
        }

        //are we done with all collapses?
        if(collapseTweens.Count == 0)
        {
            Debug.Log("tween done");
            CollapseGrid();
        }
    }

    private int NextVisibleBall(List<Ball> column, int last)
    {
        if(last - 1 < 0)
        {
            return -1;
        }

        for(var i = last - 1; i >= 0; --i)
        {
            var ball = column[i];
            if (ball.gameObject.activeSelf)
            {
                return i;
            }
        }
        return -1;
    }
}