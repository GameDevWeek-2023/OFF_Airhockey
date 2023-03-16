using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace _Lab.Outline {
    [Serializable]
    [PostProcess(typeof(GaussBlurRenderer), PostProcessEvent.AfterStack, "Custom/Gauss Blur")]
    public sealed class GaussBlur : PostProcessEffectSettings {
        [Range(0f, 1f)] public FloatParameter intensity = new FloatParameter{value = 1f};
        public FloatParameter amount = new FloatParameter(){value = 0.25f};
        public FloatParameter standardDeviation = new FloatParameter(){value = 0.02f};
        public IntParameter step = new IntParameter(){value = 32};
    }

    public sealed class GaussBlurRenderer : PostProcessEffectRenderer<GaussBlur> {
        private static readonly int Intensity = Shader.PropertyToID("_Intensity");
        private static readonly int BlurAmount = Shader.PropertyToID("_BlurAmount");
        private static readonly int StandardDeviation = Shader.PropertyToID("_StandardDeviation");
        private static readonly int Step = Shader.PropertyToID("_Steps");


        public override void Render(PostProcessRenderContext context) {
            context.camera.depthTextureMode = DepthTextureMode.Depth;

            CommandBuffer command = context.command;
            
            command.BeginSample("GaussBlurPostEffect");
            
            var sheet = context.propertySheets.Get(Shader.Find("Custom/Hidden/GaussBlur"));
            sheet.properties.Clear();
            sheet.properties.SetFloat(Intensity, settings.intensity);
            sheet.properties.SetFloat(BlurAmount, settings.amount);
            sheet.properties.SetFloat(StandardDeviation, settings.standardDeviation);
            sheet.properties.SetInt(Step, settings.step);

            int horizontalPassId = 0;
            int verticalPassId = 1;
            
            int blurId = Shader.PropertyToID("_GaussBlurPostEffect");
            
            int rtId = Shader.PropertyToID("_GaussBlurPostEffect" + horizontalPassId);
            command.GetTemporaryRT(rtId, context.width, context.height, 0, FilterMode.Bilinear);
            command.BlitFullscreenTriangle(context.source, rtId, sheet, horizontalPassId);
            command.ReleaseTemporaryRT(blurId);
            blurId = rtId;
            
            rtId = Shader.PropertyToID("_GaussBlurPostEffect" + verticalPassId);
            command.GetTemporaryRT(rtId, context.width, context.height, 0, FilterMode.Bilinear);
            command.BlitFullscreenTriangle(blurId, rtId, sheet, verticalPassId);
            command.ReleaseTemporaryRT(blurId);
            blurId = rtId;
            
            command.Blit(blurId, context.destination);
            command.ReleaseTemporaryRT(blurId);
            
            command.EndSample("GaussBlurPostEffect");
        }
    }
}