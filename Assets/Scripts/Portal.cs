using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    [Header("Eventos")]
    public UnityEvent OnPortalEntered;

    private bool hasEntered;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasEntered) return;

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player != null && !player.IsDead)
        {
            hasEntered = true;
            OnPortalEntered?.Invoke();
        }
    }
}