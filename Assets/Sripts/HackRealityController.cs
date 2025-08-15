using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HackRealityController : MonoBehaviour
{
    [Header("UI Elements")]
    public Image vignetteOverlay;
    public Image cooldownBar;

    [Header("References")]
    public PlayerMovementAlt playerMovement;

    [Header("Hack Settings")]
    public float hackDuration = 20f;
    public float cooldownTime = 30f;
    public KeyCode activateKey = KeyCode.E;

    private bool isInHackMode = false;
    private bool isOnCooldown = false;

    private void Start()
    {
        // Set initial UI states
        if (vignetteOverlay) vignetteOverlay.color = new Color(0f, 0.8f, 1f, 0f);
        if (cooldownBar) cooldownBar.fillAmount = 1f;

        // Auto-assign playerMovement if not set
        if (playerMovement == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerMovement = player.GetComponent<PlayerMovementAlt>();
        }
    }

    private void Update()
    {
        // Activate Hack Mode
        if (Input.GetKeyDown(activateKey) && !isInHackMode && !isOnCooldown)
        {
            StartCoroutine(HackPhase());
        }
    }

    IEnumerator HackPhase()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        isInHackMode = true;
        isOnCooldown = true;

        float timeRemaining = hackDuration;

        

        // Fade in vignette
        float targetAlpha = 0.7f;
        float fadeSpeed = 1f;
        float currentAlpha = 0f;

        while (currentAlpha < targetAlpha)
        {
            currentAlpha += Time.deltaTime * fadeSpeed;
            vignetteOverlay.color = new Color(0f, 0.8f, 1f, currentAlpha);
            yield return null;
        }

        // Timer loop with vignette fade-out effect
        while (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;

            // Nonlinear fadeout for urgency
            float t = timeRemaining / hackDuration;
            float fade = Mathf.Pow(t, 2); // Fast fade at the end
            vignetteOverlay.color = new Color(0f, 0.8f, 1f, fade * targetAlpha);

            yield return null;
        }

        

        vignetteOverlay.color = new Color(0f, 0.8f, 1f, 0f);
        isInHackMode = false;

        // Begin cooldown
        StartCoroutine(CooldownRoutine());
    }

    IEnumerator CooldownRoutine()
    {
        float cooldownRemaining = cooldownTime;

        while (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;

            if (cooldownBar != null)
                cooldownBar.fillAmount = 1f - (cooldownRemaining / cooldownTime);

            yield return null;
        }

        isOnCooldown = false;
        if (cooldownBar != null)
            cooldownBar.fillAmount = 1f;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
}
