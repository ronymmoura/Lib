#region Usings
using Foundation;
using System;
using Xamarin.Forms;
using Lib.Mobile.DependencyServices;
using Lib.Mobile.iOS.DependencyServices;
#endregion

[assembly: Dependency(typeof(OperacoesNativas))]
namespace Lib.Mobile.iOS.DependencyServices
{
    public class OperacoesNativas : IOperacoesNativas
    {
        public Version Versao => Version.Parse(NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString());
    }
}
