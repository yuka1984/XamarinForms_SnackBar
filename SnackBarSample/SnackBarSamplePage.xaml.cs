using System;
using Xamarin.Forms;

namespace SnackBarSample
{
    public partial class SnackBarSamplePage : ContentPage
    {

        public SnackBarSamplePage()
        {
            InitializeComponent();
            SnackButton.Clicked += ButtonOnClicked;
#if __IOS__
             snackbar = new SnackBarSample.iOS.TouchSnackBar();
#else
            snackbar = new SnackBarSample.Droid.AndroidSnackBar();
#endif
		}
        private ISnackBar snackbar = null;

        private void ButtonOnClicked(object sender, EventArgs eventArgs)
        {
            snackbar.Show("Description" + DateTime.Now.ToString(), 2500, "Click", () => { DisplayAlert("alert", "click", "close"); });
        }
    }
}
