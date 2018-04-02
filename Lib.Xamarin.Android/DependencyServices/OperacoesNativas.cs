#region Usings
using Android.App;
using Lib.Mobile.Droid.DependencyServices;
using Lib.Mobile.DependencyServices;
using System;
#endregion

[assembly: Xamarin.Forms.Dependency(typeof(OperacoesNativas))]
namespace Lib.Mobile.Droid.DependencyServices
{
    public class OperacoesNativas : IOperacoesNativas
    {
        public Version Versao
        {
            get
            {
                var context = Application.Context.ApplicationContext;
                var version = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
                return Version.Parse(version);
            }
        }
    }
}
