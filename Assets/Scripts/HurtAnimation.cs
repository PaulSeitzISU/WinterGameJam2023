using System.Collections;
using UnityEngine;

public class HurtAnimation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    public float blinkDuration = 2.0f; // Duration in seconds for the sprite to blink
    public float blinkInterval = 0.3f; // Time interval for each blink

    private Color originalColor; // Store the original color of the sprite

    public void Blink()
    {
        // If the SpriteRenderer reference is not set, try to get it from the GameObject
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Store the original color of the sprite
        originalColor = spriteRenderer.color;

        // Start the blinking coroutine
        StartCoroutine(BlinkSprite());
    }

    IEnumerator BlinkSprite()
    {
        // Calculate the number of times the sprite should blink
        int blinkCount = Mathf.FloorToInt(blinkDuration / blinkInterval);

        // Calculate the color change step
        Color redColor = Color.red;
        float step = 1.0f / blinkCount;

        // Change the sprite color to red over time and revert back to the original color
        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.color = Color.Lerp(originalColor, redColor, i * step);
            yield return new WaitForSeconds(blinkInterval / 2); // Half interval for each color change
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkInterval / 2); // Half interval for each color change
        }

        // Ensure the sprite color is reverted back to the original color
        spriteRenderer.color = originalColor;
    }
}
