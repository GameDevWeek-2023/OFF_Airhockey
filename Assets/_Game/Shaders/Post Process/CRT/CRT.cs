using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[PostProcess(typeof(CRTRenderer), PostProcessEvent.AfterStack, "Custom/CRT")]
public class CRT : PostProcessEffectSettings {
    [Tooltip("describes the strength of CRT distortion.")]
    public FloatParameter bend = new FloatParameter() { value = 2.8f };

    [Header("Vignette")] [Range(0.01f, 4f), Tooltip("how much each corner should be rounded.")]
    public FloatParameter rounding = new FloatParameter() { value = 0.15f };

    [Range(1f, 4f), Tooltip("Smoothness of Vignette effect")]
    public FloatParameter smoothness = new FloatParameter() { value = 1.15f };

    [Header("Chromatic aberration")]
    [Range(1, 30), Tooltip("How many sample should be taken to calculate the blur effect.")]
    public IntParameter sample = new IntParameter() { value = 15 };

    [Tooltip("to offset where the sample should be taken.")]
    public FloatParameter offset = new FloatParameter() { value = 0f };

    [Tooltip("Scale of blur effect.")] public FloatParameter scale = new FloatParameter() { value = 0.01f };

    [Range(0, 1), Tooltip("weight of blur effect")]
    public FloatParameter weight = new FloatParameter() { value = 1f };

    [Header("Scanlines")] public FloatParameter frequency = new FloatParameter() { value = 300 };
    [Range(0.01f, 1f)] public FloatParameter amplitude = new FloatParameter();
    [Range(0f, 1f)] public FloatParameter strength = new FloatParameter() { value = 1f };
}

public sealed class CRTRenderer : PostProcessEffectRenderer<CRT> {
    private static readonly int _Bend = Shader.PropertyToID("_Bend");
    private static readonly int _CornerInset = Shader.PropertyToID("_CornerInset");
    private static readonly int _Smoothness = Shader.PropertyToID("_Smoothness");
    private static readonly int _Samples = Shader.PropertyToID("_Samples");
    private static readonly int _Offset = Shader.PropertyToID("_Offset");
    private static readonly int _Scale = Shader.PropertyToID("_Scale");
    private static readonly int _Weight = Shader.PropertyToID("_Weight");
    private static readonly int _Frequency = Shader.PropertyToID("_Frequency");
    private static readonly int _Amplitude = Shader.PropertyToID("_Amplitude");
    private static readonly int _Resolution = Shader.PropertyToID("_Resolution");
    private static readonly int _ScanlineStrength = Shader.PropertyToID("_ScanlineStrength");

    public override void Render(PostProcessRenderContext context) {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/CRT"));
        sheet.properties.SetFloat(_Bend, settings.bend);

        sheet.properties.SetFloat(_CornerInset, settings.rounding);
        sheet.properties.SetFloat(_Smoothness, settings.smoothness);

        sheet.properties.SetInt(_Samples, settings.sample);
        sheet.properties.SetFloat(_Offset, settings.offset);
        sheet.properties.SetFloat(_Scale, settings.scale);
        sheet.properties.SetFloat(_Weight, settings.weight);

        sheet.properties.SetFloat(_Frequency, settings.frequency);
        sheet.properties.SetFloat(_Amplitude, settings.amplitude);
        sheet.properties.SetFloat(_ScanlineStrength, settings.strength);

        sheet.properties.SetVector(_Resolution, new Vector2(context.screenWidth, context.screenHeight));
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}