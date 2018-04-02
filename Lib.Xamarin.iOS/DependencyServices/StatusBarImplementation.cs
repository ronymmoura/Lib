#region Usings
using Lib.Mobile.DependencyServices;
using Lib.Xamarin.iOS.DependencyServices;
using UIKit;
#endregion

[assembly: Xamarin.Forms.Dependency(typeof(StatusBarImplementation))]
namespace Lib.Xamarin.iOS.DependencyServices
{
    public class StatusBarImplementation : IStatusBar
    {
        public void Hide()
        {
            UIApplication.SharedApplication.StatusBarHidden = true;
        }

        public void Show()
        {
            UIApplication.SharedApplication.StatusBarHidden = false;
        }
    }
}
