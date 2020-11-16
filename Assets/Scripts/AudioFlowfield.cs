using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(NoiseFlowfield))]
public class AudioFlowfield : MonoBehaviour
{
    NoiseFlowfield noiseFlowfield;
    public AudioPeer64 audioPeer;
    [Header("Speed")]
    public bool useSpeed;
    public Vector2 moveSpeedMinMax, rotateSpeedMinMax;
    [Header("Scale")]
    public bool useScale;
    public Vector2 scaleMinMax;
    [Header("Material")]
    public Material material;
    private Material[] audioMaterial;
    // First color pattern
    public bool useColor1;
    public string colorName1 = "_Color";
    public Gradient gradient1;
    private Color[] color1;
    [Range(0f, 1f)]
    public float colorThreshold1;
    public float colorMultiplier1 = 1.0f;
    public bool useColor2;
    public string colorName2 = "_EmissionColor";
    public Gradient gradient2;
    private Color[] color2;
    [Range(0f, 1f)]
    public float colorThreshold2;
    public float colorMultiplier2 = 1.0f;
    // Second color pattern
    public bool useColor3;
    public string colorName3 = "_Color";
    public Gradient gradient3;
    private Color[] color3;
    [Range(0f, 1f)]
    public float colorThreshold3;
    public float colorMultiplier3 = 1.0f;
    public bool useColor4;
    public string colorName4 = "_EmissionColor";
    public Gradient gradient4;
    private Color[] color4;
    [Range(0f, 1f)]
    public float colorThreshold4;
    public float colorMultiplier4 = 1.0f;
    // Third color pattern
    public bool useColor5;
    public string colorName5 = "_Color";
    public Gradient gradient5;
    private Color[] color5;
    [Range(0f, 1f)]
    public float colorThreshold5;
    public float colorMultiplier5 = 1.0f;
    public bool useColor6;
    public string colorName6 = "_EmissionColor";
    public Gradient gradient6;
    private Color[] color6;
    [Range(0f, 1f)]
    public float colorThreshold6;
    public float colorMultiplier6 = 1.0f;

    float duration = 1;
    // Change color every this amount of seconds.
    [SerializeField]
    float breakTime = 3.0f;

    float breakIdx = 0f;

    float time = 0f;
    

    // Start is called before the first frame update
    void Start()
    {
        noiseFlowfield = GetComponent<NoiseFlowfield>();
        audioMaterial = new Material[8];
        color1 = new Color[8];
        color2 = new Color[8];
        color3 = new Color[8];
        color4 = new Color[8];
        color5 = new Color[8];
        color6 = new Color[8];
        for (int i = 0; i < 8; i++)
        {
            color1[i] = gradient1.Evaluate((1f / 8f) * i);
            color2[i] = gradient2.Evaluate((1f / 8f) * i);
            color3[i] = gradient3.Evaluate((1f / 8f) * i);
            color4[i] = gradient4.Evaluate((1f / 8f) * i);
            color5[i] = gradient5.Evaluate((1f / 8f) * i);
            color6[i] = gradient6.Evaluate((1f / 8f) * i);
            audioMaterial[i] = new Material(material);
        }

        int countBand = 0;
        for (int i = 0; i < noiseFlowfield.amountOfParticles; i++)
        {
            int band = countBand % 8;
            noiseFlowfield.particleMeshRenderer[i].material = audioMaterial[band];
            noiseFlowfield.particles[i].audioBand = band;
            countBand++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime / duration;
        
        if (time > breakTime * breakIdx)
        {
            breakIdx++;
        }

        if (useSpeed)
        {
            noiseFlowfield.particleMoveSpeed = Mathf.Lerp(moveSpeedMinMax.x, moveSpeedMinMax.y, audioPeer.AmplitudeBuffer);
            noiseFlowfield.particleRotateSpeed = Mathf.Lerp(rotateSpeedMinMax.x, moveSpeedMinMax.y, audioPeer.AmplitudeBuffer);
        }
        for (int i = 0; i < noiseFlowfield.amountOfParticles; i++)
        {
            if (useScale)
            {
                float scale = Mathf.Lerp(scaleMinMax.x, scaleMinMax.y, audioPeer.audioBandBuffer[noiseFlowfield.particles[i].audioBand]);
                if (!float.IsNaN(scale)) {
                    noiseFlowfield.particles[i].transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
        for (int i = 0; i < 8; i++) {
            if (useColor1 && (0 == breakIdx % 3))
            {
                if (audioPeer.audioBandBuffer[i] > colorThreshold1)
                {
                    audioMaterial[i].SetColor(colorName1, brightenVal(color1[i] * audioPeer.audioBandBuffer[i] * colorMultiplier1));
                }
                else
                {
                    audioMaterial[i].SetColor(colorName1, color1[i] * 0.0f);
                }
            }
            if (useColor2 && (0 == breakIdx % 3))
            {
                if (audioPeer.audioBandBuffer[i] > colorThreshold2)
                {
                    audioMaterial[i].SetColor(colorName2, brightenVal(color2[i] * audioPeer.audioBandBuffer[i] * colorMultiplier2));
                }
                else
                {
                    audioMaterial[i].SetColor(colorName2, color2[i] * 0.0f);
                }
            }
            if (useColor3 && (1 == breakIdx % 3))
            {
                if (audioPeer.audioBandBuffer[i] > colorThreshold3)
                {
                    audioMaterial[i].SetColor(colorName3, brightenVal(color3[i] * audioPeer.audioBandBuffer[i] * colorMultiplier3));
                }
                else
                {
                    audioMaterial[i].SetColor(colorName3, color3[i] * 0.0f);
                }
            }
            if (useColor4 && (1 == breakIdx % 3))
            {
                if (audioPeer.audioBandBuffer[i] > colorThreshold4)
                {
                    audioMaterial[i].SetColor(colorName4, brightenVal(color4[i] * audioPeer.audioBandBuffer[i] * colorMultiplier4));

                }
                else
                {
                    audioMaterial[i].SetColor(colorName4, color4[i] * 0.0f);
                }
            }
            if (useColor5 && (2 == breakIdx % 3))
            {
                if (audioPeer.audioBandBuffer[i] > colorThreshold3)
                {
                    audioMaterial[i].SetColor(colorName5, brightenVal(color5[i] * audioPeer.audioBandBuffer[i] * colorMultiplier5));
                }
                else
                {
                    audioMaterial[i].SetColor(colorName5, color5[i] * 0.0f);
                }
            }
            if (useColor6 && (2 == breakIdx % 3))
            {
                if (audioPeer.audioBandBuffer[i] > colorThreshold6)
                {
                    audioMaterial[i].SetColor(colorName6, brightenVal(color6[i] * audioPeer.audioBandBuffer[i] * colorMultiplier6));
                }
                else
                {
                    audioMaterial[i].SetColor(colorName6, color6[i] * 0.0f);
                }
            }
        }
    }

    private Color brightenVal(Color input) {
        Color c = input;
        // c.a = (c.a < 0.3f) ? c.a + 0.5f : c.a;
        // c.r = (c.r < 0.3f) ? c.r + 0.2f : c.r;
        // c.g = (c.g < 0.3f) ? c.g + 0.5f : c.g;
        // c.b = (c.b < 0.3f) ? c.b + 0.5f : c.b;
        // c.a = (c.a < 0.3f) ? 1.0f : c.a;
        // c.r = (c.r < 0.3f) ? 1.0f : c.r;
        // c.g = (c.g < 0.3f) ? 1.0f : c.g;
        // c.b = (c.b < 0.3f) ? 1.0f : c.b;
        return c;
    }
}
