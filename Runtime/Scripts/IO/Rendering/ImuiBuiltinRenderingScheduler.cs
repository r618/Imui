using Imui.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Imui.IO.Rendering
{
    public class ImuiBuiltinRenderingScheduler : IImuiRenderingScheduler, IImuiRenderingContext
    {
        private ImDynamicArray<CommandBuffer> commandBufferPool = new(2);
        
        public void Schedule(IImuiRenderDelegate renderDelegate)
        {
            renderDelegate.Render(this);
        }
        
        public CommandBuffer CreateCommandBuffer()
        {
            if (commandBufferPool.Count == 0)
            {
                var cmd = new CommandBuffer() { name = "Imui (Builtin)" };
                commandBufferPool.Add(cmd);
            }

            return commandBufferPool.Pop();
        }
        
        public void ReleaseCommandBuffer(CommandBuffer cmd)
        {
            cmd.Clear();
            commandBufferPool.Add(cmd);
        }
        
        public void ExecuteCommandBuffer(CommandBuffer cmd)
        {
            Graphics.ExecuteCommandBuffer(cmd);
        }

        public void Dispose()
        {
            for (int i = 0; i < commandBufferPool.Count; ++i)
            {
                commandBufferPool.Array[i].Release();
            }
            
            commandBufferPool.Clear(false);
        }
    }
}