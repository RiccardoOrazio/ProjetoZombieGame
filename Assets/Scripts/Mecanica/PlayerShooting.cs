using UnityEngine;
using UnityEngine.VFX;
using TMPro;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private VisualEffect muzzleFlash;

    [Header("Configurações do Tiro")]
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float distanciaDoPontoTiro = 2.5f;

    [Header("Configurações de Munição")]
    [SerializeField] private int clipSize = 8;
    [SerializeField] private int startingAmmo = 32;
    [SerializeField] private int maxAmmo = 999;
    [SerializeField] private float reloadTime = 1.5f;

    [Header("Referências da UI")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadText;
    [SerializeField] private float reloadTextAnimationSpeed = 0.3f;

    public bool CanShoot { get; set; } = true;

    private AimController aimController;
    private Animator animator;

    private int currentClipAmmo;
    private int currentTotalAmmo;
    private bool isReloading = false;

    void Awake()
    {
        aimController = GetComponent<AimController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentClipAmmo = clipSize;
        currentTotalAmmo = startingAmmo;
        isReloading = false;
        CanShoot = true;
        UpdateAmmoUI();

        if (reloadText != null)
        {
            reloadText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive)
        {
            return;
        }

        PosicionarPontoDeTiro();

        if (isReloading)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && currentClipAmmo < clipSize && currentTotalAmmo > 0)
        {
            StartReload();
            return;
        }

        if (Input.GetMouseButtonDown(0) && CanShoot)
        {
            if (currentClipAmmo > 0)
            {
                Shoot();
            }
            else if (currentTotalAmmo > 0)
            {
                StartReload();
            }
            else
            {
                Debug.Log("CLIQUE! (Sem munição)");
            }
        }
    }

    private void PosicionarPontoDeTiro()
    {
        Vector3 aimDirection = aimController.AimDirection;
        Vector3 playerCenterWithOffset = transform.position;
        Vector3 offset = aimDirection * distanciaDoPontoTiro;

        firePoint.position = new Vector3(
            playerCenterWithOffset.x + offset.x,
            1.55f,
            playerCenterWithOffset.z + offset.z
        );
    }

    private void StartReload()
    {
        if (isReloading) return;

        StartCoroutine(ReloadSequence());
    }

    private IEnumerator ReloadSequence()
    {
        isReloading = true;
        CanShoot = false;
        Debug.Log("Recarregando...");

        if (reloadText != null)
        {
            reloadText.gameObject.SetActive(true);
            StartCoroutine(AnimateReloadText());
        }

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = clipSize - currentClipAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentTotalAmmo);

        currentClipAmmo += ammoToReload;
        currentTotalAmmo -= ammoToReload;

        isReloading = false;
        CanShoot = true;
        UpdateAmmoUI();
        Debug.Log("Recarga completa!");

        if (reloadText != null)
        {
            reloadText.gameObject.SetActive(false);
        }
    }

    private IEnumerator AnimateReloadText()
    {
        string baseText = "Recarregando";
        while (isReloading)
        {
            reloadText.text = baseText + ".";
            yield return new WaitForSeconds(reloadTextAnimationSpeed);
            if (!isReloading) break;

            reloadText.text = baseText + "..";
            yield return new WaitForSeconds(reloadTextAnimationSpeed);
            if (!isReloading) break;

            reloadText.text = baseText + "...";
            yield return new WaitForSeconds(reloadTextAnimationSpeed);
        }
    }

    void Shoot()
    {
        currentClipAmmo--;
        UpdateAmmoUI();

        Vector3 finalDirection;
        Transform target = aimController.TargetedEnemy;

        if (target != null)
        {
            Vector3 targetPosition = target.position;
            Collider targetCollider = target.GetComponent<Collider>();
            if (targetCollider != null)
            {
                targetPosition = targetCollider.bounds.center;
            }

            finalDirection = (targetPosition - firePoint.position).normalized;
        }
        else if (aimController.AimDirection.sqrMagnitude > 0.01f)
        {
            finalDirection = aimController.AimDirection;
        }
        else
        {
            return;
        }

        CanShoot = false;
        animator.SetTrigger("Shoot");

        firePoint.rotation = Quaternion.LookRotation(finalDirection);

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = finalDirection * projectileSpeed;
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentClipAmmo} / {currentTotalAmmo}";
        }
    }

    public void AddAmmo(int amount)
    {
        currentTotalAmmo += amount;

        if (currentTotalAmmo > maxAmmo)
        {
            currentTotalAmmo = maxAmmo;
        }
        UpdateAmmoUI();
    }
}