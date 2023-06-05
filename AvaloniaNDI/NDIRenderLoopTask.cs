using Avalonia.Rendering;
using Avalonia.Threading;
using System;

namespace AvaloniaNDI
{
    class NDIRenderLoopTask : IRenderLoopTask
    {
        private Action<TimeSpan> onUpdateFunc;

        public NDIRenderLoopTask(Action<TimeSpan> onUpdateFunc = null)
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
                onUpdateFunc?.Invoke(time);
            });
        }
    }
}
