#define EIGHT_DIRECTIONAL

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameView : MonoBehaviour
{
    public static bool PAUSED = false;

    [HideInInspector]
    public Ball selectedBall;

    [HideInInspector]
    public float SCREEN_WIDTH = 6.6f;

    [HideInInspector]
    public float SCREEN_HEIGHT = 10.2f;

    public GameObject thumb;

    public Grid grid;

    private Vector2 worldViewTouch;

    private Ball targetBall;

    private List<Ball> selectedBalls;

    private static float SIN_45 = Mathf.Sin(Mathf.PI * 0.25f);

    void Start()
    {
        selectedBall = null;
        selectedBalls = new List<Ball>();
    }

    public void HandleTouchDown(Vector2 touch)
    {
        if(PAUSED)
        {
            return;
        }

        foreach(var b in selectedBalls)
        {
            b.ClearLine();
        }

        selectedBalls.Clear();

        this.worldViewTouch = Camera.main.ScreenToWorldPoint(touch);

        var ball = BallCloseToPoint(touch);

        if (ball != null)
        {
            selectedBall = ball;
            selectedBalls.Add(ball);
        }
    }

    public void HandleTouchUp(Vector2 touch)
    {
        if (PAUSED)
        {
            return;
        }

        if(selectedBall == null)
        {
            return;
        }

        selectedBall.ClearLine();
        selectedBall = null;

        if(selectedBalls.Count > 2)
        {
            grid.CollapseGrid(selectedBalls);
        }

        foreach(var b in selectedBalls)
        {
            b.ClearLine();
        }

        selectedBalls.Clear();
    }

    public void HandleTouchMove(Vector2 touch)
    {
        if(PAUSED)
        {
            return;
        }

        this.worldViewTouch = Camera.main.ScreenToWorldPoint(touch);

        if(selectedBall == null)
        {
            return;
        }

        var nextBall = BallCloseToPoint(touch);

        if(nextBall != null && nextBall != selectedBall && nextBall.touched == true && nextBall.type == selectedBall.type && IsValidTarget(nextBall))
        {
            if(!selectedBalls.Contains(nextBall))
            {
                selectedBalls.Add(nextBall);
                selectedBall = nextBall;
            }
        }

        DrawSelection();
    }

    private void DrawSelection()
    {
        for(var i = 0; i < selectedBalls.Count; ++i)
        {
            var b = selectedBalls[i];
            b.ClearLine();
            if(selectedBalls.Count == 1)
            {
                b.DrawLine(worldViewTouch);
            }
            else
            {
                if(i != selectedBalls.Count - 1)
                {
                    b.DrawLine(selectedBalls[i + 1].transform.position);
                }
            }
        }
    }

    private void SwapBalls(bool reset = false)
    {
        var tb = targetBall;
        var sb = selectedBall;

        targetBall.transform.position = selectedBall.transform.position;
        selectedBall = targetBall;

        tb.Select(true);
        sb.Select(false);

        selectedBall.Swap(sb, () =>
        {
            if (!reset)
            {
                if (grid.GridHasMatches())
                {
                    grid.CollapseGrid();
                }
                else
                {
                    selectedBall = tb;
                    targetBall = sb;
                    SwapBalls(true);
                    return;
                }
            }
            else
            {
                PAUSED = false;
            }

            targetBall = null;
            selectedBall = null;
        });
    }

    private Ball BallCloseToPoint(Vector2 point)
    {
        worldViewTouch = Camera.main.ScreenToWorldPoint(point);

        int c = Mathf.FloorToInt((worldViewTouch.x + grid.GRID_OFFSET_X + (grid.TILE_SIZE * 0.5f)) / grid.TILE_SIZE);
        if (c < 0)
            c = 0;
        if (c >= grid.COLUMNS)
            c = grid.COLUMNS - 1;

        int r = Mathf.FloorToInt((grid.GRID_OFFSET_Y + (grid.TILE_SIZE * 0.5f) - worldViewTouch.y) / grid.TILE_SIZE);
        if (r < 0) r = 0;
        if (r >= grid.ROWS) r = grid.ROWS - 1;

        return grid.gridBalls[c][r];

    }

    private bool IsValidTarget(Ball ball)
    {
        if(ball == selectedBall)
        {
            return false;
        }

        var offBounds = false;
        var px = ball.column;
        var py = ball.row;

        if(px > selectedBall.column + 1)
        {
            offBounds = true;
        }

        if(px < selectedBall.column - 1)
        {
            offBounds = true;
        }

        if(py > selectedBall.row + 1)
        {
            offBounds = true;
        }

        if(py < selectedBall.row - 1)
        {
            offBounds = true;
        }

        var diagonal = Mathf.Sin(Mathf.Atan2(Mathf.Pow(selectedBall.column - px, 2), Mathf.Pow(selectedBall.row - py, 2)));

        if(diagonal != 0 && diagonal != 1 && diagonal != SIN_45)
        {
            offBounds = true;
        }

        if (offBounds)
        {
            return false;
        }

        return true;
    }

    private void FixedUpdate()
    {
        thumb.transform.position = this.worldViewTouch;   
    }
}