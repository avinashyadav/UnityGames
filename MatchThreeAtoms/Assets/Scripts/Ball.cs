using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{

    public GameObject[] colorsGO;

    [HideInInspector]
    public int row;

    [HideInInspector]
    public int column;

    private Vector3 tilePosition;

    private Grid grid;


    public void SetBallPosition(Grid grid, int column, int row)
    {

        this.grid = grid;
        this.column = column;
        this.row = row;

        tilePosition = new Vector3((column * grid.TILE_SIZE) - grid.GRID_OFFSET_X, grid.GRID_OFFSET_Y + (-row * grid.TILE_SIZE), 0);
        transform.localPosition = tilePosition;

        foreach (var go in colorsGO)
        {
            go.SetActive(false);
        }
        colorsGO[0].SetActive(true);
    }

}