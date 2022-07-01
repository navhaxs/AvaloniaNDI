using Avalonia.Rendering;
using Avalonia.Threading;
using System;

namespace AvaloniaNDI
{
    class NDIRenderLoopTask : IRenderLoopTask
    {
        private Action onUpdateFunc;

        public NDIRenderLoopTask(Action onUpdateFunc = null)
        {
            this.onUpdateFunc = onUpdateFunc;
        }

        public bool NeedsUpdate => true;

        public void Render()
        {

        }

        public void Update(TimeSpan time)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                onUpdateFunc?.Invoke();
            });
        }
    }
}
