using UnityEngine;
using System.Collections.Generic;

public class ObstacleHider : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Transform playerTransform;
    [SerializeField, Range(0f, 1f)] private float targetAlpha = 0.2f;
    [SerializeField] private float fadeSpeed = 6f;
    [SerializeField] private float maxDistanceToHide = 15f;

    private Shader transparentShader;

    private class OriginalState
    {
        public Material[] sharedMaterials;
    }

    private Dictionary<Renderer, OriginalState> originalStates = new Dictionary<Renderer, OriginalState>();
    private Dictionary<Renderer, float> currentAlphas = new Dictionary<Renderer, float>();

    private Dictionary<Light, float> originalLights = new Dictionary<Light, float>();
    private Dictionary<Light, float> currentLightAlphas = new Dictionary<Light, float>();

    private HashSet<Component> componentsHitThisFrame = new HashSet<Component>();

    void Start()
    {
        if (playerTransform == null) enabled = false;

        transparentShader = Shader.Find("Universal Render Pipeline/Lit");
        if (transparentShader == null) transparentShader = Shader.Find("Standard");
    }

    void LateUpdate()
    {
        componentsHitThisFrame.Clear();

        Vector3 direction = playerTransform.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, obstacleLayer);

        foreach (RaycastHit hit in hits)
        {
            if (Vector3.Distance(hit.collider.transform.position, playerTransform.position) > maxDistanceToHide)
                continue;

            Renderer hitRenderer = hit.collider.GetComponent<Renderer>();
            if (hitRenderer != null) componentsHitThisFrame.Add(hitRenderer);

            Renderer[] childrenRenderers = hit.collider.GetComponentsInChildren<Renderer>();
            foreach (var r in childrenRenderers) componentsHitThisFrame.Add(r);

            Light[] childrenLights = hit.collider.GetComponentsInChildren<Light>();
            foreach (var l in childrenLights) componentsHitThisFrame.Add(l);
        }

        ProcessObjects();
    }

    private void ProcessObjects()
    {
        List<Renderer> trackedRenderers = new List<Renderer>(currentAlphas.Keys);
        foreach (Component comp in componentsHitThisFrame)
        {
            if (comp is Renderer r && !trackedRenderers.Contains(r)) trackedRenderers.Add(r);
        }

        List<Renderer> renderersToRemove = new List<Renderer>();
        foreach (Renderer rend in trackedRenderers)
        {
            if (rend == null) { renderersToRemove.Add(rend); continue; }

            bool shouldHide = componentsHitThisFrame.Contains(rend);
            float target = shouldHide ? targetAlpha : 1f;

            if (!currentAlphas.ContainsKey(rend))
            {
                currentAlphas[rend] = 1f;
                StoreOriginalState(rend);
                ReplaceWithTransparentMaterial(rend);
            }

            float newAlpha = Mathf.MoveTowards(currentAlphas[rend], target, Time.deltaTime * fadeSpeed);
            currentAlphas[rend] = newAlpha;

            UpdateMaterialAlpha(rend, newAlpha);

            if (!shouldHide && Mathf.Approximately(newAlpha, 1f))
            {
                RestoreOriginalState(rend);
                renderersToRemove.Add(rend);
            }
        }
        foreach (var r in renderersToRemove) { currentAlphas.Remove(r); originalStates.Remove(r); }

        List<Light> trackedLights = new List<Light>(currentLightAlphas.Keys);
        foreach (Component comp in componentsHitThisFrame)
        {
            if (comp is Light l && !trackedLights.Contains(l)) trackedLights.Add(l);
        }

        List<Light> lightsToRemove = new List<Light>();
        foreach (Light light in trackedLights)
        {
            if (light == null) { lightsToRemove.Add(light); continue; }

            bool shouldHide = componentsHitThisFrame.Contains(light);
            float target = shouldHide ? 0f : 1f;

            if (!currentLightAlphas.ContainsKey(light))
            {
                currentLightAlphas[light] = 1f;
                originalLights[light] = light.intensity;
            }

            float newAlpha = Mathf.MoveTowards(currentLightAlphas[light], target, Time.deltaTime * fadeSpeed);
            currentLightAlphas[light] = newAlpha;
            light.intensity = Mathf.Lerp(0f, originalLights[light], newAlpha);

            if (!shouldHide && Mathf.Approximately(newAlpha, 1f))
            {
                lightsToRemove.Add(light);
            }
        }
        foreach (var l in lightsToRemove) { currentLightAlphas.Remove(l); originalLights.Remove(l); }
    }

    private void StoreOriginalState(Renderer rend)
    {
        if (!originalStates.ContainsKey(rend))
        {
            OriginalState state = new OriginalState();
            state.sharedMaterials = rend.sharedMaterials;
            originalStates[rend] = state;
        }
    }

    private void ReplaceWithTransparentMaterial(Renderer rend)
    {
        Material[] originalMats = rend.sharedMaterials;
        Material[] newMats = new Material[originalMats.Length];

        for (int i = 0; i < originalMats.Length; i++)
        {
            Material source = originalMats[i];
            Material tempMat = new Material(transparentShader);

            Texture mainTex = null;
            if (source.HasProperty("_BaseMap")) mainTex = source.GetTexture("_BaseMap");
            else if (source.HasProperty("_MainTex")) mainTex = source.GetTexture("_MainTex");
            else if (source.HasProperty("_Texture2D")) mainTex = source.GetTexture("_Texture2D");

            Color mainColor = Color.white;
            if (source.HasProperty("_BaseColor")) mainColor = source.GetColor("_BaseColor");
            else if (source.HasProperty("_Color")) mainColor = source.GetColor("_Color");

            mainColor.a = 1f;

            Color emissionColor = Color.black;
            if (source.HasProperty("_EmissionColor")) emissionColor = source.GetColor("_EmissionColor");
            else if (source.HasProperty("_Emission")) emissionColor = source.GetColor("_Emission");

            if (tempMat.HasProperty("_BaseMap")) tempMat.SetTexture("_BaseMap", mainTex);
            if (tempMat.HasProperty("_MainTex")) tempMat.SetTexture("_MainTex", mainTex);

            if (tempMat.HasProperty("_BaseColor")) tempMat.SetColor("_BaseColor", mainColor);
            if (tempMat.HasProperty("_Color")) tempMat.SetColor("_Color", mainColor);

            if (tempMat.HasProperty("_EmissionColor")) tempMat.SetColor("_EmissionColor", emissionColor);

            tempMat.SetFloat("_Surface", 1);
            tempMat.SetFloat("_Blend", 0);
            tempMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            tempMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            tempMat.SetInt("_ZWrite", 1);
            tempMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            tempMat.EnableKeyword("_EMISSION");
            tempMat.renderQueue = 3000;

            newMats[i] = tempMat;
        }

        rend.materials = newMats;
    }

    private void UpdateMaterialAlpha(Renderer rend, float alpha)
    {
        foreach (Material mat in rend.materials)
        {
            if (mat.HasProperty("_BaseColor"))
            {
                Color c = mat.GetColor("_BaseColor");
                c.a = alpha;
                mat.SetColor("_BaseColor", c);
            }
            else if (mat.HasProperty("_Color"))
            {
                Color c = mat.GetColor("_Color");
                c.a = alpha;
                mat.SetColor("_Color", c);
            }
        }

        if (originalStates.TryGetValue(rend, out OriginalState state))
        {
            Material[] currentMats = rend.materials;
            for (int i = 0; i < currentMats.Length && i < state.sharedMaterials.Length; i++)
            {
                Material original = state.sharedMaterials[i];
                Material current = currentMats[i];

                Color baseEmission = Color.black;
                if (original.HasProperty("_EmissionColor")) baseEmission = original.GetColor("_EmissionColor");
                else if (original.HasProperty("_Emission")) baseEmission = original.GetColor("_Emission");

                if (current.HasProperty("_EmissionColor"))
                {
                    current.SetColor("_EmissionColor", baseEmission * alpha);
                }
            }
        }
    }

    private void RestoreOriginalState(Renderer rend)
    {
        if (originalStates.ContainsKey(rend))
        {
            rend.sharedMaterials = originalStates[rend].sharedMaterials;
        }
    }
}