using System;
using Xamarin.Forms;
using Android.Support.Design.Widget;
using Android.Content.Res;
using Xamarin.Forms.Platform.Android;

namespace SnackBarSample.Droid
{
    public class AndroidSnackBar : ISnackBar
    {
		public static Android.Graphics.Color ActionColor { get; set; } = new Color(0.95f, 0.76f, 0.20f).ToAndroid();

        public void Show(string text, int duration, string actionText, Action action)
        {
			var activity = (Android.App.Activity)Forms.Context;
			var view = activity.FindViewById(Android.Resource.Id.Content);

            var snackbar = Snackbar.Make(view, text, duration);
            var snackAction = new Action<Android.Views.View>((Android.Views.View obj) => { action(); snackbar.SetDuration(0); });
            snackbar.SetAction(actionText, snackAction);
            snackbar.SetActionTextColor(ActionColor);
            snackbar.Show();
        }
    }
}
