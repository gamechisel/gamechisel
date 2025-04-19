using UnityEngine;

public class UIWiggleEffect : MonoBehaviour
{
    public float wiggleAmount = 10f;
    public float wiggleSpeed = 2f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Calculate the wiggle effect using Mathf.Sin and Time.time
        float offsetX = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;
        float offsetY = Mathf.Cos(Time.time * wiggleSpeed * 1.5f) * wiggleAmount;

        // Apply the wiggle effect to the button's position
        transform.position = initialPosition + new Vector3(offsetX, offsetY, 0f);
    }
}
