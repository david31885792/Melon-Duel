using UnityEngine;
using TMPro;

public class BoardManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform boardParent;
    public GameManager gameManager;
    public TextMeshProUGUI moveText;

    private const int boardSize = 5;
    private Tile[,] tiles = new Tile[boardSize, boardSize];
    private Tile emptyTile;
    private float tileSize = 200f;
    private int moveCount = 0;

    public void InitializeBoard()
    {
        foreach (Transform child in boardParent)
        {
            Destroy(child.gameObject);
        }

        moveCount = 0;
        UpdateMoveText();

        Color[] colors = { Color.red, new Color(1f, 0.5f, 0f), Color.yellow, Color.green, Color.blue, Color.white };
        int[] colorCounts = new int[6];
        System.Random rand = new System.Random();

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (x == boardSize - 1 && y == boardSize - 1)
                    continue;

                int colorIndex;
                do
                {
                    colorIndex = rand.Next(0, 6);
                } while (colorCounts[colorIndex] >= 4);

                colorCounts[colorIndex]++;

                GameObject obj = Instantiate(tilePrefab, boardParent);
                obj.transform.localPosition = new Vector3(x * tileSize, -y * tileSize, 0);
                Tile tile = obj.GetComponent<Tile>();
                tile.SetColor(colors[colorIndex]);
                tile.x = x;
                tile.y = y;
                tiles[x, y] = tile;
            }
        }

        GameObject emptyObj = Instantiate(tilePrefab, boardParent);
        emptyObj.transform.localPosition = new Vector3((boardSize - 1) * tileSize, -(boardSize - 1) * tileSize, 0);
        emptyTile = emptyObj.GetComponent<Tile>();
        emptyTile.SetColor(Color.clear);
        emptyTile.x = boardSize - 1;
        emptyTile.y = boardSize - 1;
        tiles[boardSize - 1, boardSize - 1] = emptyTile;
    }

    public void TryMoveTile(Tile clicked)
    {
        int dx = clicked.x - emptyTile.x;
        int dy = clicked.y - emptyTile.y;

        // 같은 행 또는 열 && 거리 1~4
        if ((dx == 0 || dy == 0) && Mathf.Abs(dx + dy) >= 1 && Mathf.Abs(dx + dy) <= 4)
        {
            int stepX = dx != 0 ? (int)Mathf.Sign(dx) : 0;
            int stepY = dy != 0 ? (int)Mathf.Sign(dy) : 0;
            int distance = Mathf.Abs(dx != 0 ? dx : dy);

            for (int i = 0; i < distance; i++)
            {
                int fromX = emptyTile.x + stepX * (i + 1);
                int fromY = emptyTile.y + stepY * (i + 1);
                int toX = emptyTile.x + stepX * i;
                int toY = emptyTile.y + stepY * i;

                if (
                    fromX < 0 || fromX >= boardSize ||
                    fromY < 0 || fromY >= boardSize ||
                    toX < 0 || toX >= boardSize ||
                    toY < 0 || toY >= boardSize
                )
                {
                    Debug.LogWarning($"잘못된 인덱스 접근: from({fromX},{fromY}) → to({toX},{toY})");
                    return;
                }

                Tile movingTile = tiles[fromX, fromY];
                Vector3 targetPos = new Vector3(toX * tileSize, -toY * tileSize, 0);
                tiles[toX, toY] = movingTile;
                movingTile.x = toX;
                movingTile.y = toY;
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
            Debug.Log("이동 조건 불일치: 같은 행 또는 열이 아니거나 거리 초과");
        }
    }

    public void UpdateMoveText()
    {
        moveText.text = $"Moves: {moveCount}";
    }

    public Tile[,] GetTiles()
    {
        return tiles;
    }
}
