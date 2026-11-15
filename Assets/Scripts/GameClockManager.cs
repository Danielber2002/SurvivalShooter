using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameClockManager : MonoBehaviour
{
    [Header("Configuración del Reloj")]
    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private float nightDurationInSeconds = 600f;

    [Header("Eventos")]
    public UnityEvent OnNightEnded;

    private float currentTime;
    private bool isClockRunning = true;

    private void Start()
    {
        currentTime = 0f;
        UpdateClockUI(20, 0);
    }

    private void Update()
    {
        if (!isClockRunning) return;

        currentTime += Time.deltaTime;
        float progress = Mathf.Clamp01(currentTime / nightDurationInSeconds);

        float totalVirtualMinutes = progress * 600f;
        int hours = (20 + (int)(totalVirtualMinutes / 60f)) % 24;
        int minutes = (int)(totalVirtualMinutes % 60f);

        UpdateClockUI(hours, minutes);

        if (progress >= 1f)
        {
            isClockRunning = false;
            OnNightEnded?.Invoke();
        }
    }

    private void UpdateClockUI(int hours, int minutes)
    {
        if (clockText != null)
        {
            clockText.text = $"{hours:D2}:{minutes:D2}";
        }
    }

    public void StopClock()
    {
        isClockRunning = false;
    }
}