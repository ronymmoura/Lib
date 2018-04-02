#region Usings
using Android.Views;
using Lib.Mobile.Droid.DependencyServices;
using Lib.Mobile.DependencyServices;
using Android.App;
using Xamarin.Forms;
#endregion

[assembly: Xamarin.Forms.Dependency(typeof(StatusBarImplementation))]
namespace Lib.Mobile.Droid.DependencyServices
{
    public class StatusBarImplementation : IStatusBar
    {
        WindowManagerFlags _originalFlags;

        void IStatusBar.Hide()
        {
            var activity = (Activity)Forms.Context;
            var attrs = activity.Window.Attributes;
            _originalFlags = attrs.Flags;
            attrs.Flags |= WindowManagerFlags.Fullscreen;
            activity.Window.Attributes = attrs;
        }

        public void Show()
        {
            var activity = (Activity)Forms.Context;
            var attrs = activity.Window.Attributes;
            attrs.Flags = _originalFlags;
            activity.Window.Attributes = attrs;
        }
    }
}
