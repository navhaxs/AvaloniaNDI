using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Skia.Helpers;
using Avalonia.Skia;
using Avalonia.Threading;
using NAudio.Wave;
using NewTek;
using NewTek.NDI;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform;
using System.Diagnostics;

namespace AvaloniaNDI
{
    public class NDISendContainer : Viewbox, INotifyPropertyChanged, IDisposable
    {
        [Category("NewTek NDI"),
        Description("NDI output width in pixels. Required.")]
        public int NdiWidth
        {
            get { return _NdiWidth; }
            set { SetAndRaise(NdiWidthProperty, ref _NdiWidth, value); }
        }
        private int _NdiWidth = 1280;
        public static readonly DirectProperty<NDISendContainer, int> NdiWidthProperty =
            AvaloniaProperty.RegisterDirect<NDISendContainer, int>(nameof(NdiWidth), o => o.NdiWidth, (o, v) => o.NdiWidth = v);

        [Category("NewTek NDI"),
        Description("NDI output height in pixels. Required.")]
        public int NdiHeight
        {
            get { return _NdiHeight; }
            set { SetAndRaise(NdiHeightProperty, ref _NdiHeight, value); }
        }
        private int _NdiHeight = 720;
        public static readonly DirectProperty<NDISendContainer, int> NdiHeightProperty =
            AvaloniaProperty.RegisterDirect<NDISendContainer, int>(nameof(NdiHeight), o => o.NdiHeight, (o, v) => o.NdiHeight = v);


        [Category("NewTek NDI"),
        Description("NDI output frame rate numerator. Required.")]
        public int NdiFrameRateNumerator
        {
            get { return _NdiFrameRateNumerator; }
            set { SetAndRaise(NdiFrameRateNumeratorProperty, ref _NdiFrameRateNumerator, value); }
        }
        private int _NdiFrameRateNumerator = 60000;
        public static readonly DirectProperty<NDISendContainer, int> NdiFrameRateNumeratorProperty =
            AvaloniaProperty.RegisterDirect<NDISendContainer, int>(nameof(NdiFrameRateNumerator), o => o.NdiFrameRateNumerator, (o, v) => o.NdiFrameRateNumerator = v);

        [Category("NewTek NDI"),
        Description("NDI output frame rate denominator. Required.")]
        public int NdiFrameRateDenominator
        {
            get { return _NdiFrameRateDenominator; }
            set { SetAndRaise(NdiFrameRateDenominatorProperty, ref _NdiFrameRateDenominator, value); }
        }
        private int _NdiFrameRateDenominator = 1000;
        public static readonly DirectProperty<NDISendContainer, int> NdiFrameRateDenominatorProperty =
            AvaloniaProperty.RegisterDirect<NDISendContainer, int>(nameof(NdiFrameRateDenominator), o => o.NdiFrameRateDenominator, (o, v) => o.NdiFrameRateDenominator = v);


        [Category("NewTek NDI"),
        Description("NDI output name as displayed to receivers. Required.")]
        public string NdiName
        {
            get { return _NdiName; }
            set { SetAndRaise(NdiNameProperty, ref _NdiName, value); }
        }
        private string _NdiName = "Unnamed - Fix Me.";
        public static readonly DirectProperty<NDISendContainer, string> NdiNameProperty =
            AvaloniaProperty.RegisterDirect<NDISendContainer, string>(nameof(NdiName), o => o.NdiName, (o, v) => { o.NdiName = v; });

        [Category("NewTek NDI"),
Description("Function to determine whether the content requires high resolution NDI frame updates (i.e. is an animation or video playback). Optional.")]
        public Func<NDISendContainer, bool> IsContentHighResCheckFunc
        {
            get { return _IsContentHighResCheckFunc; }
            set { SetAndRaise(IsContentHighResCheckFuncProperty, ref _IsContentHighResCheckFunc, value); }
        }
        private Func<NDISendContainer, bool> _IsContentHighResCheckFunc = null;
        public static readonly DirectProperty<NDISendContainer, Func<NDISendContainer, bool>> IsContentHighResCheckFuncProperty =
            AvaloniaProperty.RegisterDirect<NDISendContainer, Func<NDISendContainer, bool>>(nameof(IsContentHighResCheckFunc), o => o.IsContentHighResCheckFunc, (o, v) => { o.IsContentHighResCheckFunc = v; });

        [Category("NewTek NDI"),
        Description("NDI groups this sender will belong to. Optional.")]
        public List<string> NdiGroups
        {
            get { return _NdiGroups; }
            set { SetAndRaise(NdiGroupsProperty, ref _NdiGroups, value); }
        }
        private List<string> _NdiGroups = new List<string>();
        public static readonly DirectProperty<NDISendContainer, List<string>> NdiGroupsProperty =
            AvaloniaProperty.RegisterDirect<NDISendContainer, List<string>>(nameof(NdiGroups), o => o.NdiGroups, (o, v) => { o.NdiGroups = v; });

        [Category("NewTek NDI"),
        Description("If clocked to video, NDI will rate limit drawing to the specified frame rate. Defaults to true.")]
        public bool NdiClockToVideo
        {
            get { return _NdiClockToVideo; }
            set { SetAndRaise(NdiClockToVideoProperty, ref _NdiClockToVideo, value); }
        }
        private bool _NdiClockToVideo = true;
        public static readonly DirectProperty<NDISendContainer, bool> NdiClockToVideoProperty =
            AvaloniaProperty.RegisterDirect<NDISendContainer, bool>(nameof(NdiClockToVideo), o => o.NdiClockToVideo, (o, v) => { o.NdiClockToVideo = v; });

        [Category("NewTek NDI"),
        Description("True if some receiver has this source on program out.")]
        public bool IsOnProgram
        {
            get { return _IsOnProgram; }
            set { SetAndRaise(IsOnProgramProperty, ref _IsOnProgram, value); }
        }
        private bool _IsOnProgram = false;
        public static readonly DirectProperty<NDISendContainer, bool> IsOnProgramProperty =
            AvaloniaProperty.RegisterDirect<NDISendContainer, bool>(nameof(IsOnProgram), o => o.IsOnProgram, (o, v) => { o.IsOnProgram = v; });

        [Category("NewTek NDI"),
        Description("True if some receiver has this source on preview out.")]
        public bool IsOnPreview
        {
            get { return _IsOnPreview; }
            set { SetAndRaise(IsOnPreviewProperty, ref _IsOnPreview, value); }
        }
        private bool _IsOnPreview = false;
        public static readonly DirectProperty<NDISendContainer, bool> IsOnPreviewProperty =
            AvaloniaProperty.RegisterDirect<NDISendContainer, bool>(nameof(IsOnPreview), o => o.IsOnPreview, (o, v) => { o.IsOnPreview = v; });


        [Category("NewTek NDI"),
        Description("If True, the send thread does not send, taking no CPU time.")]
        public bool IsSendPaused
        {
            get { return isPausedValue; }
            set
            {
                if (value != isPausedValue)
                {
                    SetAndRaise(IsOnPreviewProperty, ref isPausedValue, value);
                }
            }
        }
        public static readonly DirectProperty<NDISendContainer, bool> IsSendPausedProperty =
            AvaloniaProperty.RegisterDirect<NDISendContainer, bool>(nameof(IsSendPaused), o => o.IsSendPaused, (o, v) => { o.IsSendPaused = v; });


        [Category("NewTek NDI"),
        Description("Send System Audio")]
        public bool SendSystemAudio
        {
            get { return sendSystemAudio; }
            set
            {
                if (value != sendSystemAudio)
                {
                    if (value)
                    {
                        try
                        {
                            audioCap = new WasapiLoopbackCapture();
                            audioCap.StartRecording();
                            audioSampleRate = audioCap.WaveFormat.SampleRate;
                            audioSampleSizeInBytes = audioCap.WaveFormat.BitsPerSample / 8;
                            audioNumChannels = audioCap.WaveFormat.Channels;

                            audioCap.DataAvailable += AudioCap_DataAvailable;
                        }
                        catch
                        {
                            // loopback capture may not be available on all systems
                            value = false;
                        }
                    }
                    else
                    {
                        if (audioCap != null)
                        {
                            if (audioCap.CaptureState == NAudio.CoreAudioApi.CaptureState.Capturing)
                            {
                                audioCap.StopRecording();

                                while (audioCap.CaptureState != NAudio.CoreAudioApi.CaptureState.Stopped)
                                {
                                    Thread.Sleep(10);
                                }
                            }

                            audioCap.Dispose();
                            audioCap = null;
                        }
                    }

                    sendSystemAudio = value;
                    NotifyPropertyChanged("SendSystemAudio");
                }
            }
        }

        private void AudioCap_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (isPausedValue || sendInstancePtr == IntPtr.Zero)
                return;

            // how many samples?
            int numSamples = (e.BytesRecorded / (audioNumChannels * audioSampleSizeInBytes));

            // how much float buffer will this need?
            int bufferSizeNeeded = numSamples * audioNumChannels * sizeof(float);

            // is our audio frame big enough? too big is fine
            if (audioBufferSize < bufferSizeNeeded || audioFrame.p_data == IntPtr.Zero)
            {
                if (audioFrame.p_data != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(audioFrame.p_data);

                    audioFrame.p_data = IntPtr.Zero;
                }

                audioFrame.p_data = Marshal.AllocHGlobal(bufferSizeNeeded);

                audioBufferSize = bufferSizeNeeded;
            }

            // set these every time because why not?
            audioFrame.sample_rate = audioSampleRate;
            audioFrame.no_channels = audioNumChannels;
            audioFrame.no_samples = numSamples;

            // pin the byte[] audio received and get a GC handle to it
            GCHandle interleavedHandle = GCHandle.Alloc(e.Buffer, GCHandleType.Pinned);

            if (audioSampleSizeInBytes == 2)
            {
                // make an temporary interleaved NDI audio frame around the received samples
                NDIlib.audio_frame_interleaved_16s_t interleavedShortFrame = new NDIlib.audio_frame_interleaved_16s_t()
                {
                    sample_rate = audioSampleRate,
                    no_channels = audioNumChannels,
                    no_samples = numSamples,
                    p_data = interleavedHandle.AddrOfPinnedObject()
                };

                // Convert from s16 interleaved to float planar audio
                NDIlib.util_audio_from_interleaved_16s_v2(ref interleavedShortFrame, ref audioFrame);
            }
            else if (audioSampleSizeInBytes == 4)
            {
                // make an temporary interleaved NDI audio frame around the received samples
                NDIlib.audio_frame_interleaved_32f_t interleavedFloatFrame = new NDIlib.audio_frame_interleaved_32f_t()
                {
                    sample_rate = audioSampleRate,
                    no_channels = audioNumChannels,
                    no_samples = numSamples,
                    p_data = interleavedHandle.AddrOfPinnedObject()
                };

                // Convert from float interleaved to float planar audio
                NDIlib.util_audio_from_interleaved_32f_v2(ref interleavedFloatFrame, ref audioFrame);
            }
            else
            {
                Debug.Assert(false, "Unexpected audio sample size.");
            }

            // release the GC pinning of the byte[]'s
            interleavedHandle.Free();

            Monitor.Enter(sendInstanceLock);

            // send the planar frame
            if (sendInstancePtr != IntPtr.Zero)
            {
                if (!IsSendPaused)
                {
                    NDIlib.send_send_audio_v2(sendInstancePtr, ref audioFrame);
                }
            }

            Monitor.Exit(sendInstanceLock);
        }

        // TODO this was from WPF. Is there an Avalonia way to do this?
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public NDISendContainer()
        {
            if (Design.IsDesignMode)
                return;

            // start up a thread to receive on
            sendThread = new Thread(SendThreadProc) { IsBackground = true, Name = "WpfNdiSendThread" };
            sendThread.Start();

            _loop = Task.Factory.StartNew(() =>
            {
                Arguments args;
                while (true && _disposed == false)
                {
                    if (_argQueue.TryDequeue(out args))
                    {
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            OnCompositionTargetRendering();
                        });
                    }

                    // TODO: 2x here is a hack for testing
                    // (1/60fps)*1000 = 16.67 ms
                    Thread.Sleep(TimeSpan.FromMilliseconds(2 * 16.67d));
                }
            });

            this.LayoutUpdated += NDISendContainer_LayoutUpdated;

            try
            {
                // Not required, but "correct". (see the SDK documentation)
                if (!NDIlib.initialize())
                {
                    // Cannot run NDI. Most likely because the CPU is not sufficient (see SDK documentation).
                    // you can check this directly with a call to NDIlib.is_supported_CPU()
                    //MessageBox.Show("Cannot run NDI");
                }

                NdiNameProperty.Changed.Subscribe(OnNdiSenderPropertyChanged);
                NdiGroupsProperty.Changed.Subscribe(OnNdiSenderPropertyChanged);
                NdiClockToVideoProperty.Changed.Subscribe(OnNdiSenderPropertyChanged);

                InitializeNdi();
            }
            catch (Exception)
            {
                // Cannot run NDI. Most likely because the CPU is not sufficient (see SDK documentation).
                // you can check this directly with a call to NDIlib.is_supported_CPU()
                //MessageBox.Show("Cannot run NDI");
                throw;
            }

            this.AttachedToVisualTree += NDISendContainer_AttachedToVisualTree;
            this.DetachedFromVisualTree += NDISendContainer_DetachedFromVisualTree;

        }

        private void NDISendContainer_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            GetNextRenderTick();
        }

        void GetNextRenderTick()
        {
            if (!_disposed)
            {
                Window.GetTopLevel(this).RequestAnimationFrame((TimeSpan s) =>
                {
                    _argQueue.Enqueue(new Arguments() { });
                    GetNextRenderTick();
            });
            }
        }

        private void NDISendContainer_LayoutUpdated(object sender, EventArgs e)
        {
        }
        private void NDISendContainer_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            if (Design.IsDesignMode)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~NDISendContainer()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // clean up the audio capture if needed
                if (audioCap != null)
                {
                    audioCap.StopRecording();

                    // have to let it stop
                    while (audioCap.CaptureState != NAudio.CoreAudioApi.CaptureState.Stopped)
                    {
                        Thread.Sleep(10);
                    }

                    audioCap.Dispose();
                    audioCap = null;
                }

                // free allocated frame if needed
                if (audioFrame.p_data != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(audioFrame.p_data);
                    audioFrame.p_data = IntPtr.Zero;
                }

                if (disposing)
                {
                    // tell the thread to exit
                    exitThread = true;

                    // wait for it to exit
                    if (sendThread != null)
                    {
                        sendThread.Join();

                        sendThread = null;
                    }

                    // cause the pulling of frames to fail
                    pendingFrames.CompleteAdding();

                    // clear any pending frames
                    while (pendingFrames.Count > 0)
                    {
                        NDIlib.video_frame_v2_t discardFrame = pendingFrames.Take();
                        Marshal.FreeHGlobal(discardFrame.p_data);
                    }

                    pendingFrames.Dispose();
                }

                // Destroy the NDI sender
                if (sendInstancePtr != IntPtr.Zero)
                {
                    NDIlib.send_destroy(sendInstancePtr);

                    sendInstancePtr = IntPtr.Zero;
                }

                try
                {
                    // Not required, but "correct". (see the SDK documentation)
                    NDIlib.destroy();
                }
                catch (DllNotFoundException) { }

                _loop = null;

                _disposed = true;
            }
        }

        private bool _disposed = false;
        private class Arguments
        {
            public int coolInt;
            public double whatever;
            public string stringyThing;
        }

        private ConcurrentQueue<Arguments> _argQueue = new ConcurrentQueue<Arguments>();
        private Task _loop;


        public void ThrottledFunction(TimeSpan t)
        {
            _argQueue.Enqueue(new Arguments() { });
        }

        RenderTargetBitmap rtb;

        private unsafe void OnCompositionTargetRendering()
        {
            if (IsSendPaused)
                return;

#if DEBUG
            if (Design.IsDesignMode)
                return;
#endif

            // skip if UI thread has pending render jobs (fixes blinking/flashing empty frames)
            if (Dispatcher.UIThread.HasJobsWithPriority(DispatcherPriority.Render))
                return;

            // TODO: cache this value so its not called on *every* call
            if (NDIlib.send_get_no_connections(sendInstancePtr, 0) == 0)
                return;

            if (this.Child == null)
                return;

            int xres = NdiWidth;
            int yres = NdiHeight;

            int frNum = NdiFrameRateNumerator;
            int frDen = NdiFrameRateDenominator;

            // sanity
            if (sendInstancePtr == IntPtr.Zero || xres < 8 || yres < 8)
                return;

            if (rtb == null || rtb.PixelSize.Width != xres || rtb.PixelSize.Height != yres)
            {
                // Create a properly sized RenderTargetBitmap
                var scale = 1; // VisualRoot!.RenderScaling;
                rtb = new RenderTargetBitmap(new PixelSize(xres, yres), new Vector(96 * scale, 96 * scale));
            }

            stride = (xres * 32/*BGRA bpp*/ + 7) / 8;
            bufferSize = yres * stride;
            aspectRatio = (float)xres / (float)yres;

            // allocate some memory for a video buffer
            IntPtr bufferPtr = Marshal.AllocCoTaskMem(bufferSize);

            // We are going to create a progressive frame at 60Hz.
            NDIlib.video_frame_v2_t videoFrame = new NDIlib.video_frame_v2_t()
            {
                // Resolution
                xres = NdiWidth,
                yres = NdiHeight,
                // Use BGRA video
                FourCC = NDIlib.FourCC_type_e.FourCC_type_BGRA,
                // The frame-eate
                frame_rate_N = frNum,
                frame_rate_D = frDen,
                // The aspect ratio
                picture_aspect_ratio = aspectRatio,
                // This is a progressive frame
                frame_format_type = NDIlib.frame_format_type_e.frame_format_type_progressive,
                // Timecode.
                timecode = NDIlib.send_timecode_synthesize,
                // The video memory used for this frame
                p_data = bufferPtr,
                // The line to line stride of this image
                line_stride_in_bytes = stride,
                // no metadata
                p_metadata = IntPtr.Zero,
                // only valid on received frames
                timestamp = 0
            };

            // define the surface properties
            var info = new SKImageInfo(xres, yres);

            // construct a surface around the existing memory
            var destinationSurface = SKSurface.Create(info, bufferPtr, info.RowBytes);

            // get the canvas from the surface
            var destinationCanvas = destinationSurface.Canvas;
            using IDrawingContextImpl iHaveTheDestination = DrawingContextHelper.WrapSkiaCanvas(destinationCanvas, SkiaPlatform.DefaultDpi);

            // render the Avalonia visual
            rtb.Render(this.Child);

            rtb.CopyPixels(new PixelRect(0, 0, xres, yres), bufferPtr, bufferSize, stride);

            // add it to the output queue
            AddFrame(videoFrame);
        }

        private static void OnNdiSenderPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            NDISendContainer s = e.Sender as NDISendContainer;
            if (s != null)
                s.InitializeNdi();
        }

        private void InitializeNdi()
        {
            if (Design.IsDesignMode)
                return;

            Monitor.Enter(sendInstanceLock);

            {
                // we need a name
                if (String.IsNullOrEmpty(NdiName))
                    return;

                // re-initialize?
                if (sendInstancePtr != IntPtr.Zero)
                {
                    NDIlib.send_destroy(sendInstancePtr);
                    sendInstancePtr = IntPtr.Zero;
                }

                // .Net interop doesn't handle UTF-8 strings, so do it manually
                // These must be freed later
                IntPtr sourceNamePtr = UTF.StringToUtf8(NdiName);

                IntPtr groupsNamePtr = IntPtr.Zero;

                // build a comma separated list of groups?
                if (NdiGroups.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < NdiGroups.Count(); i++)
                    {
                        sb.Append(NdiGroups[i]);

                        if (i < NdiGroups.Count - 1)
                            sb.Append(',');
                    }

                    groupsNamePtr = UTF.StringToUtf8(sb.ToString());
                }

                // Create an NDI source description using sourceNamePtr and it's clocked to the video.
                NDIlib.send_create_t createDesc = new NDIlib.send_create_t()
                {
                    p_ndi_name = sourceNamePtr,
                    p_groups = groupsNamePtr,
                    clock_video = NdiClockToVideo,
                    clock_audio = false
                };

                // We create the NDI finder instance
                sendInstancePtr = NDIlib.send_create(ref createDesc);

                // free the strings we allocated
                Marshal.FreeHGlobal(sourceNamePtr);
                Marshal.FreeHGlobal(groupsNamePtr);

                // unlock
                Monitor.Exit(sendInstanceLock);
            }
        }

        // the receive thread runs though this loop until told to exit
        private void SendThreadProc()
        {
            // look for changes in tally
            bool lastProg = false;
            bool lastPrev = false;

            NDIlib.tally_t tally = new NDIlib.tally_t();
            tally.on_program = lastProg;
            tally.on_preview = lastPrev;

            while (!exitThread)
            {
                if (Monitor.TryEnter(sendInstanceLock))
                {
                    // if this is not here, then we must be being reconfigured
                    if (sendInstancePtr == IntPtr.Zero)
                    {
                        // unlock
                        Monitor.Exit(sendInstanceLock);

                        // give up some time
                        Thread.Sleep(20);

                        // loop again
                        continue;
                    }

                    try
                    {
                        // get the next available frame
                        NDIlib.video_frame_v2_t frame;
                        if (pendingFrames.TryTake(out frame, 250))
                        {
                            // this dropps frames if the UI is rendernig ahead of the specified NDI frame rate
                            while (pendingFrames.Count > 1)
                            {
                                NDIlib.video_frame_v2_t discardFrame = pendingFrames.Take();
                                Marshal.FreeHGlobal(discardFrame.p_data);
                            }

                            // We now submit the frame. Note that this call will be clocked so that we end up submitting 
                            // at exactly the requested rate.
                            // If WPF can't keep up with what you requested of NDI, then it will be sent at the rate WPF is rendering.
                            if (!IsSendPaused)
                            {
                                NDIlib.send_send_video_v2(sendInstancePtr, ref frame);
                            }

                            // free the memory from this frame
                            Marshal.FreeHGlobal(frame.p_data);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        pendingFrames.CompleteAdding();
                    }
                    catch
                    {
                    }

                    // unlock
                    Monitor.Exit(sendInstanceLock);
                }
                else
                {
                    Thread.Sleep(20);
                }

                // check tally
                NDIlib.send_get_tally(sendInstancePtr, ref tally, 0);

                // if tally changed trigger an update
                if (lastProg != tally.on_program || lastPrev != tally.on_preview)
                {
                    // save the last values
                    lastProg = tally.on_program;
                    lastPrev = tally.on_preview;

                    // set these on the UI thread
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        IsOnProgram = lastProg;
                        IsOnPreview = lastPrev;
                    });
                }
            }
        }

        public bool AddFrame(NDIlib.video_frame_v2_t frame)
        {
            try
            {
                pendingFrames.Add(frame);
            }
            catch (OperationCanceledException)
            {
                // we're shutting down
                pendingFrames.CompleteAdding();
                return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private Object sendInstanceLock = new Object();
        private IntPtr sendInstancePtr = IntPtr.Zero;

        //RenderTargetBitmap iHaveTheTargetBitmap = null;

        private int stride;
        private int bufferSize;
        private float aspectRatio;

        // a thread to send frames on so that the UI isn't dragged down
        Thread sendThread = null;

        // a way to exit the thread safely
        bool exitThread = false;

        // a thread safe collection to store pending frames
        BlockingCollection<NDIlib.video_frame_v2_t> pendingFrames = new BlockingCollection<NDIlib.video_frame_v2_t>();

        // used for pausing the send thread
        bool isPausedValue = false;

        // should we send system audio with the video?
        bool sendSystemAudio = false;

        // a capture device to grab system audio
        WasapiLoopbackCapture audioCap = null;

        // basic description of the audio stream
        int audioSampleRate = 48000;
        int audioSampleSizeInBytes = 4;
        int audioNumChannels = 2;

        // an audio frame to reuse
        NDIlib.audio_frame_v2_t audioFrame = new NDIlib.audio_frame_v2_t()
        {
            sample_rate = 48000,
            no_channels = 2,
            no_samples = 0,
            timecode = NDIlib.send_timecode_synthesize,
            p_data = IntPtr.Zero,
            channel_stride_in_bytes = 0,
            p_metadata = IntPtr.Zero,
            timestamp = 0
        };

        // the size of the allocated audioFrame.p_data
        int audioBufferSize = 0;
    }
}
