﻿using System;
using System.Diagnostics; 
using lang = Windows.ApplicationModel;  
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;   
using Infoécran.Classes; 
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Infoécran.AppStates;

namespace Infoécran
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {

            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame { CacheSize = 1 };

                // TODO: change this value to a cache size that is appropriate for your application

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

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

                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("StacktraceError"))
                {
                    await
                        Utilities.SendEmail("support-wp@paulfasola.fr",
                            ApplicationData.Current.LocalSettings.Values["StacktraceError"].ToString());
                    ApplicationData.Current.LocalSettings.Values.Remove("StacktraceError");
                }

                
                var statusBar = StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
                if(!ApplicationData.Current.LocalSettings.Values.ContainsKey("AppJustLaunched")) 
                    ApplicationData.Current.LocalSettings.Values.Add("AppJustLaunched", true);  
#endif

                if (e.Arguments == "")
                {
                    if (!rootFrame.Navigate(typeof(FirstLandingState)))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                }
                else
                {
                    await AppStateManager.SecondaryTileOnNavigatedTo(e.Arguments, rootFrame);
                }
            }

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
        private void OnSuspending(object sender, lang.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}