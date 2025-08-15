using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class NewBFScript : MonoBehaviour
{
    private GameObject canvasGO;
    private GameObject uiPanel;
    private InputField inputField;
    private Text[] flashSlots;
    private Image timeBar;

    private string codeToGuess = "";
    private int level = 1;
    private float timeRemaining;
    private bool isPlaying = false;

    private bool playerInRange = false;

    private Coroutine countdownCoroutine;

    private void Update()
    {
        if (playerInRange && !isPlaying && Input.GetKeyDown(KeyCode.F))
        {
            ShowHackUI();
            StartLevel(level);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void ShowHackUI()
    {
        // Canvas
        canvasGO = new GameObject("HackCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        canvasGO.AddComponent<EventSystem>();
        canvasGO.AddComponent<StandaloneInputModule>();

        // Panel
        uiPanel = new GameObject("HackPanel", typeof(Image));
        uiPanel.transform.SetParent(canvasGO.transform);

        var panelImage = uiPanel.GetComponent<Image>();
        panelImage.color = Color.black;

        var panelRT = uiPanel.GetComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(500, 600);
        panelRT.anchoredPosition = Vector2.zero;

        // Flash Slots
        flashSlots = new Text[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject slot = new GameObject("Slot" + i, typeof(Text));
            slot.transform.SetParent(uiPanel.transform);

            var txt = slot.GetComponent<Text>();
            txt.font = Font.CreateDynamicFontFromOSFont("Courier New", 34);
            txt.color = Color.green;
            txt.text = "*";
            txt.alignment = TextAnchor.MiddleCenter;

            var rt = txt.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            rt.anchoredPosition = new Vector2(-75 + i * 50, 200);
            flashSlots[i] = txt;
        }

        // Input Field
        GameObject inputGO = new GameObject("CodeInput", typeof(InputField), typeof(Image));
        inputGO.transform.SetParent(uiPanel.transform);

        var inputRT = inputGO.GetComponent<RectTransform>();
        inputRT.sizeDelta = new Vector2(200, 40);
        inputRT.anchoredPosition = new Vector2(0, 130);

        var inputImage = inputGO.GetComponent<Image>();
        inputImage.color = Color.black;

        inputField = inputGO.GetComponent<InputField>();
        inputField.textComponent = CreateUIText(inputGO.transform, Vector2.zero, 24);
        inputField.textComponent.color = Color.green;
        inputField.placeholder = CreateUIText(inputGO.transform, Vector2.zero, 24, "Enter Code");
        inputField.placeholder.color = new Color(0, 1, 0, 0.5f);

        // Number Buttons (0–9)
        for (int i = 1; i <= 9; i++)
            CreateButton(i.ToString(), new Vector2(-100 + ((i - 1) % 3) * 100, 40 - ((i - 1) / 3) * 60));
        CreateButton("0", new Vector2(0, -140));

        // Submit & Clear
        CreateButton("SUBMIT", new Vector2(-100, -200), SubmitCode);
        CreateButton("CLEAR", new Vector2(100, -200), () => inputField.text = "");

        // Timer Bar
        GameObject barGO = new GameObject("TimeBar", typeof(Image));
        barGO.transform.SetParent(uiPanel.transform);
        timeBar = barGO.GetComponent<Image>();
        timeBar.color = Color.green;

        //Add a source image with fill support
        Sprite fillSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        timeBar.sprite = fillSprite;

        timeBar.type = Image.Type.Filled;
        timeBar.fillMethod = Image.FillMethod.Horizontal;
        timeBar.fillOrigin = 0;
        timeBar.fillAmount = 1f;

        var barRT = timeBar.GetComponent<RectTransform>();
        barRT.sizeDelta = new Vector2(300, 20);
        barRT.anchoredPosition = new Vector2(0, -250);
    }

    Text CreateUIText(Transform parent, Vector2 pos, int fontSize, string placeholder = "")
    {
        GameObject txtGO = new GameObject("Text", typeof(Text));
        txtGO.transform.SetParent(parent);

        var txt = txtGO.GetComponent<Text>();
        txt.font = Font.CreateDynamicFontFromOSFont("Courier New", 34);
        txt.fontSize = 34;
        txt.text = placeholder;
        txt.alignment = TextAnchor.MiddleCenter;

        var rt = txt.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(180, 40);
        rt.anchoredPosition = pos;
        return txt;
    }

    void CreateButton(string label, Vector2 pos, UnityEngine.Events.UnityAction action = null)
    {
        GameObject btnGO = new GameObject(label + "Btn", typeof(Button), typeof(Image));
        btnGO.transform.SetParent(uiPanel.transform);

        var img = btnGO.GetComponent<Image>();
        img.color = Color.black;

        var btn = btnGO.GetComponent<Button>();

        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 40);
        rt.anchoredPosition = pos;

        Text txt = CreateUIText(btnGO.transform, Vector2.zero, 20, label);
        txt.color = Color.green;

        if (action == null)
            btn.onClick.AddListener(() => inputField.text += label);
        else
            btn.onClick.AddListener(action);
    }

    void StartLevel(int lvl)
    {
        Wait(1f);
        level = lvl;
        inputField.text = "";
        codeToGuess = "";
        for (int i = 0; i < 4; i++) codeToGuess += Random.Range(0, 10);
        timeRemaining = level == 1 ? 30f : level == 2 ? 20f : 15f;
        isPlaying = true;
        StartCoroutine(FlashLoop());
        countdownCoroutine = StartCoroutine(Countdown());
    }

    IEnumerator Wait(float n)
    {
        yield return new WaitForSeconds(n);
    }

    IEnumerator FlashFailureThenClose()
    {
        Image panelImage = uiPanel.GetComponent<Image>();

        for (int i = 0; i < 3; i++)
        {
            panelImage.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            panelImage.color = Color.black;
            yield return new WaitForSeconds(0.2f);
        }

        isPlaying = false;

        if (countdownCoroutine != null)
            StopCoroutine(countdownCoroutine);

        StopCoroutine(FlashLoop());

        timeBar = null;
        inputField = null;
        flashSlots = null;

        yield return new WaitForSeconds(0.5f);
        canvasGO.SetActive(false);
    }

    IEnumerator FlashSuccessThenClose()
    {
        Image panelImage = uiPanel.GetComponent<Image>();

        for (int i = 0; i < 3; i++)
        {
            panelImage.color = Color.green;
            yield return new WaitForSeconds(0.2f);
            panelImage.color = Color.black;
            yield return new WaitForSeconds(0.2f);
        }

        StopCoroutine(FlashLoop());
        yield return new WaitForSeconds(0.5f);
        canvasGO.SetActive(false);
    }

    IEnumerator FlashLoop()
    {
        float flashDuration = level == 1 ? 0.6f : level == 2 ? 0.4f : 0.3f;
        float pauseBetweenDigits = 0.2f;

        int length = codeToGuess.Length;


        while (isPlaying)
        {
            if (flashSlots == null || flashSlots.Length == 0)
                yield break; 

            int randomIndex = Random.Range(0, length);

            var slot = flashSlots[randomIndex];
            if (slot != null)
            {
                slot.text = codeToGuess[randomIndex].ToString();
                yield return new WaitForSeconds(flashDuration);

                if (slot != null)
                    slot.text = "*";
            }

            yield return new WaitForSeconds(pauseBetweenDigits);
        }
    }

    IEnumerator Countdown()
    {
        float total = timeRemaining;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeBar != null)
                timeBar.fillAmount = timeRemaining / total;

            yield return null;


        }

        isPlaying = false;
        RevealCode(false);
        StartCoroutine(FlashFailureThenClose());
    }

    void SubmitCode()
    {
        
        if (!isPlaying) return;

        if (inputField.text == codeToGuess)
        {
            isPlaying = false;

            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);

            Debug.Log("Correct! Proceed to next level.");

            if (level < 3)
                StartLevel(level + 1);
            else
            {
                RevealCode(true);
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    var inventory = player.GetComponent<PlayerInventory>();
                    if (inventory != null)
                    {
                        inventory.hasKey = true;
                        inventory.key = 1;
                        Debug.Log("Player got the key!");
                    }
                }
                StartCoroutine(FlashSuccessThenClose());
                
            }
        }
        else
        {
            Debug.Log("Incorrect!");
            StartCoroutine(FlashFailureThenClose());
        }
    }

    void RevealCode(bool success)
    {
        for (int i = 0; i < flashSlots.Length; i++)
            flashSlots[i].text = codeToGuess[i].ToString();

        Debug.Log(success ? "All levels complete!" : "Failed! Code was: " + codeToGuess);
    }
}
