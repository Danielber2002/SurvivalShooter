using UnityEngine;

public enum PickupType
{
    Health,
    Ammo
}

[RequireComponent(typeof(Collider))]
public class Pickup : MonoBehaviour
{
    [Header("Tipo y Cantidad")]
    [SerializeField] private PickupType type = PickupType.Health;
    [SerializeField] private int amount = 2;

    [Header("Visual")]
    [SerializeField] private float rotationSpeed = 60f;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime), Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        PlayerAttack attack = other.GetComponentInParent<PlayerAttack>();

        if (health == null && attack == null) return;

        bool wasCollected = false;

        if (type == PickupType.Health && health != null)
        {
            wasCollected = health.Heal(amount);
        }
        else if (type == PickupType.Ammo && attack != null)
        {
            attack.AddAmmo(amount);
            wasCollected = true;
        }

        if (wasCollected)
        {
            Destroy(gameObject);
        }
    }
}