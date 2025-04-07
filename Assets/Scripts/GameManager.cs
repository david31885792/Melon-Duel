using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    public TMP_Text timerText;
    public GameObject winText;

    private float elapsedTime;
    private bool isPlaying = false;
    private Color[] targetPattern;

    public void StartGame(Color[] pattern)
    {
        boardManager.InitializeBoard();
        targetPattern = pattern;
        elapsedTime = 0;
        isPlaying = true;
    }

    void Update()
    {
        if (isPlaying)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = $"Time: {elapsedTime:F1}s";
        }
    }

    public void CheckWin()
    {
        Tile[,] tiles = boardManager.GetTiles();
        int index = 0;

        for (int y = 1; y <= 3; y++)
        {
            for (int x = 1; x <= 3; x++)
            {
                if (tiles[x, y].GetColor() != targetPattern[index])
                    return;
                index++;
            }
        }

        isPlaying = false;
        winText.SetActive(true);
        Invoke("ReturnToMainScene", 2f);
    }

    void ReturnToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
