using UnityEngine;
using System.Collections.Generic;

public class ObstacleHider : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform playerTransform;
    [SerializeField, Range(0f, 1f)] private float targetAlpha = 0.3f;
    [SerializeField] private float fadeSpeed = 6f;
    [SerializeField] private float maxDistanceToHide = 15f;

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private Dictionary<Renderer, float> currentAlphas = new Dictionary<Renderer, float>();
    private HashSet<Renderer> renderersHitThisFrame = new HashSet<Renderer>();

    void Start()
    {
        if (playerTransform == null) enabled = false;
    }

    void LateUpdate()
    {
        renderersHitThisFrame.Clear();

        Vector3 direction = playerTransform.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, obstacleLayer);

        foreach (RaycastHit hit in hits)
        {
            if (Vector3.Distance(hit.collider.transform.position, playerTransform.position) > maxDistanceToHide)
            {
                continue;
            }

            Renderer hitRenderer = hit.collider.GetComponent<Renderer>();
            if (hitRenderer != null)
            {
                renderersHitThisFrame.Add(hitRenderer);
            }

            Renderer[] children = hit.collider.GetComponentsInChildren<Renderer>();
            foreach (var r in children)
            {
                renderersHitThisFrame.Add(r);
            }
        }

        ProcessObjects();
    }

    private void ProcessObjects()
    {
        List<Renderer> allTrackedRenderers = new List<Renderer>(currentAlphas.Keys);

        foreach (Renderer rend in renderersHitThisFrame)
        {
            if (!allTrackedRenderers.Contains(rend))
            {
                allTrackedRenderers.Add(rend);
            }
        }

        List<Renderer> toRemove = new List<Renderer>();

        foreach (Renderer rend in allTrackedRenderers)
        {
            if (rend == null)
            {
                toRemove.Add(rend);
                continue;
            }

            bool shouldBeTransparent = renderersHitThisFrame.Contains(rend);
            float target = shouldBeTransparent ? targetAlpha : 1f;

            if (!currentAlphas.ContainsKey(rend))
            {
                currentAlphas[rend] = 1f;
                if (!originalMaterials.ContainsKey(rend))
                {
                    originalMaterials[rend] = rend.sharedMaterials;
                }
                SetupMaterial(rend);
            }

            float newAlpha = Mathf.MoveTowards(currentAlphas[rend], target, Time.deltaTime * fadeSpeed);
            currentAlphas[rend] = newAlpha;
            UpdateMaterialAlpha(rend, newAlpha);

            if (!shouldBeTransparent && Mathf.Approximately(newAlpha, 1f))
            {
                RestoreMaterial(rend);
                toRemove.Add(rend);
            }
        }

        foreach (var r in toRemove)
        {
            currentAlphas.Remove(r);
            if (originalMaterials.ContainsKey(r)) originalMaterials.Remove(r);
        }
    }

    private void SetupMaterial(Renderer rend)
    {
        Material[] mats = rend.materials;
        foreach (Material mat in mats)
        {
            mat.SetFloat("_Mode", 2);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1);
            if (mat.HasProperty("_Blend")) mat.SetFloat("_Blend", 0);
        }
        rend.materials = mats;
    }

    private void UpdateMaterialAlpha(Renderer rend, float alpha)
    {
        foreach (Material mat in rend.materials)
        {
            if (mat.HasProperty("_BaseColor"))
            {
                Color color = mat.GetColor("_BaseColor");
                color.a = alpha;
                mat.SetColor("_BaseColor", color);
            }
            else if (mat.HasProperty("_Color"))
            {
                Color color = mat.GetColor("_Color");
                color.a = alpha;
                mat.SetColor("_Color", color);
            }
        }
    }

    private void RestoreMaterial(Renderer rend)
    {
        if (originalMaterials.ContainsKey(rend))
        {
            rend.sharedMaterials = originalMaterials[rend];
        }
    }
}