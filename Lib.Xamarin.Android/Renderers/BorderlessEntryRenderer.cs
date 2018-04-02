#region Usings
using Android.Content;
using Lib.Mobile.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android; 
#endregion

[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]
public class BorderlessEntryRenderer : EntryRenderer
{
    public BorderlessEntryRenderer(Context context) : base(context)
    {
    }

    protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
    {
        base.OnElementChanged(e);

        if (Control != null)
            Control.Background = null;
    }
}
