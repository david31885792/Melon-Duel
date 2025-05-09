using UnityEngine;
using TMPro;

public class BoardManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameManager gameManager;
    public TextMeshProUGUI moveText;

    private const int boardSize = 5;
    private Tile[,] tiles = new Tile[boardSize, boardSize];
    private Tile emptyTile;
    private float tileSpacing = 1.1f; // 타일 간격 (world unit 기준)
    private Vector3 origin;
    private int moveCount = 0;

    public void InitializeBoard()
    {
        moveCount = 0;
        UpdateMoveText();

        // 화면 좌상단 기준 origin 계산 (카메라 기준 위치 0.1 ~ 0.9)
        origin = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0.9f, 10f)); // z=10은 카메라 거리 보정

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

                Vector3 worldPos = origin + new Vector3(x * tileSpacing, -y * tileSpacing, 0);
                GameObject obj = Instantiate(tilePrefab, worldPos, Quaternion.identity);
                Tile tile = obj.GetComponent<Tile>();
                tile.SetColor(colors[colorIndex]);
                tile.x = x;
                tile.y = y;
                tiles[x, y] = tile;
            }
        }

        // 빈칸 생성
        Vector3 emptyPos = origin + new Vector3((boardSize - 1) * tileSpacing, -(boardSize - 1) * tileSpacing, 0);
        GameObject emptyObj = Instantiate(tilePrefab, emptyPos, Quaternion.identity);
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
                Vector3 targetPos = origin + new Vector3(toX * tileSpacing, -toY * tileSpacing, 0);
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
            Debug.Log("이동 조건 불일치: 같은 행/열 & 거리 1~4 범위만 가능");
        }
    }

    public void UpdateMoveText()
    {
        if (moveText != null)
            moveText.text = $"Moves: {moveCount}";
    }

    public Tile[,] GetTiles()
    {
        return tiles;
    }
}
