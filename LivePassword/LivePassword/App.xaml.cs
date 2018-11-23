using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using LivePassword.classes;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using LivePassword.Controls;
using Windows.ApplicationModel;
using LivePassword.Common;
using Windows.Security.Credentials;
namespace LivePassword
{
    public partial class App : Application
    {
        public const string KEY = "wfm31228223929367";
        public FileOpenPickerContinuationEventArgs FilePickerContinuationArgs { get; set; }
        public static pPassword password = new pPassword();
        public static ObservableCollection<string> suggestions = new ObservableCollection<string>();
        /// <summary>
        /// Component used to handle unhandle exceptions, to collect runtime info and to send email to developer.
        /// </summary>
        public RadDiagnostics diagnostics;
        /// <summary>
        /// Component used to remind end users about the trial state of the application.
        /// </summary>
        public RadTrialApplicationReminder trialReminder;

        /// <summary>
        /// Component used to raise a notification to the end users to rate the application on the marketplace.
        /// </summary>
        public static RadRateApplicationReminder rateReminder;

        public static string DatabasePath { get; private set; }

        public static bool MergeAvailable { get; set; }

        public static bool IsLoggedIn { get; set; }
        
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;
        //    PhoneApplicationService.Current.ContractActivated+=Current_ContractActivated;
            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();
         
            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            ThemeManager.OverrideOptions = ThemeManagerOverrideOptions.None;
            ThemeManager.ToLightTheme();
            ThemeManager.SetBackground(new SolidColorBrush(Color.FromArgb(0xFF, 0xec, 0xf0, 0xf1)));
            ThemeManager.SetAccentColor(Color.FromArgb(0xFF, 0x29, 0x80, 0xb9));

            //Creates an instance of the Diagnostics component.
            diagnostics = new RadDiagnostics();

            //Defines the default email where the diagnostics info will be send.
            diagnostics.EmailTo = "arrowgreen@live.it";

            //Initializes this instance.
            diagnostics.Init();
            //Creates an instance of the RadTrialApplicationReminder component.
            //trialReminder = new RadTrialApplicationReminder();

            ////Sets the lenght of the trial period.
            //trialReminder.AllowedTrialUsageCount = 3110;

            ////Sets how often the trial reminder is displayed.
            //trialReminder.OccurrenceUsageCount = 2;

            ////The reminder is shown only if the application is in trial mode. When this property is set to true the application will simulate that it is in trial mode.
            //trialReminder.SimulateTrialForTests = true;
        
              //Creates a new instance of the RadRateApplicationReminder component.
            rateReminder = new RadRateApplicationReminder();

            //Sets how often the rate reminder is displayed.
            rateReminder.RecurrencePerUsageCount = 3;
    
        }
        public static void ToastMessage(string title, string message)
        {
            Coding4Fun.Toolkit.Controls.ToastPrompt toast = new Coding4Fun.Toolkit.Controls.ToastPrompt();
            toast.Title = title;
            toast.Message = message;
            toast.ImageSource = new BitmapImage(new Uri("/Assets/ApplicationIcon.png", UriKind.RelativeOrAbsolute));
            toast.Stretch = System.Windows.Media.Stretch.Uniform;
            toast.ImageHeight = 25;
            toast.ImageWidth = 25;
            toast.Show();
        }
        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private async void Application_Launching(object sender, LaunchingEventArgs e)
        {
            //Before using any of the ApplicationBuildingBlocks, this class should be initialized with the version of the application.
            ApplicationUsageHelper.Init("1.0");

            //var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("database.sqlite", CreationCollisionOption.OpenIfExists);          
           
            var suggestion = await StorageHelper.LoadData("suggestion.dat", typeof(ObservableCollection<string>));
            if (suggestion!=null) 
                App.suggestions  = suggestion as ObservableCollection<string>;
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private async void Application_Activated(object sender, ActivatedEventArgs e)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("database.sqlite", CreationCollisionOption.OpenIfExists);
            DatabasePath = file.Path;

            if (!e.IsApplicationInstancePreserved)
            {
                //This will ensure that the ApplicationUsageHelper is initialized again if the application has been in Tombstoned state.
                ApplicationUsageHelper.OnApplicationActivated();

                IsLoggedIn = false;

                var suggestion = await StorageHelper.LoadData("suggestion.dat", typeof(ObservableCollection<string>));
                if (suggestion != null)
                    App.suggestions = suggestion as ObservableCollection<string>;
            }

            //App.mp = "M";
            PhoneApplicationService.Current.ContractActivated+=Current_ContractActivated;
        }

        private void Application_ContractActivated(object sender, IActivatedEventArgs e)
        {
            var filePickerContinuationArgs = e as FileOpenPickerContinuationEventArgs;
            if (filePickerContinuationArgs != null)
            {
                this.FilePickerContinuationArgs = filePickerContinuationArgs;
            }
        }


        public static string mp = "";
        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                    System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private async void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            //try
            //{
            //    var file = await Package.Current.InstalledLocation.GetFileAsync("Assets\\Test\\data.sdf");
            //    await file.CopyAsync(ApplicationData.Current.LocalFolder, "data.sdf", NameCollisionOption.ReplaceExisting);
            //}
            //catch { }

            //try
            //{
            //    var file = await Package.Current.InstalledLocation.GetFileAsync("Assets\\Test\\password.dat");
            //    await file.CopyAsync(ApplicationData.Current.LocalFolder, "password.dat", NameCollisionOption.ReplaceExisting);
            //}
            //catch { }

            try
            {
                var file = await Package.Current.InstalledLocation.GetFileAsync("Assets\\database.sqlite");
                await file.CopyAsync(ApplicationData.Current.LocalFolder, "database.sqlite", NameCollisionOption.FailIfExists);
                RemoveCredential();    
            }
            catch 
            {
            }

            var result = await ApplicationData.Current.LocalFolder.GetFileAsync("database.sqlite");
            DatabasePath = result.Path;

            PhoneApplicationService.Current.ContractActivated += Current_ContractActivated;
            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new LiveApplicationFrame();
            RootFrame.UriMapper = new AppUriMapper();
            RootFrame.Background = new SolidColorBrush(Color.FromArgb(255,236,240,241));
            RootFrame.Navigated += CompleteInitializePhoneApplication;
           
            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

      async  void Current_ContractActivated(object sender, IActivatedEventArgs e)
        {
            var args = e as FileSavePickerContinuationEventArgs;
            if (args != null)
            {
                StorageFile file = args.File;
                if (file != null)
                {
                    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync. 
                    CachedFileManager.DeferUpdates(file);
                    // write to file 
                    var name = args.ContinuationData["file"].ToString();
                    StorageFile sourceFile = await ApplicationData.Current.LocalFolder.GetFileAsync(name);

                    IRandomAccessStream sourceStream = await sourceFile.OpenReadAsync();

                    var bytes = new byte[sourceStream.Size];

                    var sourceBuffer = await sourceStream.ReadAsync(bytes.AsBuffer(), (uint)sourceStream.Size, Windows.Storage.Streams.InputStreamOptions.None);

                    await FileIO.WriteBufferAsync(file, sourceBuffer);
                    // Let Windows know that we're finished changing the file so the other app can update the remote version of the file. 
                    // Completing updates may require Windows to ask for user input. 
                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                }
            }
            var filePickerContinuationArgs = e as FileOpenPickerContinuationEventArgs;
            if (filePickerContinuationArgs != null)
            {
                this.FilePickerContinuationArgs = filePickerContinuationArgs;
            }

        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion  
        private void RemoveCredential()
        {
            var vault = new PasswordVault();
            try
            {
                vault.Remove(vault.Retrieve("Live Password", "Current User"));
            }
            catch { }
        }
    }
}
