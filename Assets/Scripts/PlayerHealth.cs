using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Salud")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHealth;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathTriggerName = "Die";

    [Header("Eventos de UI")]
    public UnityEvent<float, float> OnHealthChanged;
    public UnityEvent OnPlayerDeath;

    public bool IsDead { get; private set; }
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public bool Heal(float amount)
    {
        if (IsDead || currentHealth >= maxHealth) return false;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"[Jugador] Curado {amount} HP. Salud actual: {currentHealth}/{maxHealth}");
        return true;
    }

    private void Die()
    {
        IsDead = true;
        OnPlayerDeath?.Invoke();

        if (animator != null && !string.IsNullOrEmpty(deathTriggerName))
        {
            animator.SetTrigger(deathTriggerName);
        }
    }
}