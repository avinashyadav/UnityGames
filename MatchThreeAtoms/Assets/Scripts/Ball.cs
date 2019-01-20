﻿using UnityEngine;
using System.Collections;
using Prime31.GoKitLite;

public class Ball : MonoBehaviour
{
    public enum BALL_TYPE
    {
        NONE = -1,
        TYPE_1,
        TYPE_2,
        TYPE_3,
        TYPE_4,
        TYPE_5,
        TYPE_6,
        TYPE_7,
        TYPE_8,
        TYPE_9,
        TYPE_10,
        TYPE_11,
        TYPE_12,
        TYPE_13,
        TYPE_14,
        TYPE_15,
        TYPE_16,
        TYPE_17,
        TYPE_18,
        TYPE_19,
        TYPE_20
    };


    public GameObject[] colorsGO;

    [HideInInspector]
    public int row;

    [HideInInspector]
    public int column;

    [HideInInspector]
    public BALL_TYPE type;

    private Vector3 ballPosition;

    private Grid grid;


    public void SetBallPosition(Grid grid, int column, int row)
    {

        this.grid = grid;
        this.column = column;
        this.row = row;

        ballPosition = new Vector3((column * grid.TILE_SIZE) - grid.GRID_OFFSET_X, grid.GRID_OFFSET_Y + (-row * grid.TILE_SIZE), 0);
        transform.localPosition = ballPosition;

        foreach (var go in colorsGO)
        {
            go.SetActive(false);
        }
    }

    public void Drop(System.Action<Ball> callBack)
    {
        gameObject.SetActive(true);
        if(transform.position.y != ballPosition.y)
        {
            GoKitLite.instance.positionTo(transform, (transform.position.y - ballPosition.y) * 0.3f, ballPosition)
                .setEaseType(EaseType.BackInOut)
                .setCompletionHandler((Transform t) =>
                {
                    callBack(this);
                });
        }
    }

    public void MatchBallType(Ball ball)
    {
        SetType(ball.type);
    }

    public void SetTempPosition(int index)
    {
        var newPosition = ballPosition;
        newPosition.y = grid.GRID_OFFSET_Y + (-index * grid.TILE_SIZE);
        transform.localPosition = newPosition;
    }

    public void SetType(BALL_TYPE type)
    {
        foreach(var go in colorsGO)
        {
            go.SetActive(false);
        }

        this.type = type;

        if (type == BALL_TYPE.NONE)
            return;

        colorsGO[(int)type].SetActive(true);
    }
}