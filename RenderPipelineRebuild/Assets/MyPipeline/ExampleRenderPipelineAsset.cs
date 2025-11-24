using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

[CreateAssetMenu(menuName = "Rendering/ExampleRenderPipelineAsset")]
public class ExampleRenderPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline() {
        return new ExampleRenderPipelineInstance(this);
    }
}

public class ExampleRenderPipelineInstance : RenderPipeline
{
    private ExampleRenderPipelineAsset renderPipelineAsset;
    public ExampleRenderPipelineInstance(ExampleRenderPipelineAsset asset) {
        renderPipelineAsset = asset;
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        var cmd = new CommandBuffer();
        cmd.ClearRenderTarget(true, true, Color.black);
        context.ExecuteCommandBuffer(cmd);
        cmd.Release();
        
        foreach (Camera camera in cameras)
        {
            // 获取剔除参数
            camera.TryGetCullingParameters(out var cullingParameters);
            // 执行剔除
            var cullingResults = context.Cull(ref cullingParameters);
            // 更新built-in shader的变量
            context.SetupCameraProperties(camera);
            // 根据LightMode Pass tag value区分那些几何体要绘制
            ShaderTagId shaderTagId = new ShaderTagId("ExampleLightModeTag");
            // 分类几何体
            var sortingSettings = new SortingSettings(camera);
            // 创建一个DrawingSettings，它包含了那些几何体要绘制
            DrawingSettings drawingSettings = new DrawingSettings(shaderTagId, sortingSettings);
            // 再次过滤剔除结果，这里用默认
            // Use FilteringSettings.defaultValue to specify no filtering
            FilteringSettings filteringSettings = FilteringSettings.defaultValue;
            // 根据上面的剔除，生成一个Command
            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
            
            // 安排天空球绘制
            if (camera.clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null)
            {
                context.DrawSkybox(camera);
            }

            // 我们已经完成了指令装填，下面把他提交
            context.Submit();
        }
    }
}

// public class MyRenderPipeline : RenderPipeline
// {
//     RenderGraph m_RenderGraph;
//
//     void InitializeRenderGraph()
//     {
//         m_RenderGraph = new RenderGraph("MyRenderGraph");
//     }
//
//     void CleanupRenderGraph()
//     {
//         m_RenderGraph.Cleanup();
//         m_RenderGraph = null;
//     }
//     
//     class MyRenderPassData
//     {
//         public float parameter;
//         public TextureHandle inputTexture;
//         public TextureHandle outputTexture;
//     }
//     
//     TextureHandle MyRenderPass(RenderGraph renderGraph, TextureHandle inputTexture, float parameter, Material material)
//     {
//         using (var builder = renderGraph.AddRenderPass<MyRenderPassData>("My Render Pass", out var passData))
//         {
//             passData.parameter = parameter;
//             passData.material = material;
//
//             // Tells the graph that this pass will read inputTexture.
//             passData.inputTexture = builder.ReadTexture(inputTexture);
//
//             // Creates the output texture.
//             TextureHandle output = renderGraph.CreateTexture(new TextureDesc(Vector2.one, true, true)
//                 { colorFormat = GraphicsFormat.R8G8B8A8_UNorm, clearBuffer = true, clearColor = Color.black, name = "Output" });
//             // Tells the graph that this pass will write this texture and needs to be set as render target 0.
//             passData.outputTexture = builder.UseColorBuffer(output, 0);
//
//             builder.SetRenderFunc(
//                 (MyRenderPassData data, RenderGraphContext ctx) =>
//                 {
//                     // Render Target is already set via the use of UseColorBuffer above.
//                     // If builder.WriteTexture was used, you'd need to do something like that:
//                     // CoreUtils.SetRenderTarget(ctx.cmd, data.output);
//
//                     // Setup material for rendering
//                     var materialPropertyBlock = ctx.renderGraphPool.GetTempMaterialPropertyBlock();
//                     materialPropertyBlock.SetTexture("_MainTexture", data.input);
//                     materialPropertyBlock.SetFloat("_FloatParam", data.parameter);
//
//                     CoreUtils.DrawFullScreen(ctx.cmd, data.material, materialPropertyBlock);
//                 });
//
//             return output;
//         }
//     }
//
//     protected override void Render(ScriptableRenderContext context, Camera[] cameras)
//     {
//         
//     }
// }
