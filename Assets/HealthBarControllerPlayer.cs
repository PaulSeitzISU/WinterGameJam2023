using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using JetBrains.Annotations;
using TMPro;

public class HealthBarControllerPlayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image healthBarImage;
    public Sprite[] healthSprites;

    public Health healthScript;
    private bool isHovering = false;

    void Start()
    {
        if(healthScript == null)
        {
            healthScript = GetComponentInParent<Health>();
        }
        if (healthScript == null)
        {
            Debug.LogError("Health script not found in parent object!");
            return;
        }

        SetHealthBarActive(false); // Initially, hide the health bar
    }

    void FixedUpdate()
    {
            UpdateHealthBar();
    }

    // Update health bar UI
    void UpdateHealthBar()
    {
        float healthPercentage = (float)healthScript.currentHealth / healthScript.maxHealth;
        int spriteIndex = Mathf.Max(0, Mathf.RoundToInt((1 - healthPercentage) * (healthSprites.Length - 1)));
        Debug.Log(spriteIndex);
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
        //healthBarImage.gameObject.SetActive(isActive);
    }
}


