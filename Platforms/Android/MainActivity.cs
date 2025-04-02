using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace C971;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Check if the POST_NOTIFICATIONS permission is granted
        if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) != (int)Permission.Granted)
            {
                // Request the POST_NOTIFICATIONS permission
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.PostNotifications }, 100);
            }
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == 100 && grantResults.Length > 0 && grantResults[0] == Permission.Granted)
        {
            // The notification permission was granted
            Console.WriteLine("Notification permission granted.");
        }
        else
        {
            // Permission was denied
            Console.WriteLine("Notification permission denied.");
        }
    }
}