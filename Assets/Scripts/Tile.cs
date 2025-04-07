using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    private BoardManager boardManager;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Setup(int _x, int _y, BoardManager manager)
    {
        x = _x;
        y = _y;
        boardManager = manager;
    }

    public void SetColor(Color color)
    {
        if (image == null)
            image = GetComponent<Image>();
        image.color = color;
    }

    public Color GetColor()
    {
        return image.color;
    }

    public void OnClick()
    {
        if (boardManager != null)
        {
            boardManager.TryMoveTile(this);
        }
    }

    public void AnimateTo(Vector3 targetPosition)
    {
        transform.DOLocalMove(targetPosition, 0.2f).SetEase(Ease.OutQuad);
    }
}
