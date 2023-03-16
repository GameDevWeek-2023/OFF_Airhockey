using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace Ambient.Environment{
    [Serializable]
    [PostProcess(typeof(TiltShiftRenderer), PostProcessEvent.AfterStack, "Custom/Tilt Shift")]
    public sealed class TiltShift : PostProcessEffectSettings{
        [Range(0f, 1f)] public FloatParameter intensity = new FloatParameter{value = 1f};
        public FloatParameter amount = new FloatParameter(){value = 0.25f};
        public FloatParameter center = new FloatParameter(){value = 1.1f};
        public FloatParameter standardDeviation = new FloatParameter(){value = 0.02f};
        public IntParameter step = new IntParameter(){value = 3};
    }

    public sealed class TiltShiftRenderer : PostProcessEffectRenderer<TiltShift>{
        private static readonly int _Intensity = Shader.PropertyToID("_Intensity");
        private static readonly int _BlurAmount = Shader.PropertyToID("_BlurAmount");
        private static readonly int _StandardDeviation = Shader.PropertyToID("_StandardDeviation");
        private static readonly int _Center = Shader.PropertyToID("_Center");
        private static readonly int _Step = Shader.PropertyToID("_Steps");

        public override void Render(PostProcessRenderContext context) {

            context.camera.depthTextureMode = DepthTextureMode.Depth;

            CommandBuffer command = context.command;
            
            command.BeginSample("TiltShiftPostEffect");
            
            var sheet = context.propertySheets.Get(Shader.Find("Custom/Hidden/Tilt Shift"));
            sheet.properties.Clear();
            sheet.properties.SetFloat(_Intensity, settings.intensity);
            sheet.properties.SetFloat(_BlurAmount, settings.amount);
            sheet.properties.SetFloat(_StandardDeviation, settings.standardDeviation);
            sheet.properties.SetFloat(_Center, settings.center);
            sheet.properties.SetInt(_Step, settings.step);

            int horizontalPassId = 0;
            int verticalPassId = 1;
            
            int blurId = Shader.PropertyToID("_TiltShiftProcessEffect");
            
            int rtId = Shader.PropertyToID("_TiltShiftProcessEffect" + horizontalPassId);
            command.GetTemporaryRT(rtId, context.width, context.height, 0, FilterMode.Bilinear);
            command.BlitFullscreenTriangle(context.source, rtId, sheet, horizontalPassId);
            command.ReleaseTemporaryRT(blurId);
            blurId = rtId;
            
            rtId = Shader.PropertyToID("_TiltShiftProcessEffect" + verticalPassId);
            command.GetTemporaryRT(rtId, context.width, context.height, 0, FilterMode.Bilinear);
            command.BlitFullscreenTriangle(blurId, rtId, sheet, verticalPassId);
            command.ReleaseTemporaryRT(blurId);
            blurId = rtId;
            
            command.Blit(blurId, context.destination);
            command.ReleaseTemporaryRT(blurId);
            
            command.EndSample("TiltShiftPostEffect");
        }
    }
}