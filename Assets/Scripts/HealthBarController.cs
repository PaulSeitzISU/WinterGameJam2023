using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HealthBarController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image healthBarImage;
    public Sprite[] healthSprites;

    private Health healthScript;
    private bool isHovering = false;

    void Start()
    {
        healthScript = GetComponentInParent<Health>();
        if (healthScript == null)
        {
            Debug.LogError("Health script not found in parent object!");
            return;
        }

        SetHealthBarActive(false); // Initially, hide the health bar
    }

    void Update()
    {
        if (isHovering)
        {
            UpdateHealthBar();
        }
    }

    // Update health bar UI
    void UpdateHealthBar()
    {
        float healthPercentage = (float)healthScript.currentHealth / healthScript.maxHealth;
        int spriteIndex = Mathf.Max(0, Mathf.RoundToInt(healthPercentage * (healthSprites.Length - 1)));

        healthBarImage.sprite = healthSprites[spriteIndex];
    }

    // Toggle health bar visibility on hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        SetHealthBarActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        SetHealthBarActive(false);
    }

    // Helper method to enable/disable health bar
    void SetHealthBarActive(bool isActive)
    {
        healthBarImage.gameObject.SetActive(isActive);
    }
}


