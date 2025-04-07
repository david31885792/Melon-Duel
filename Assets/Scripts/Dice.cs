using UnityEngine;

public class Dice : MonoBehaviour
{
    private Rigidbody rb;
    public float torqueMin = 100f;
    public float torqueMax = 200f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void Roll()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 randomDir = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        Vector3 torque = randomDir * Random.Range(torqueMin, torqueMax);
        rb.AddTorque(torque, ForceMode.Impulse);
    }

    public void StopRoll()
    {
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }

    public Color DetectTopColor()
    {
        Transform topFace = null;
        float maxY = float.MinValue;

        foreach (Transform face in transform)
        {
            if (face.position.y > maxY)
            {
                maxY = face.position.y;
                topFace = face;
            }
        }

        if (topFace != null)
        {
            SpriteRenderer sr = topFace.GetComponent<SpriteRenderer>();
            if (sr != null)
                return sr.color;
        }

        return Color.black;
    }
}
