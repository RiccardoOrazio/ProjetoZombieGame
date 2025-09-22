using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderInteraction2 : MonoBehaviour
{
    public Renderer shieldRenderer;

    private float health = 3f;

    private Material shieldMaterial;

    void Start()
    {
        shieldMaterial = shieldRenderer.material;
        shieldMaterial.SetFloat("_FloatWaterIntensity", 0.1f);
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            health--;
            UpdateWaterIntensity();
        }
    }

    private void UpdateWaterIntensity()
    {
        float intensity = 0f;

        switch (health)
        {
            case 3:
                intensity = 0.1f; 
                break;
            case 2:
                intensity = 0.3f; 
                break;
            case 1:
                intensity = 0.5f; 
                break;
            case 0:
                intensity = 0.7f; 
                break;
        }

        shieldMaterial.SetFloat("_FloatWaterIntensity", intensity);
    }
}
