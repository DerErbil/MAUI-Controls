using System.Linq;
using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace MAUI_Controls
{
    public class TintImage : Image
    {
        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
            nameof(TintColor), 
            typeof(Color), 
            typeof(TintImage), 
            null,
            propertyChanged: OnTintColorChanged);

        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        public TintImage()
        {
            Behaviors?.Clear();
            Behaviors?.Add(new IconTintColorBehavior());
        }

        static void OnTintColorChanged(BindableObject bindable, object oldVal, object newVal)
        {
            if (bindable is TintImage image && newVal is Color color)
            {
                var behavior = image.Behaviors.OfType<IconTintColorBehavior>().First();
                behavior.TintColor = color;
            }
        }
    }
}
