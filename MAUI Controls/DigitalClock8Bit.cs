using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace MAUI_Controls
{
    public class DigitalClock8Bit : SKCanvasView 
    {
        public static readonly BindableProperty DisplayColorProperty = 
            BindableProperty.Create(nameof(DisplayColor), typeof(Color), typeof(DigitalClock8Bit), Colors.Lime, propertyChanged: OnDisplayPropertyChanged);
        public static readonly BindableProperty BackgroundColorProperty = 
            BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(DigitalClock8Bit), Colors.Black, propertyChanged: OnDisplayPropertyChanged);

        private static readonly byte[,] DigitMatrix = new byte[10, 20]
        {
            {1,1,1,1, 1,0,0,1, 1,0,0,1, 1,0,0,1, 1,1,1,1}, // 0
            {0,0,0,1, 0,0,1,1, 0,0,0,1, 0,0,0,1, 0,0,0,1}, // 1
            {1,1,1,1, 0,0,0,1, 1,1,1,1, 1,0,0,0, 1,1,1,1}, // 2
            {1,1,1,1, 0,0,0,1, 1,1,1,1, 0,0,0,1, 1,1,1,1}, // 3
            {1,0,0,1, 1,0,0,1, 1,1,1,1, 0,0,0,1, 0,0,0,1}, // 4
            {1,1,1,1, 1,0,0,0, 1,1,1,1, 0,0,0,1, 1,1,1,1}, // 5
            {1,1,1,1, 1,0,0,0, 1,1,1,1, 1,0,0,1, 1,1,1,1}, // 6
            {1,1,1,1, 0,0,0,1, 0,0,1,0, 0,1,0,0, 1,0,0,0}, // 7
            {1,1,1,1, 1,0,0,1, 1,1,1,1, 1,0,0,1, 1,1,1,1}, // 8
            {1,1,1,1, 1,0,0,1, 1,1,1,1, 0,0,0,1, 1,1,1,1}  // 9
        };

        private DateTime _currentTime;
        private readonly SKPaint _paint;
        private IDispatcherTimer _timer;

        public Color DisplayColor
        {
            get => (Color)GetValue(DisplayColorProperty);
            set => SetValue(DisplayColorProperty, value);
        }

        public new Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public event EventHandler SecondPassed;
        public event EventHandler MinutePassed;
        public event EventHandler HourPassed;

        public DigitalClock8Bit()
        {
            _currentTime = DateTime.Now;
            _paint = new SKPaint { IsAntialias = false };

            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            var previousTime = _currentTime;
            _currentTime = DateTime.Now;

            if (_currentTime.Second != previousTime.Second)
                SecondPassed?.Invoke(this, EventArgs.Empty);
            if (_currentTime.Minute != previousTime.Minute)
                MinutePassed?.Invoke(this, EventArgs.Empty);
            if (_currentTime.Hour != previousTime.Hour)
                HourPassed?.Invoke(this, EventArgs.Empty);

            InvalidateSurface();
        }

        private static void OnDisplayPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as DigitalClock8Bit)?.InvalidateSurface();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var info = e.Info;

            canvas.Clear(BackgroundColor.ToSKColor());

            float width = info.Width;
            float height = info.Height;
            
            float pixelSize = Math.Min(width / 45, height / 4.5f);
            
            float totalWidth = pixelSize * 42; // 8 digits (4 pixels each) + 2 colons (2 pixels each) + spacing
            float startX = (width - totalWidth) / 2;
            float startY = (height - (pixelSize * 5)) / 2;

            _paint.Color = DisplayColor.ToSKColor();

            string time = _currentTime.ToString("HH:mm:ss");
            float currentX = startX;

            for (int i = 0; i < time.Length; i++)
            {
                if (time[i] == ':')
                {
                    DrawColon(canvas, currentX, startY + (pixelSize * 3), pixelSize);
                    currentX += pixelSize * 2;
                }
                else
                {
                    int number = int.Parse(time[i].ToString());
                    DrawDigit(canvas, number, currentX, startY, pixelSize);
                    currentX += pixelSize * 5;
                }
                currentX += pixelSize; // Space
            }
        }

        private void DrawDigit(SKCanvas canvas, int digit, float x, float y, float pixelSize)
        {
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (DigitMatrix[digit, row * 4 + col] == 1)
                    {
                        canvas.DrawRect(
                            x + (col * pixelSize),
                            y + (row * pixelSize),
                            pixelSize,
                            pixelSize,
                            _paint
                        );
                    }
                }
            }
        }

        private void DrawColon(SKCanvas canvas, float x, float y, float pixelSize)
        {
            canvas.DrawRect(x, y - pixelSize * 1.5f, pixelSize, pixelSize, _paint);
            canvas.DrawRect(x, y + pixelSize * 0.5f, pixelSize, pixelSize, _paint);
        }
    }
}
