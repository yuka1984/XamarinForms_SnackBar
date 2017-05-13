using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace SnackBarSample.iOS
{
    public class TouchSnackBar : ISnackBar
    {
        private static readonly List<Tuple<CancellationTokenSource, UIView>> _addedsnacks =
            new List<Tuple<CancellationTokenSource, UIView>>();

        public static int BoxHeighy { get; set; } = 50;

        public static UIColor SnackBackgroundColor { get; set; } = new UIColor(0, 0, 0, 0.8f);
        public static UIColor TextColor { get; set; } = UIColor.White;
        public static UIColor ActionColor { get; set; } = new UIColor(0.95f, 0.76f, 0.20f, 1f);
        public static double OpenDuration { get; set; } = 0.3;
        public static double CloseDuration { get; set; } = 0.2;

        public async void Show(string text, int duration, string actionText, Action action)
        {
            await ClearSnack();
            var window = UIApplication.SharedApplication.KeyWindow;
            var bounds = UIScreen.MainScreen.Bounds;

            var snackbase = new UIView();
            snackbase.BackgroundColor = SnackBackgroundColor;
            var label = new UILabel
            {
                Text = text,
                BackgroundColor = UIColor.Clear,
                TextColor = TextColor,
                TextAlignment = UITextAlignment.Left,
                AutoresizingMask = UIViewAutoresizing.All,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                Font = UIFont.SystemFontOfSize(13)
            };
            snackbase.AddSubview(label);

            var button = new UIButton();
            button.SetTitle(actionText.ToUpper(), UIControlState.Normal);
            button.SetTitleColor(ActionColor, UIControlState.Normal);
            button.Font = UIFont.SystemFontOfSize(12);
            snackbase.AddSubview(button);

            label.TopAnchor.ConstraintEqualTo(snackbase.TopAnchor, 5).Active = true;
            label.LeftAnchor.ConstraintEqualTo(snackbase.LeftAnchor, 20).Active = true;
            label.BottomAnchor.ConstraintEqualTo(snackbase.BottomAnchor, -5).Active = true;
            label.WidthAnchor.ConstraintEqualTo(snackbase.WidthAnchor, 0.85f, 0).Active = true;

            button.TopAnchor.ConstraintEqualTo(snackbase.TopAnchor).Active = true;
            button.RightAnchor.ConstraintEqualTo(snackbase.RightAnchor).Active = true;
            button.BottomAnchor.ConstraintEqualTo(snackbase.BottomAnchor).Active = true;
            button.WidthAnchor.ConstraintEqualTo(snackbase.WidthAnchor, 0.15f, 0).Active = true;

            label.TranslatesAutoresizingMaskIntoConstraints = false;
            button.TranslatesAutoresizingMaskIntoConstraints = false;

            window.AddSubview(snackbase);

            snackbase.TranslatesAutoresizingMaskIntoConstraints = false;

            snackbase.WidthAnchor.ConstraintEqualTo(window.WidthAnchor).Active = true;
            snackbase.HeightAnchor.ConstraintEqualTo(BoxHeighy).Active = true;
            snackbase.CenterXAnchor.ConstraintEqualTo(window.CenterXAnchor).Active = true;

            var initialTop = snackbase.TopAnchor.ConstraintEqualTo(window.BottomAnchor);
            var displayTop = snackbase.CenterYAnchor.ConstraintEqualTo(window.CenterYAnchor);

            initialTop.Active = true;

            window.LayoutIfNeeded();

            var cancel = new CancellationTokenSource();
            button.TouchUpInside += (sender, e) =>
            {
                cancel.Cancel();
                action();
            };

            Task.Delay(duration, cancel.Token)
                .ContinueWith(task =>
                {
                    if (task.IsCompleted || task.IsCanceled)
                    {
                        snackbase.Layer.RemoveAllAnimations();
                        snackbase.BeginInvokeOnMainThread(() =>
                        {
                            UIView.Animate(CloseDuration
                                , () =>
                                {
                                    initialTop.Constant = 0;
                                    window.LayoutIfNeeded();
                                }
                                , () =>
                                {
                                    snackbase.RemoveFromSuperview();
                                    var added = _addedsnacks.FirstOrDefault(x => x.Item2.Equals(snackbase));
                                    _addedsnacks.Remove(added);
                                }
                            );
                        });
                    }
                });

            UIView.Animate(OpenDuration, () =>
            {
                initialTop.Constant -= BoxHeighy;
                window.LayoutIfNeeded();
            });

            _addedsnacks.Add(new Tuple<CancellationTokenSource, UIView>(cancel, snackbase));
        }

        private async Task ClearSnack()
        {
            var count = _addedsnacks.Count;
            foreach (var added in _addedsnacks)
                added.Item1.Cancel();

            await Task.Delay((int) (CloseDuration * 1000 * count));
        }
    }
}