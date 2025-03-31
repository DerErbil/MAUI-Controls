using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace MAUI_Controls
{
    public class AnalogClock : SKCanvasView
    {
        public static readonly BindableProperty ClockFaceColorProperty = BindableProperty.Create(nameof(ClockFaceColor), typeof(Color), typeof(AnalogClock), Colors.White, propertyChanged: OnColorPropertyChanged);
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(AnalogClock), Colors.RoyalBlue, propertyChanged: OnColorPropertyChanged);
        public static readonly BindableProperty HourMarkerColorProperty = BindableProperty.Create(nameof(HourMarkerColor), typeof(Color), typeof(AnalogClock), Color.FromHex("#303030"), propertyChanged: OnColorPropertyChanged);
        public static readonly BindableProperty HourHandColorProperty = BindableProperty.Create(nameof(HourHandColor), typeof(Color), typeof(AnalogClock), Colors.RoyalBlue, propertyChanged: OnColorPropertyChanged);
        public static readonly BindableProperty MinuteHandColorProperty = BindableProperty.Create(nameof(MinuteHandColor), typeof(Color), typeof(AnalogClock), Colors.RoyalBlue, propertyChanged: OnColorPropertyChanged);
        public static readonly BindableProperty SecondHandColorProperty = BindableProperty.Create(nameof(SecondHandColor), typeof(Color), typeof(AnalogClock), Color.FromHex("#303030"), propertyChanged: OnColorPropertyChanged);
        public static readonly BindableProperty UseNumberMarkersProperty = BindableProperty.Create(nameof(UseNumberMarkers), typeof(bool), typeof(AnalogClock), false, propertyChanged: (bindable, oldValue, newValue) => ((AnalogClock)bindable).InvalidateSurface());

        private static void OnColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var clock = (AnalogClock)bindable;
            clock.UpdateColors();
            clock.InvalidateSurface();
        }

        private void UpdateColors()
        {
            _clockFacePaint.Color = ClockFaceColor.ToSKColor();
            _borderPaint.Color = BorderColor.ToSKColor();
            _hourMarkerPaint.Color = HourMarkerColor.ToSKColor();
            _hourHandPaint.Color = HourHandColor.ToSKColor();
            _minuteHandPaint.Color = MinuteHandColor.ToSKColor();
            _secondHandPaint.Color = SecondHandColor.ToSKColor();
            _numberPaint.Color = HourMarkerColor.ToSKColor();
        }

        public Color ClockFaceColor
        {
            get => (Color)GetValue(ClockFaceColorProperty);
            set => SetValue(ClockFaceColorProperty, value);
        }

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public Color HourMarkerColor
        {
            get => (Color)GetValue(HourMarkerColorProperty);
            set => SetValue(HourMarkerColorProperty, value);
        }

        public Color HourHandColor
        {
            get => (Color)GetValue(HourHandColorProperty);
            set => SetValue(HourHandColorProperty, value);
        }

        public Color MinuteHandColor
        {
            get => (Color)GetValue(MinuteHandColorProperty);
            set => SetValue(MinuteHandColorProperty, value);
        }

        public Color SecondHandColor
        {
            get => (Color)GetValue(SecondHandColorProperty);
            set => SetValue(SecondHandColorProperty, value);
        }

        public bool UseNumberMarkers
        {
            get => (bool)GetValue(UseNumberMarkersProperty);
            set => SetValue(UseNumberMarkersProperty, value);
        }

        private readonly SKPaint _clockFacePaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.White,
            IsAntialias = true
        };

        private readonly SKPaint _borderPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.RoyalBlue,
            StrokeWidth = 2,
            IsAntialias = true
        };

        private readonly SKPaint _hourMarkerPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColor.Parse("#303030"),
            IsAntialias = true
        };

        private readonly SKPaint _hourHandPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.RoyalBlue,
            StrokeWidth = 8,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };

        private readonly SKPaint _minuteHandPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.RoyalBlue,
            StrokeWidth = 6,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };

        private readonly SKPaint _secondHandPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColor.Parse("#303030"),
            StrokeWidth = 2,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };

        private readonly SKPaint _numberPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColor.Parse("#303030"),
            TextAlign = SKTextAlign.Center,
            IsAntialias = true
        };

        private IDispatcherTimer _timer;

        public AnalogClock()
        {
            UpdateColors();

            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50);
            _timer.Tick += (s, e) => InvalidateSurface();
            _timer.Start();
            
            _lastHour = DateTime.Now.Hour;
            _lastMinute = DateTime.Now.Minute;
            _lastSecond = DateTime.Now.Second;
        }

        
        public event EventHandler MinutePassed;
        public event EventHandler SecondPassed;
        public event EventHandler HourPassed;
        
        private int _lastHour;
        private int _lastMinute;
        private int _lastSecond;
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            // Alle 50ms wird diese Methode aufgerufen, um die Zeiger sehr Smooth zu bewegen.

            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            var info = e.Info;
            var centerX = info.Width / 2f;
            var centerY = info.Height / 2f;
            var radius = Math.Min(centerX, centerY) - 20;

            float scaleFactor = Math.Min(info.Width, info.Height) / 200f;
            _numberPaint.TextSize = 20 * scaleFactor;

            // Clock Background
            canvas.DrawCircle(centerX, centerY, radius, _clockFacePaint);

            // Clock Border
            canvas.DrawCircle(centerX, centerY, radius, _borderPaint);

            // stunden marker
            if (UseNumberMarkers)
                DrawNumberMarkers(canvas, centerX, centerY, radius);
            else
                DrawLineMarkers(canvas, centerX, centerY, radius);

            
            // Aktuelle Zeit ziehen
            var currentTime = DateTime.Now;
            
            if(_lastHour != currentTime.Hour)
                HourPassed?.Invoke(this, EventArgs.Empty);
            
            if(_lastMinute != currentTime.Minute)
                MinutePassed?.Invoke(this, EventArgs.Empty);
            
            if(_lastSecond != currentTime.Second)
                SecondPassed?.Invoke(this, EventArgs.Empty);

            // Stunde
            double hourAngle = (currentTime.Hour % 12 + currentTime.Minute / 60.0) * 30;
            DrawHand(canvas, centerX, centerY, radius * 0.5f, hourAngle, _hourHandPaint);

            // Minute
            double minuteAngle = (currentTime.Minute + currentTime.Second / 60.0) * 6;
            DrawHand(canvas, centerX, centerY, radius * 0.7f, minuteAngle, _minuteHandPaint);

            // Sekunde
            double secondAngle = (currentTime.Second + currentTime.Millisecond / 1000.0) * 6;
            DrawHand(canvas, centerX, centerY, radius * 0.9f, secondAngle, _secondHandPaint);

            // Center dot
            canvas.DrawCircle(centerX, centerY, 5, _hourHandPaint);
        }

        private void DrawLineMarkers(SKCanvas canvas, float centerX, float centerY, float radius)
        {
            for (int i = 0; i < 12; i++)
            {
                var angle = i * 30 * Math.PI / 180;
                var markerLength = i % 3 == 0 ? 15f : 10f;
                var startX = centerX + (radius - markerLength) * Math.Sin(angle);
                var startY = centerY - (radius - markerLength) * Math.Cos(angle);
                var endX = centerX + radius * Math.Sin(angle);
                var endY = centerY - radius * Math.Cos(angle);
                canvas.DrawLine((float)startX, (float)startY, (float)endX, (float)endY, _hourMarkerPaint);
            }
        }

        private void DrawNumberMarkers(SKCanvas canvas, float centerX, float centerY, float radius)
        {
            float numberRadius = radius * 0.77f;
            for (int i = 1; i <= 12; i++)
            {
                var angle = (i * 30 - 90) * Math.PI / 180;
                var x = centerX + numberRadius * Math.Cos(angle);
                var y = centerY + numberRadius * Math.Sin(angle);
                
                canvas.DrawText(i.ToString(), (float)x, (float)y + (_numberPaint.TextSize / 3), _numberPaint);
            }
        }

        private void DrawHand(SKCanvas canvas, float centerX, float centerY, float length, double angleDegrees, SKPaint paint)
        {
            var angleRadians = angleDegrees * Math.PI / 180;
            var endX = centerX + length * Math.Sin(angleRadians);
            var endY = centerY - length * Math.Cos(angleRadians);
            canvas.DrawLine(centerX, centerY, (float)endX, (float)endY, paint);
        }
    }
}
