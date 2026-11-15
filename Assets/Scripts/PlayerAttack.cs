using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración de Munición")]
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private int currentAmmo = 15;
    [SerializeField] private int maxAmmo = 30;

    [Header("Parámetros de Disparo")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject visualArrow;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float arrowForce = 40f;
    [SerializeField] private float attackCooldown = 0.8f;
    [SerializeField] private LayerMask ignoreLayers;

    [Header("Cantidad de Flechas en el Pool")]
    [SerializeField] private int poolSize = 5;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;

    [Header("Eventos de UI")]
    public UnityEvent<int, int> OnAmmoChanged;

    private List<Arrow> arrowPool = new List<Arrow>();
    private int currentArrowIndex = 0;
    private float nextAttackTime = 0f;
    private bool isLoaded = true;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        InitPool();
    }

    private void InitPool()
    {
        if (arrowPrefab == null)
        {
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            Arrow instance = Instantiate(arrowPrefab);
            instance.gameObject.SetActive(false);
            arrowPool.Add(instance);
        }
    }

    private void Start()
    {
        ReloadVisualArrow();
        NotifyAmmoUI();
    }

    private void Update()
    {
        if (playerHealth != null && playerHealth.IsDead) return;

        if (!isLoaded && Time.time >= nextAttackTime)
        {
            ReloadVisualArrow();
        }

        if (isLoaded && IsAttackPressed())
        {
            Shoot();
        }
    }

    private bool IsAttackPressed()
    {
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    }

    private void Shoot()
    {
        if (currentAmmo <= 0 || firePoint == null || arrowPool.Count == 0)
        {
            return;
        }

        isLoaded = false;
        nextAttackTime = Time.time + attackCooldown;

        currentAmmo--;
        NotifyAmmoUI();

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        if (visualArrow != null)
        {
            visualArrow.SetActive(false);
        }

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, 100f, ~ignoreLayers, QueryTriggerInteraction.Ignore)
            ? hit.point
            : ray.GetPoint(100f);

        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

        Arrow arrowToShoot = arrowPool[currentArrowIndex];
        if (arrowToShoot == null)
        {
            arrowToShoot = Instantiate(arrowPrefab);
            arrowPool[currentArrowIndex] = arrowToShoot;
        }
        else if (arrowToShoot.gameObject.activeSelf)
        {
            arrowToShoot.ForceReturnToPool();
        }

        arrowToShoot.Launch(firePoint.position, Quaternion.LookRotation(shootDirection), shootDirection * arrowForce);
        currentArrowIndex = (currentArrowIndex + 1) % poolSize;
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(maxAmmo, currentAmmo + amount);
        NotifyAmmoUI();
    }

    private void ReloadVisualArrow()
    {
        if (currentAmmo > 0)
        {
            isLoaded = true;
            if (visualArrow != null)
            {
                visualArrow.SetActive(true);
            }
        }
    }

    private void NotifyAmmoUI()
    {
        OnAmmoChanged?.Invoke(currentAmmo, maxAmmo);
    }
}