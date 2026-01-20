using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    public static GameHUD Instance;
    
    [Header("Time Display")]
    public TextMeshProUGUI timeText;
    
    [Header("Spirit Power Display")]
    public Image spiritPowerFill; // The filled image
    public TextMeshProUGUI spiritTitleText;
    public TextMeshProUGUI spiritValueText; // Optional percentage text
    
    [Header("Spirit Power Settings")]
    public float maxSpiritPower = 100f;
    public Color lowColor = Color.green;
    public Color mediumColor = Color.yellow;
    public Color highColor = Color.red;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    private void OnEnable()
    {
        GameEvents.OnSpiritPowerChanged += UpdateSpiritPower;
    }
    
    private void OnDisable()
    {
        GameEvents.OnSpiritPowerChanged -= UpdateSpiritPower;
    }
    
    private void Update()
    {
        UpdateTimeDisplay();
    }
    
    private void UpdateTimeDisplay()
    {
        if (TimeManager.Instance != null)
        {
            float time = TimeManager.Instance.currentTime;
            int hours = Mathf.FloorToInt(time);
            int minutes = Mathf.FloorToInt((time - hours) * 60);
            
            // Convert to 12-hour format
            string period = hours >= 12 ? "PM" : "AM";
            int displayHours = hours > 12 ? hours - 12 : (hours == 0 ? 12 : hours);
            
            timeText.text = $"{displayHours:00}:{minutes:00} {period}";
        }
    }
    
    private void UpdateSpiritPower(float power)
    {
        // Update fill amount (0 to 1)
        float fillAmount = Mathf.Clamp01(power / maxSpiritPower);
        spiritPowerFill.fillAmount = fillAmount;
        
        // Update color based on power level
        if (power >= 80)
        {
            spiritPowerFill.color = highColor;
            spiritTitleText.text = "CRITICAL THREAT!";
        }
        else if (power >= 50)
        {
            spiritPowerFill.color = mediumColor;
            spiritTitleText.text = "High Spirit Activity";
        }
        else if (power >= 20)
        {
            spiritPowerFill.color = Color.Lerp(lowColor, mediumColor, (power - 20) / 30);
            spiritTitleText.text = "Spirit Activity Detected";
        }
        else
        {
            spiritPowerFill.color = lowColor;
            spiritTitleText.text = "All Clear";
        }
        
        // Update percentage text (optional)
        if (spiritValueText != null)
        {
            int percentage = Mathf.RoundToInt((power / maxSpiritPower) * 100);
            spiritValueText.text = $"{percentage}%";
        }
    }
}
