﻿/*
    Copyright (c) Microsoft Corporation All rights reserved.  
 
    MIT License: 
 
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
    documentation files (the  "Software"), to deal in the Software without restriction, including without limitation
    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
    and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
 
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
 
    THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Basel;
using Basel.Detection;
using Basel.Detection.Detectors;
using Basel.Detection.Recognizer;
using Basel.Detection.Recognizer.Dollar;
using Basel.Recorder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TileEvents;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace BandSlider
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application, INotifyPropertyChanged
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        public event PropertyChangedEventHandler PropertyChanged;

        public IDataRecorder Recorder { get; set; }
        public IDataPlayer Player { get; set; }
        public IRecord CurrentRecord { get; set; }
        public IRecognizer Recognizer { get; set; }
        public ISensorDataProducer Producer { get; set; }
        public SliderCtrlClient SliderCtrl { get; set; }
        public AccelerometerGestureDetector PPDetector { get; set; }

        public Frame Root { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            Accelerometer = true;
            SliderCtrl = new SliderCtrlClient("http://localhost:5000");
        }

        public static new App Current
        {
            get { return (App)Application.Current; }
        }

        private string _statusMessage = "Pair a Microsoft Band with your device and click Run.";
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { UpdatePropertyField(ref _statusMessage, value, "StatusMessage"); }
        }

        private string _filePath = @"";
        public string FilePath
        {
            get { return _filePath; }
            set { UpdatePropertyField(ref _filePath, value, "FilePath"); }
        }

        private ObservableCollection<IGesture> _gestures = new ObservableCollection<IGesture>();
        public ObservableCollection<IGesture> Gestures
        {
            get { return _gestures; }
            set
            {
                UpdatePropertyField(ref _gestures, value, "Gestures");
            }
        }

        private bool _playing;
        public bool Playing
        {
            get { return _playing; }
            set { UpdatePropertyField(ref _playing, value, "Playing"); }
        }

        private bool _useTextButton = true;
        public bool UseTextButton
        {
            get { return _useTextButton; }
            set
            {
                ResetButtonPropertiesIf(value);
                UpdatePropertyField(ref _useTextButton, value, "UseTextButton");
            }
        }

        private bool _useFilledButton = false;
        public bool UseFilledButton
        {
            get { return _useFilledButton; }
            set
            {
                ResetButtonPropertiesIf(value);
                UpdatePropertyField(ref _useFilledButton, value, "UseFilledButton");
            }
        }

        private bool _useIconButton = false;
        public bool UseIconButton
        {
            get { return _useIconButton; }
            set
            {
                ResetButtonPropertiesIf(value);
                UpdatePropertyField(ref _useIconButton, value, "UseIconButton");
            }
        }

        #region Consig
        private BaselConfiguration _config = new BaselConfiguration();
        public BaselConfiguration Config
        {
            get { return _config; }
            set
            {
                UpdatePropertyField(ref _config, value);
            }
        }


        //Beschleunigungsmesser 
        public bool Accelerometer
        {
            get { return _config.Accelerometer; }
            set
            {
                _config.Accelerometer = value;
                RaisPropertyChanged();
            }
        }

        //Höhenmesser
        public bool Altimeter
        {
            get { return _config.Altimeter; }
            set
            {
                _config.Altimeter = value;
                RaisPropertyChanged();
            }
        }

        //Umgebunslicht
        public bool AmbientLight
        {
            get { return _config.AmbientLight; }
            set
            {
                _config.AmbientLight = value;
                RaisPropertyChanged();
            }
        }

        //Luftdruckmesser
        public bool Barometer
        {
            get { return _config.Barometer; }
            set
            {
                _config.Barometer = value;
                RaisPropertyChanged();
            }
        }

        //Kaloriemesser
        public bool Calories
        {
            get { return _config.Calories; }
            set
            {
                _config.Calories = value;
                RaisPropertyChanged();
            }
        }

        //Band kontakt
        public bool Contact
        {
            get { return _config.Contact; }
            set
            {
                _config.Contact = value;
                RaisPropertyChanged();
            }
        }

        //Entfernungsmesser
        public bool Distance
        {
            get { return _config.Distance; }
            set
            {
                _config.Distance = value;
                RaisPropertyChanged();
            }
        }

        //Galvanischer Hautsensor
        public bool Gsr
        {
            get { return _config.Gsr; }
            set
            {
                _config.Gsr = value;
                RaisPropertyChanged();
            }
        }

        //Kreisel
        public bool Gyroscope
        {
            get { return _config.Gyroscope; }
            set
            {
                _config.Gyroscope = value;
                RaisPropertyChanged();
            }
        }

        //Herzfrequenz
        public bool HeartRate
        {
            get { return _config.HeartRate; }
            set
            {
                _config.HeartRate = value;
                RaisPropertyChanged();
            }
        }

        //Schrittzähler
        public bool Pedometer
        {
            get { return _config.Pedometer; }
            set
            {
                _config.Pedometer = value;
                RaisPropertyChanged();
            }
        }

        //Der Abstand zwischen zwei Herzschlägen 
        public bool RRInterval
        {
            get { return _config.RRInterval; }
            set
            {
                _config.RRInterval = value;
                RaisPropertyChanged();
            }
        }

        //Hauttemperatur
        public bool SkinTemperature
        {
            get { return _config.SkinTemperature; }
            set
            {
                _config.SkinTemperature = value;
                RaisPropertyChanged();
            }
        }

        //Uv
        public bool UV
        {
            get { return _config.UV; }
            set
            {
                _config.UV = value;
                RaisPropertyChanged();
            }
        }



        #endregion

        private void RaisPropertyChanged([CallerMemberName]string fieldName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(fieldName));
            }
        }

        private void UpdatePropertyField<T>(ref T field, T value, [CallerMemberName]string fieldName = null)
        {
            field = value;
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(fieldName));
            }
        }



        private void ResetButtonPropertiesIf(bool value)
        {
            if (value)
            {
                _useTextButton = _useFilledButton = _useIconButton = false;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            Root = rootFrame;
            // Ensure the current window is active
            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}