using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Light Glow")]
public class CustomBloom : PostEffectsBase
{
    [SerializeField]
    bool visualizeBrightFilter;

    public Shader customBloom;
    private Material customBloomMaterial;

    public Shader fastBlur;
    private Material fastBlurMaterial;

    public Shader customBrightCap;
    private Material customBrightCapMaterial;

    [Range(0, 1)]
    public float threshold;

    public override bool CheckResources()
    {
        //  CheckSupport(false);
        customBloomMaterial = CheckShaderAndCreateMaterial(customBloom, customBloomMaterial);
        customBrightCapMaterial = CheckShaderAndCreateMaterial(customBrightCap, customBrightCapMaterial);
        fastBlurMaterial = CheckShaderAndCreateMaterial(fastBlur, fastBlurMaterial);

        return isSupported;
    }
    [Range(0, 2)]
    public int downsample = 1;

    public enum BlurType
    {
        StandardGauss = 0,
        SgxGauss = 1,
    }

    [Range(0.0f, 10.0f)]
    public float blurSize = 3.0f;

    [Range(1, 4)]
    public int blurIterations = 2;

    public BlurType blurType = BlurType.StandardGauss;

    [SerializeField]
    private float intensity;

    private void MobileBlur(RenderTexture source, RenderTexture destination)
    {
        float widthMod = 1.0f / (1.0f * (1 << downsample));

        fastBlurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));
        source.filterMode = FilterMode.Bilinear;

        int rtW = source.width >> downsample;
        int rtH = source.height >> downsample;

        // downsample
        RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);

        rt.filterMode = FilterMode.Bilinear;
        Graphics.Blit(source, rt, fastBlurMaterial, 0);

        var passOffs = blurType == BlurType.StandardGauss ? 0 : 2;

        for (int i = 0; i < blurIterations; i++)
        {
            float iterationOffs = (i * 1.0f);
            fastBlurMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

            // vertical blur
            RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, fastBlurMaterial, 1 + passOffs);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;

            // horizontal blur
            rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, fastBlurMaterial, 2 + passOffs);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }

        Graphics.Blit(rt, destination);

        RenderTexture.ReleaseTemporary(rt);
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!CheckResources())
        {
            Graphics.Blit(source, destination);
            return;
        }

        var rtW2 = source.width / 2;
        var rtH2 = source.height / 2;
        var rtW4 = source.width / 4;
        var rtH4 = source.height / 4;
        float widthOverHeight = (1.0f * source.width) / (1.0f * source.height);

        RenderTexture original = RenderTexture.GetTemporary(source.width, source.height, 0);
        Graphics.Blit(source, original);

        RenderTexture threshTex = RenderTexture.GetTemporary(rtW2, rtH2, 0);

        BrightFilter(source, threshTex, threshold);


        RenderTexture blurTex = RenderTexture.GetTemporary(source.width, source.height, 0);
        MobileBlur(threshTex, blurTex);

        if (visualizeBrightFilter)
        {
            Graphics.Blit(blurTex, destination);
            RenderTexture.ReleaseTemporary(threshTex);
            RenderTexture.ReleaseTemporary(blurTex);
            RenderTexture.ReleaseTemporary(original);
            return;
        }

        Blend(original, blurTex, destination);

        RenderTexture.ReleaseTemporary(threshTex);
        RenderTexture.ReleaseTemporary(blurTex);
        RenderTexture.ReleaseTemporary(original);
    }

    private void Blend(RenderTexture source, RenderTexture blurTex, RenderTexture destination)
    {
        customBloomMaterial.SetFloat("_Intensity", intensity);
        customBloomMaterial.SetTexture("_ColorBuffer", blurTex);
        Graphics.Blit(source, destination, customBloomMaterial);
    }

    private void BrightFilter(RenderTexture source, RenderTexture destination, float threshold)
    {
        customBrightCapMaterial.SetFloat("_Threshold", threshold);
        Graphics.Blit(source, destination, customBrightCapMaterial);
    }
}
