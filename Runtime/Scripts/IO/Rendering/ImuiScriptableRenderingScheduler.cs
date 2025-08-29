using System.Collections.Generic;
using Imui.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Imui.IO.Rendering
{
    public class ImuiScriptableRenderingScheduler : IImuiRenderingScheduler, IImuiRenderingContext
    {
        private Queue<IImuiRenderDelegate> queue = new(1);
        private ImDynamicArray<CommandBuffer> commandBufferPool = new(1);
        private ScriptableRenderContext activeContext;

        public ImuiScriptableRenderingScheduler()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }
        
        public void Schedule(IImuiRenderDelegate renderDelegate)
        {
            queue.Enqueue(renderDelegate);
        }
        
        public CommandBuffer CreateCommandBuffer()
        {
            if (commandBufferPool.Count == 0)
            {
                var cmd = new CommandBuffer() { name = "Imui (SRP)" };
                commandBufferPool.Add(cmd);
            }

            return commandBufferPool.Pop();
        }
        
        public void ReleaseCommandBuffer(CommandBuffer cmd)
        {
            cmd.Clear();
            commandBufferPool.Push(cmd);
        }

        public void ExecuteCommandBuffer(CommandBuffer cmd)
        {
            activeContext.ExecuteCommandBuffer(cmd);
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            activeContext = context;
            
            while (queue.TryDequeue(out var renderDelegate))
            {
                renderDelegate.Render(this);
            }

            activeContext = default;
        }
        
        public void Dispose()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            queue.Clear();
        }
    }
}