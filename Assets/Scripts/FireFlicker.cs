using UnityEngine;

public class FireFlicker : MonoBehaviour
{
    [SerializeField]
    private Light fireLight;

    [SerializeField]
    private float minIntensity = 32f;

    [SerializeField]
    private float maxIntensity = 45f;

    [SerializeField]
    private float noiseSpeed = 7f;

    [SerializeField]
    private float smoothing = 2.5f;

    private float targetIntensity;

    void Start()
    {
        if (fireLight == null)
        {
            fireLight = GetComponent<Light>();
        }
        targetIntensity = fireLight.intensity;
    }

    void Update()
    {
        if (fireLight != null)
        {
            float noiseValue = Mathf.PerlinNoise(Time.time * noiseSpeed, 0);

            targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, noiseValue);

            fireLight.intensity = Mathf.Lerp(fireLight.intensity, targetIntensity, Time.deltaTime * smoothing);
        }
    }
}