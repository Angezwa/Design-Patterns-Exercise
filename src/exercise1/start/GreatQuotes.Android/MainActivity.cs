using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using GreatQuotes.ViewModels;
using System.Collections.ObjectModel;

namespace GreatQuotes.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    // MainActivity.cs

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly SimpleContainer container = new SimpleContainer();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            container.Register<IQuoteLoader, QuoteLoader>();
            container.Register<ITextToSpeech, TextToSpeechService>();
            container.Create<QuoteManager>();

            ServiceLocator.Instance.Add<ITextToSpeech, TextToSpeechService>();

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}