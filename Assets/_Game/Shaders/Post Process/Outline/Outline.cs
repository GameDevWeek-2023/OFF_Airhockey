using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace _Lab.Outline {
    [Serializable]
    [PostProcess(typeof(OutlineRenderer), PostProcessEvent.BeforeStack, "Custom/Outline")]
    public sealed class Outline : PostProcessEffectSettings {
        public ColorParameter color = new ColorParameter() { value = Color.black };
        public FloatParameter scale = new FloatParameter() { value = 1f };
        [Range(0, 1)] public FloatParameter normalThreshold = new FloatParameter() { value = 1f };
        [Range(0, 1)] public FloatParameter depthThreshold = new FloatParameter() { value = 1f };
        public FloatParameter depthNormalThreshold = new FloatParameter() { value = 1f };
        public FloatParameter depthNormalThresholdScale = new FloatParameter() { value = 1f };
    }

    public sealed class OutlineRenderer : PostProcessEffectRenderer<Outline> {
        private static readonly int m_Color = Shader.PropertyToID("_Color");
        private static readonly int m_Scale = Shader.PropertyToID("_Scale");
        private static readonly int m_NormalThreshold = Shader.PropertyToID("_NormalThreshold");
        private static readonly int m_DepthThreshold = Shader.PropertyToID("_DepthThreshold");
        private static readonly int m_ClipToView = Shader.PropertyToID("_ClipToView");
        private static readonly int m_DepthNormalThreshold = Shader.PropertyToID("_DepthNormalThreshold");
        private static readonly int m_DepthNormalThresholdScale = Shader.PropertyToID("_DepthNormalThresholdScale");

        public override void Render(PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get(Shader.Find("Custom/Hidden/Outline"));

            sheet.properties.SetColor(m_Color, settings.color);
            sheet.properties.SetFloat(m_Scale, settings.scale);
            sheet.properties.SetFloat(m_NormalThreshold, settings.normalThreshold);
            sheet.properties.SetFloat(m_DepthThreshold, settings.depthThreshold);
            sheet.properties.SetFloat(m_DepthNormalThreshold, settings.depthNormalThreshold);
            sheet.properties.SetFloat(m_DepthNormalThresholdScale, settings.depthNormalThresholdScale);
            
            Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, true).inverse;
            sheet.properties.SetMatrix(m_ClipToView, clipToView);
            
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}