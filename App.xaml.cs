using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("ubuntu-bold.ttf")]
[assembly: ExportFont("sulphur-bold.ttf")]
[assembly: ExportFont("sulphur-regular.ttf")]
[assembly: ExportFont("teko-regular.ttf")]
[assembly: ExportFont("teko-semibold.ttf")]

namespace Testery
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
