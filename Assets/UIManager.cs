using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject Player;

    private Stats playerStats;

    public TextMeshProUGUI playerHealthText;
    public Image HpBarFore;
    public Image HpBarBack;
    private float BarWidthFore;

    public bool PlayerHealthTextIsFraction;
    public bool PlayerHealthTextIsPercentage;
    public bool PlayerHealthBarIsVisible;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = Player.GetComponent<Stats>();

        BarWidthFore = HpBarFore.rectTransform.sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHpText();
    }

    private void UpdateHpText()
    {
        float pHpCur = playerStats.Hp.Current;
        float pHpCurMax = playerStats.Hp.CurrentMax;
        float pHpMod = playerStats.Hp.CurrentMax - playerStats.Hp.Max;

        float pHpPercent = pHpCur / pHpCurMax * 100f;

        float pHpModPercent = pHpMod / playerStats.Hp.Max * 100f;

        string newHpText = $"0%";

        if (PlayerHealthTextIsPercentage || PlayerHealthTextIsFraction)
        {
            playerHealthText.enabled = true;
        }
        else
        {
            playerHealthText.enabled = false;
        }
        
        if (PlayerHealthTextIsPercentage)
        {
            newHpText = $"{pHpPercent}% [+{pHpModPercent}%]";
        }

        if (PlayerHealthTextIsFraction)
        {
            newHpText = $"{playerStats.Hp.Current}/{playerStats.Hp.Max} [+{pHpMod}]";
        }

        if (PlayerHealthBarIsVisible)
        {
            Vector2 barSize = new Vector2((pHpPercent / 100f) * BarWidthFore, HpBarFore.rectTransform.sizeDelta.y);

            HpBarFore.rectTransform.sizeDelta = barSize;

            HpBarFore.enabled = true;
            HpBarBack.enabled = true;
        }
        else
        {
            HpBarFore.enabled = false;
            HpBarBack.enabled = false;
        }

        playerHealthText.text = newHpText;
    }
}