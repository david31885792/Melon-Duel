using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    public int x, y;
    private Image img;
    private Color color;
    private BoardManager boardManager;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void SetColor(Color c)
    {
        color = c;
        img.color = c;
    }

    public Color GetColor()
    {
        return color;
    }

    public void AnimateTo(Vector3 targetPos)
    {
        transform.DOLocalMove(targetPos, 0.3f).SetEase(Ease.OutQuad);
    }

    public void OnClick()
    {
        if (boardManager == null)
        {
            boardManager = FindAnyObjectByType<BoardManager>(); // ✅ 자동 연결
        }

        Debug.Log($"[Tile] Clicked: ({x},{y})");
        boardManager.TryMoveTile(this);
    }
}
