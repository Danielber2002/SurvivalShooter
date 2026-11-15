using UnityEngine;
using Unity.Behavior;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Salud")]
    [SerializeField] private float maxHealth = 10f;
    private float currentHealth;

    [Header("Behavior Graph")]
    [SerializeField] private BehaviorGraphAgent behaviorAgent;
    [SerializeField] private string isDeadVariableName = "IsDead";

    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        if (behaviorAgent == null)
        {
            behaviorAgent = GetComponent<BehaviorGraphAgent>();
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;

        if (behaviorAgent != null)
        {
            behaviorAgent.SetVariableValue(isDeadVariableName, true);
        }
    }
}