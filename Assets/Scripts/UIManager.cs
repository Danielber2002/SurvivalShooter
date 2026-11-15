using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Salud")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Munición")]
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Paneles de Pantalla")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject drawPanel;
    [SerializeField] private GameObject crosshair;

    [Header("Referencias")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private GameClockManager clockManager;
    [SerializeField] private Portal portal;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthUI);
            playerHealth.OnPlayerDeath.AddListener(ShowGameOverScreen);
        }

        if (playerAttack != null)
        {
            playerAttack.OnAmmoChanged.AddListener(UpdateAmmoUI);
        }

        if (clockManager != null)
        {
            clockManager.OnNightEnded.AddListener(ShowDrawScreen);
        }

        if (portal != null)
        {
            portal.OnPortalEntered.AddListener(ShowVictoryScreen);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthUI);
            playerHealth.OnPlayerDeath.RemoveListener(ShowGameOverScreen);
        }

        if (playerAttack != null)
        {
            playerAttack.OnAmmoChanged.RemoveListener(UpdateAmmoUI);
        }

        if (clockManager != null)
        {
            clockManager.OnNightEnded.RemoveListener(ShowDrawScreen);
        }

        if (portal != null)
        {
            portal.OnPortalEntered.RemoveListener(ShowVictoryScreen);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (drawPanel != null) drawPanel.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);
    }

    public void UpdateHealthUI(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }

        if (healthText != null)
        {
            healthText.text = $"{current} / {max} HP";
        }
    }

    public void UpdateAmmoUI(int current, int max)
    {
        if (ammoText != null)
        {
            ammoText.text = $"Munición:   {current} / {max}";
        }
    }

    public void ShowGameOverScreen()
    {
        DisableHUD();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (clockManager != null) clockManager.StopClock();
        PauseGame();
    }

    public void ShowDrawScreen()
    {
        DisableHUD();
        if (drawPanel != null) drawPanel.SetActive(true);
        PauseGame();
    }

    public void ShowVictoryScreen()
    {
        DisableHUD();
        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (clockManager != null) clockManager.StopClock();
        PauseGame();
    }

    private void DisableHUD()
    {
        if (crosshair != null) crosshair.SetActive(false);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}