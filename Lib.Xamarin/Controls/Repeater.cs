#region Usings
using System.Collections;
using System.Collections.Specialized;
using Xamarin.Forms;
#endregion

namespace Lib.Mobile.Controls
{
    public class Repeater : WrapLayout
    {
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create("ItemTemplate", typeof(DataTemplate), typeof(Repeater), null);
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(Repeater), propertyChanging: ItemsSourceChanging);

        public Repeater() { }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        static void ItemsSourceChanging(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != null && oldValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)oldValue).CollectionChanged -= ((Repeater)bindable).OnCollectionChanged;
            }

            if (newValue != null && newValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)newValue).CollectionChanged += ((Repeater)bindable).OnCollectionChanged;
            }
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            Populate();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == ItemTemplateProperty.PropertyName || propertyName == ItemsSourceProperty.PropertyName)
            {
                Populate();
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            Populate();
        }

        public void Populate()
        {
            if (ItemsSource != null)
            {
                Children.Clear();

                foreach (var item in ItemsSource)
                {
                    var content = ItemTemplate.CreateContent();

                    if (content is ViewCell viewCell)
                    {
                        Children.Add(viewCell.View);
                        viewCell.BindingContext = item;
                    }
                }
            }
        }
    }
}
