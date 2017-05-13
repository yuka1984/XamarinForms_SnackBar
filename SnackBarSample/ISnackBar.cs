using System;
namespace SnackBarSample
{
    public interface ISnackBar
    {
        void Show(string text, int duration, string actionText, Action action);
    }
}
