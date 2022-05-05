﻿using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps.iOS;
using Xamarin.Forms.Platform.iOS;

namespace XFGoogleMapSample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();

            // Override default ImageFactory by your implementation. 
            var platformConfig = new PlatformConfig
            {
                ImageFactory = new CachingImageFactory()
            };

            Xamarin.FormsGoogleMaps.Init(Variables.GOOGLE_MAPS_IOS_API_KEY, platformConfig);

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
