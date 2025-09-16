using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderInteraction : MonoBehaviour
{
    public Renderer shieldRenderer;
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.red;

    float health = 3;

    Material shieldMaterial;
    void Start()
    {
        shieldMaterial = shieldRenderer.material;
        shieldMaterial.SetColor("_ShieldColor", healthyColor);
    }

    void Update()
    {
      if (Input.GetButtonDown("Jump"))
        {
            health--;
            ChageColor();
        }
       
    }

    private void ChageColor()
    {
        switch (health)
        {
            case 3:
            shieldMaterial.SetColor("_ShieldColor", healthyColor);
                break;
            case 2:
            shieldMaterial.SetColor("_ShieldColor", Color.blue);
                break;
            case 1:
            shieldMaterial.SetColor("_ShieldColor", damagedColor);
                break;

        }
    }
}
