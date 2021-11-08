using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaNDI.Sample
{
    public class AnimationsPageViewModel : ReactiveObject
    {
        private bool _isPlaying = true;

        private string _playStateText = "Pause animations on this page";

        public void TogglePlayState()
        {
            PlayStateText = _isPlaying
                ? "Resume animations on this page" : "Pause animations on this page";
            _isPlaying = !_isPlaying;
        }

        public string PlayStateText
        {
            get { return _playStateText; }
            set { this.RaiseAndSetIfChanged(ref _playStateText, value); }
        }
    }
}
