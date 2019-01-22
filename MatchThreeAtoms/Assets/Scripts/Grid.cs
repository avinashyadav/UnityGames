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
    private int NO_OF_BALLS = 5;

    [HideInInspector]
    public float GRID_OFFSET_X = 0;

    [HideInInspector]
    public float GRID_OFFSET_Y = 0;

    [HideInInspector]
    public List<List<Ball>> gridBalls;

    private List<Ball> matchList;

    private List<Ball> collapseTweens;

    void Start()
    {
        matchList = new List<Ball>();

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

    public bool GridHasMatches()
    {
        matchList.Clear();

        for(var c = 0; c < COLUMNS; ++c)
        {
            for(var r = 0; r < ROWS; ++r)
            {
                CheckTypeMatch(c, r);
            }
        }

        if(matchList.Count > 2)
        {
            foreach(var b in matchList)
            {
                b.gameObject.SetActive(false);
                //b.Opaque();
            }
            return true;
        }
        return false;
    }

    void CheckTypeMatch(int c, int r)
    {
        if(matchList.Contains(gridBalls[c][r]))
        {
            return;
        }

        var type = gridBalls[c][r].type;
        var stepC = c;
        var stepR = r;
        var tempMatches = new List<Ball>();

        //check top
        while(stepR - 1 >= 0 && gridBalls[c][stepR - 1].type == type)
        {
            --stepR;
            tempMatches.Add(gridBalls[c][stepR]);
        }

        if(tempMatches.Count > 1)
        {
            tempMatches.Add(gridBalls[c][r]);
            AddMatches(tempMatches);
        }

        tempMatches.Clear();

        //check bottom
        stepR = r;
        while(stepR + 1 < ROWS && gridBalls[c][stepR+1].type == type)
        {
            ++stepR;
            tempMatches.Add(gridBalls[c][stepR]);
        }

        if (tempMatches.Count > 1)
        {
            tempMatches.Add(gridBalls[c][r]);
            AddMatches(tempMatches);
        }

        tempMatches.Clear();

        //check left
        while(stepC - 1 >= 0 && gridBalls[stepC - 1][r].type == type)
        {
            --stepC;
            tempMatches.Add(gridBalls[stepC][r]);
        }

        if (tempMatches.Count > 1)
        {
            tempMatches.Add(gridBalls[c][r]);
            AddMatches(tempMatches);
        }

        tempMatches.Clear();

        //check right
        stepC = c;
        while(stepC + 1 < COLUMNS && gridBalls[stepC+1][r].type == type)
        {
            ++stepC;
            tempMatches.Add(gridBalls[stepC][r]);
        }

        if (tempMatches.Count > 2)
        {
            tempMatches.Add(gridBalls[c][r]);
            AddMatches(tempMatches);
        }
    }

    void AddMatches(List<Ball> matches)
    {
        foreach(var b in matches)
        {
            if (!matchList.Contains(b))
            {
                matchList.Add(b);
            }
        }
    }

    Ball.BALL_TYPE GetVerticalUnique(int col, int row)
    {
        var type = Random.Range(0, NO_OF_BALLS);
        var ballType = (Ball.BALL_TYPE)type;

        if (row - 2 >= 0 && gridBalls[col][row - 1].type == ballType && gridBalls[col][row - 2].type == ballType)
        {
            type = (type + 1) % NO_OF_BALLS;
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
        if (GridHasMatches())
        {
            collapseTweens = new List<Ball>();

            foreach(var ball in matchList)
            {
                ball.gameObject.SetActive(false);
            }

            for(var c = 0; c < COLUMNS; ++c)
            {
                CollapseColumn(gridBalls[c]);
            }
        }
        else
        {
            GameView.PAUSED = false;
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
                    ball.SetType((Ball.BALL_TYPE) Random.Range(0, NO_OF_BALLS));
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

        for(var i = column.Count - 1; i >= 0; --i)
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