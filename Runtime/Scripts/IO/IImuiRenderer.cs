using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Imui.IO
{
    public interface IImuiRenderer
    {
        Vector2 GetScreenSize();
        float GetScale();
        Vector2Int SetupRenderTarget(CommandBuffer cmd, bool needsDepth);
        void Schedule(IImuiRenderDelegate renderDelegate);
    }

    public interface IImuiRenderDelegate
    {
        void Render(IImuiRenderingContext context);
    }
    
    public interface IImuiRenderingScheduler: IDisposable
    {
        void Schedule(IImuiRenderDelegate renderDelegate);
    }

    public interface IImuiRenderingContext : IDisposable
    {
        CommandBuffer CreateCommandBuffer();
        void ReleaseCommandBuffer(CommandBuffer cmd);
        void ExecuteCommandBuffer(CommandBuffer cmd);
    }
}