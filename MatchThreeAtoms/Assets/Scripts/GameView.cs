using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameView : MonoBehaviour
{

    [HideInInspector]
    public Ball selectedBall;

    [HideInInspector]
    public float SCREEN_WIDTH = 6.6f;

    [HideInInspector]
    public float SCREEN_HEIGHT = 10.2f;

    public Grid grid;

    private Vector2 worldViewTouch;

    void Start()
    {

        selectedBall = null;

    }


    public void HandleTouchDown(Vector2 touch)
    {
        selectedBall = BallCloseToPoint(touch);

        if (selectedBall != null)
        {
            selectedBall.gameObject.SetActive(false);
        }

    }

    public void HandleTouchUp(Vector2 touch)
    {

    }

    public void HandleTouchMove(Vector2 touch)
    {

    }

    private Ball BallCloseToPoint(Vector2 point)
    {
        worldViewTouch = Camera.main.ScreenToWorldPoint(point);

        var t = Camera.main.ScreenToWorldPoint(point);
        t.z = 0;


        int c = Mathf.FloorToInt((t.x + grid.GRID_OFFSET_X + (grid.TILE_SIZE * 0.5f)) / grid.TILE_SIZE);
        if (c < 0)
            c = 0;
        if (c >= grid.COLUMNS)
            c = grid.COLUMNS - 1;

        int r = Mathf.FloorToInt((grid.GRID_OFFSET_Y + (grid.TILE_SIZE * 0.5f) - t.y) / grid.TILE_SIZE);
        if (r < 0) r = 0;
        if (r >= grid.ROWS) r = grid.ROWS - 1;

        return grid.gridBalls[c][r];

    }

}