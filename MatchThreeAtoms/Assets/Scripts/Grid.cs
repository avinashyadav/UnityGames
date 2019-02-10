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
    private int NO_OF_BALL_TYPES = 5;

    [HideInInspector]
    public float GRID_OFFSET_X = 0;

    [HideInInspector]
    public float GRID_OFFSET_Y = 0;

    [HideInInspector]
    public List<List<Ball>> gridBalls;

    private List<Ball> matchList;

    private int numberOfTurns = 1;

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
                ball.SetType((Ball.BALL_TYPE)Random.Range(0, NO_OF_BALL_TYPES));

                ball.transform.parent = gameObject.transform;
                columnBalls.Add(ball);
            }
            gridBalls.Add(columnBalls);
        }
    }

    public void CheckMatchesForBall(Ball ball)
    {
        matchList.Clear();

        for(int column = 0; column < COLUMNS; ++column)
        {
            for(int row = 0; row < ROWS; ++row)
            {
                gridBalls[column][row].visited = false;
            }
        }

        //search for matches around the ball
        var initialResult = GetMatches(ball);
        matchList.AddRange(initialResult);

        while(true)
        {
            var allVisited = true;
            for(var i = matchList.Count - 1; i >= 0; --i)
            {
                var b = matchList[i];
                if(!b.visited)
                {
                    AddMatches(GetMatches(b));
                    allVisited = false;
                }
            }

            if(allVisited)
            {
                if(matchList.Count > 2)
                {
                    CollapseGrid();
                }
                return;
            }
        }

    }

    List<Ball> GetMatches(Ball ball)
    {
        ball.visited = true;
        var result = new List<Ball>() { ball };

        //+column
        if(ball.column + 1 < COLUMNS)
        {
            for(var r = -1; r <= 1; ++r)
            {
                if(ball.row + r >= 0 && ball.row + r < ROWS)
                {
                    if(DoTypesMatch(gridBalls[ball.column + 1][ball.row + r].type, ball.type))
                    {
                        result.Add(gridBalls[ball.column + 1][ball.row + r]);
                    }
                    ++r;
                }
            }
        }

        //-column
        if (ball.column - 1 >= 0)
        {
            for (var r = -1; r <= 1; ++r)
            {
                if (ball.row + r >= 0 && ball.row + r < ROWS)
                {
                    if (DoTypesMatch(gridBalls[ball.column - 1][ball.row + r].type, ball.type))
                    {
                        result.Add(gridBalls[ball.column - 1][ball.row + r]);
                    }
                    ++r;
                }
            }
        }

        //top
        if(ball.row - 1 >= 0)
        {
            if(DoTypesMatch(gridBalls[ball.column][ball.row - 1].type, ball.type))
            {
                result.Add(gridBalls[ball.column][ball.row - 1]);
            }
        }

        //bottom
        if (ball.row + 1 < ROWS)
        {
            if (DoTypesMatch(gridBalls[ball.column][ball.row + 1].type, ball.type))
            {
                result.Add(gridBalls[ball.column][ball.row + 1]);
            }
        }

        return result;
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

    bool DoTypesMatch(Ball.BALL_TYPE t1, Ball.BALL_TYPE t2)
    {
        return t1 == t2;

        //return
        //    (t1 == Ball.BALL_TYPE.TYPE_1 && t2 == Ball.BALL_TYPE.TYPE_1) || //H2
        //    (t1 == Ball.BALL_TYPE.TYPE_1 && t2 == Ball.BALL_TYPE.TYPE_3) || //LiH
        //    (t1 == Ball.BALL_TYPE.TYPE_3 && t2 == Ball.BALL_TYPE.TYPE_1) || //LiH
        //    (t1 == t2 && (numberOfTurns % 5 == 0 || t1 != Ball.BALL_TYPE.TYPE_2));
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
        while(stepR - 1 >= 0 && DoTypesMatch(gridBalls[c][stepR - 1].type, type))
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
        while(stepR + 1 < ROWS && DoTypesMatch(gridBalls[c][stepR+1].type, type))
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
        while(stepC - 1 >= 0 && DoTypesMatch(gridBalls[stepC - 1][r].type, type))
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
        while(stepC + 1 < COLUMNS && DoTypesMatch(gridBalls[stepC+1][r].type, type))
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
        var type = Random.Range(0, NO_OF_BALL_TYPES);
        var ballType = (Ball.BALL_TYPE)type;

        if (row - 2 >= 0 && gridBalls[col][row - 1].type == ballType && gridBalls[col][row - 2].type == ballType)
        {
            type = (type + 1) % NO_OF_BALL_TYPES;
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

            numberOfTurns = (numberOfTurns + 1) % 5;
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
                    ball.SetType((Ball.BALL_TYPE) Random.Range(0, NO_OF_BALL_TYPES));
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
            GameView.PAUSED = false;
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