using Avalonia.Rendering;
using Avalonia.Threading;
using System;

namespace AvaloniaNDI
{
    class NDIRenderLoopTask : IRenderLoopTask
    {
        private Action renderFunc;
        private Action updateFunc;

        public NDIRenderLoopTask(Action renderFunc = null, Action updateFunc = null)
        {
            this.renderFunc = renderFunc;
            this.updateFunc = updateFunc;
        }

        public bool NeedsUpdate => true;

        public void Render()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                renderFunc?.Invoke();
            });
        }

        public void Update(TimeSpan time)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                updateFunc?.Invoke();
            });
        }
    }
}
