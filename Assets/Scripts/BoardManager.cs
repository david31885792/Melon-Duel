using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform boardParent;
    public TMP_Text moveText;
    public GameManager gameManager;

    private Tile[,] tiles = new Tile[5, 5];
    private Tile emptyTile;
    private int moveCount = 0;

    private Color[] colorPool = new Color[]
    {
        Color.red,
        new Color(1f, 0.5f, 0f),
        Color.yellow,
        Color.green,
        Color.blue,
        Color.white
    };

    public void InitializeBoard()
    {
        List<Color> colors = new List<Color>();
        foreach (Color c in colorPool)
            for (int i = 0; i < 4; i++)
                colors.Add(c);

        for (int i = colors.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (colors[i], colors[j]) = (colors[j], colors[i]);
        }

        int idx = 0;
        float tileSize = 200f;

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                GameObject obj = Instantiate(tilePrefab, boardParent);
                Tile tile = obj.GetComponent<Tile>();
                tile.Setup(x, y, this);

                if (x == 4 && y == 4)
                {
                    emptyTile = tile;
                    tile.SetColor(new Color(0, 0, 0, 0));
                }
                else
                {
                    tile.SetColor(colors[idx++]);
                }

                tiles[x, y] = tile;
                obj.transform.localPosition = new Vector3(x * tileSize, -y * tileSize, 0);
            }
        }

        UpdateMoveText();
    }

    public void TryMoveTile(Tile clicked)
    {
        Debug.Log($"[TryMoveTile] 클릭 좌표: {clicked.x},{clicked.y} / 빈칸: {emptyTile.x},{emptyTile.y}");

        int dx = clicked.x - emptyTile.x;
        int dy = clicked.y - emptyTile.y;

        if ((dx == 0 || dy == 0) && Mathf.Abs(dx + dy) >= 1 && Mathf.Abs(dx + dy) <= 4)
        {
            int stepX = dx != 0 ? -(int)Mathf.Sign(dx) : 0;
            int stepY = dy != 0 ? -(int)Mathf.Sign(dy) : 0;
            int distance = Mathf.Abs(dx != 0 ? dx : dy);
            float tileSize = 200f;

            for (int i = 0; i < distance; i++)
            {
                int fromX = emptyTile.x + stepX * (i + 1);
                int fromY = emptyTile.y + stepY * (i + 1);

                if (fromX < 0 || fromX >= 5 || fromY < 0 || fromY >= 5)
                {
                    Debug.LogWarning($"잘못된 인덱스 접근: ({fromX}, {fromY})");
                    return;
                }

                Tile movingTile = tiles[fromX, fromY];
                Vector3 targetPos = new Vector3(emptyTile.x * tileSize, -emptyTile.y * tileSize, 0);
                tiles[emptyTile.x, emptyTile.y] = movingTile;
                movingTile.x = emptyTile.x;
                movingTile.y = emptyTile.y;
                movingTile.AnimateTo(targetPos);
            }

            tiles[clicked.x, clicked.y] = emptyTile;
            emptyTile.x = clicked.x;
            emptyTile.y = clicked.y;

            moveCount++;
            UpdateMoveText();
            gameManager.CheckWin();
        }
        else
        {
            Debug.Log("이동 조건 불일치: 행 또는 열 아님 / 거리가 초과됨");
        }
    }

    void UpdateMoveText()
    {
        moveText.text = $"Moves: {moveCount}";
    }

    public Tile[,] GetTiles()
    {
        return tiles;
    }
}
