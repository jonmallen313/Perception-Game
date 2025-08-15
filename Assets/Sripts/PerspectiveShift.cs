using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PerspectiveShift : MonoBehaviour
{
    [Header("References")]
    public Transform target; // Assign the player in inspector

    [Header("Camera Settings")]
    public Vector3 offset2D = new Vector3(0f, 0f, -10f);
    public Vector3 offset3D = new Vector3(3f, 4f, -8f);

    public Quaternion rot2D = Quaternion.Euler(0f, 90f, 0f);
    public Quaternion rot3D = Quaternion.Euler(20f, -30f, 0f);

    public float transitionSpeed = 3f;

    [Header("Hack Mode Settings")]
    public float hackDuration = 10f;

    [Header("Vignette Flash Settings")]
    public float normalFlashInterval = 1f;
    public float urgentFlashInterval = 0.5f;
    public float urgentThreshold = 0.25f; // % of time remaining to enter fast flashing

    private bool in3DMode = false;
    private bool transitioning = false;
    private bool hackTimerActive = false;
    private float hackTimeRemaining = 0f;

    private Rigidbody targetRB;

    public bool flipped2DView = false;

    // Vignette UI
    private Canvas vignetteCanvas;
    private Image vignetteImage;
    private Coroutine flashCoroutine;
    private bool isFlashingFast = false;

    private void Start()
    {
        targetRB = target.GetComponent<Rigidbody>();
        SetupVignetteUI();

        StartCoroutine(WaitAndFindPlayer());
        
    }

    IEnumerator WaitAndFindPlayer()
    {
        GameObject player = null;
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        target = player.transform;
        targetRB = target.GetComponent<Rigidbody>();

        Vector3 effectiveOffset = in3DMode
        ? (flipped2DView ? new Vector3(-offset3D.x, offset3D.y, -offset3D.z) : offset3D)
        : (flipped2DView ? new Vector3(-offset2D.x, offset2D.y, -offset2D.z) : offset2D);

        transform.position = target.position + effectiveOffset;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !transitioning)
        {
            if (!hackTimerActive)
            {
                Enter3DMode();
            }
            else
            {
                //early exit
                Exit3DMode(); 
            }
        }

        if (hackTimerActive)
        {
            hackTimeRemaining -= Time.deltaTime;

            // Switch to faster flashing near end
            if (hackTimeRemaining <= hackDuration * urgentThreshold && !isFlashingFast)
            {
                if (flashCoroutine != null) StopCoroutine(flashCoroutine);
                flashCoroutine = StartCoroutine(FlashVignette(urgentFlashInterval));

                isFlashingFast = true;
            }

            if (hackTimeRemaining <= 0f)
            {
                Exit3DMode();
            }
        }
    }

    void LateUpdate()
    {
        if (target == null || transitioning)
            return;

        // Effective 2D offset with Z also negated (to flip behind)
        Vector3 effectiveOffset2D = flipped2DView ? new Vector3(-offset2D.x, offset2D.y, -offset2D.z) : offset2D;

        // For 3D mode, flip the offset similarly when flipped2DView is true
        Vector3 effectiveOffset3D = flipped2DView ? new Vector3(-offset3D.x, offset3D.y, -offset3D.z) : offset3D;

        // Choose desired position based on mode & flipped state
        Vector3 desiredPosition = in3DMode ? target.position + effectiveOffset3D : target.position + effectiveOffset2D;

        // Rotation for 2D mode: flip Y by 180° if flipped
        Quaternion effectiveRot2D = flipped2DView ? Quaternion.Euler(rot2D.eulerAngles.x, rot2D.eulerAngles.y + 180f, rot2D.eulerAngles.z) : rot2D;

        // Rotation for 3D mode: flip Y by 180° if flipped
        Quaternion effectiveRot3D = flipped2DView ? Quaternion.Euler(rot3D.eulerAngles.x, rot3D.eulerAngles.y + 180f, rot3D.eulerAngles.z) : rot3D;

        // Pick rotation based on mode
        Quaternion desiredRotation = in3DMode ? effectiveRot3D : effectiveRot2D;

        // Smoothly interpolate position and rotation
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * transitionSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * transitionSpeed);

        // Fade vignette out when not in hack mode
        if (!hackTimerActive && vignetteImage != null && vignetteImage.color.a > 0f)
        {
            float alpha = Mathf.MoveTowards(vignetteImage.color.a, 0f, Time.deltaTime * transitionSpeed);
            SetVignetteAlpha(alpha);
        }
    }

    void Enter3DMode()
    {
        in3DMode = true;
        hackTimerActive = true;
        isFlashingFast = false;
        hackTimeRemaining = hackDuration;

        StopAllCoroutines();
        StartCoroutine(SmoothTransition());

        FreezeZ(false);

        
    }

    void Exit3DMode()
    {
        in3DMode = false;
        hackTimerActive = false;

        StopAllCoroutines();
        StartCoroutine(SmoothTransition());

        FreezeZ(true);

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        SetVignetteAlpha(0f);
    }

    IEnumerator SmoothTransition()
    {
        transitioning = true;
        yield return new WaitForSeconds(0.3f);
        transitioning = false;

        if (in3DMode && hackTimerActive)
        {
            flashCoroutine = StartCoroutine(FlashVignette(normalFlashInterval));
        }
    }

    void FreezeZ(bool freeze)
    {
        if (targetRB == null) return;

        if (freeze)
            targetRB.constraints |= RigidbodyConstraints.FreezePositionZ;
        else
            targetRB.constraints &= ~RigidbodyConstraints.FreezePositionZ;
    }

    void SetupVignetteUI()
    {
        GameObject canvasGO = new GameObject("HackVignetteCanvas");

        vignetteCanvas = canvasGO.AddComponent<Canvas>();
        vignetteCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject imgGO = new GameObject("VignetteImage");
        imgGO.transform.SetParent(canvasGO.transform, false);

        vignetteImage = imgGO.AddComponent<Image>();

        RectTransform rect = vignetteImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Use blue radial gradient texture if you have one
        Sprite vignetteSprite = Resources.Load<Sprite>("VignetteRadial");
        if (vignetteSprite != null)
        {
            vignetteImage.sprite = vignetteSprite;
            vignetteImage.type = Image.Type.Sliced;
            vignetteImage.color = new Color(1f, 1f, 1f, 0f); // Show original texture color
        }
        else
        {
            vignetteImage.color = new Color(0f, 0.8f, 1f, 0f); // Fallback: solid blue
        }
    }

    IEnumerator FlashVignette(float interval)
    {
        // flash severity
        float peakAlpha = 0.15f; 
        float halfInterval = interval / 2f;

        while (hackTimerActive && transitioning == false)
        {
            float t = 0f;

            // Fade in
            while (t < halfInterval)
            {
                t += Time.deltaTime;
                float alpha = Mathf.SmoothStep(0f, peakAlpha, t / halfInterval);
                SetVignetteAlpha(alpha);
                yield return null;
            }

            t = 0f;

            // Fade out
            while (t < halfInterval)
            {
                t += Time.deltaTime;
                float alpha = Mathf.SmoothStep(peakAlpha, 0f, t / halfInterval);
                SetVignetteAlpha(alpha);
                yield return null;
            }
        }

        // Once stopped, fade out if needed
        SetVignetteAlpha(0f);
    }

    void SetVignetteAlpha(float alpha)
    {
        if (vignetteImage != null)
        {
            Color c = vignetteImage.color;
            c.a = alpha;
            vignetteImage.color = c;
        }
    }

    public bool IsIn3DMode()
    {
        return in3DMode;
    }

    public void Flip2DView()
    {
        if(!in3DMode && !transitioning)
        {
            
            flipped2DView = !flipped2DView;
        }
    }
}
