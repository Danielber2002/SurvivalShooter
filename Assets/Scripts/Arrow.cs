using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Arrow : MonoBehaviour
{
    [Header("Configuración del Proyectil")]
    [SerializeField] private float damage = 5f;

    private Rigidbody rb;
    private Collider arrowCollider;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        arrowCollider = GetComponent<Collider>();
    }

    public void Launch(Vector3 position, Quaternion rotation, Vector3 force)
    {
        transform.SetParent(null);
        gameObject.SetActive(true);

        transform.position = position;
        transform.rotation = rotation;

        rb.isKinematic = false;
        arrowCollider.enabled = true;
        hasHit = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(force, ForceMode.Impulse);
    }

    private void Update()
    {
        if (!hasHit && rb.linearVelocity.sqrMagnitude > 1f)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
            ForceReturnToPool();
            return;
        }

        rb.isKinematic = true;
        arrowCollider.enabled = false;
        transform.SetParent(collision.transform);
    }

    public void ForceReturnToPool()
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
    }
}