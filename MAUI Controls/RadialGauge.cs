using Microsoft.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MAUI_Controls
{
    public class GaugeView : SKCanvasView
    {
        private const int AnimationDuration = 200;
        private const float SweepAngle = 270f;
        
        public static readonly BindableProperty BarColorProperty = 
            BindableProperty.Create(
                nameof(BarColor),
                typeof(Color),
                typeof(GaugeView),
                Colors.LightGray,
                propertyChanged: (bindable, oldValue, newValue) => 
                {
                    ((GaugeView)bindable).InvalidateSurface();
                });

        public static readonly BindableProperty ProgressBarColorProperty = 
            BindableProperty.Create(
                nameof(ProgressBarColor),
                typeof(Color),
                typeof(GaugeView),
                Colors.RoyalBlue,
                propertyChanged: (bindable, oldValue, newValue) => 
                {
                    ((GaugeView)bindable).InvalidateSurface();
                });

        public static readonly BindableProperty GaugeBackgroundColorProperty = 
            BindableProperty.Create(
                nameof(GaugeBackgroundColor),
                typeof(Color),
                typeof(GaugeView),
                Colors.White,
                propertyChanged: (bindable, oldValue, newValue) => 
                {
                    ((GaugeView)bindable).InvalidateSurface();
                });

        public Color BarColor
        {
            get => (Color)GetValue(BarColorProperty);
            set => SetValue(BarColorProperty, value);
        }

        public Color ProgressBarColor
        {
            get => (Color)GetValue(ProgressBarColorProperty);
            set => SetValue(ProgressBarColorProperty, value);
        }

        public Color GaugeBackgroundColor
        {
            get => (Color)GetValue(GaugeBackgroundColorProperty);
            set => SetValue(GaugeBackgroundColorProperty, value);
        }

// MaxValue Property
        public static readonly BindableProperty MaxValueProperty =
            BindableProperty.Create(
                nameof(MaxValue),
                typeof(float),
                typeof(GaugeView),
                100f, // Default value
                BindingMode.TwoWay);

        public float MaxValue
        {
            get => (float)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

// MinValue Property
        public static readonly BindableProperty MinValueProperty =
            BindableProperty.Create(
                nameof(MinValue),
                typeof(float),
                typeof(GaugeView),
                0f, // Default value
                BindingMode.TwoWay);

        public float MinValue
        {
            get => (float)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }


        private float _animatedValue = 0f;
        private CancellationTokenSource _cts;

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(float), typeof(GaugeView), 0f,
                propertyChanged: OnValueChanged);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(GaugeView), Colors.Black);

        public static readonly BindableProperty NeedleColorProperty =
            BindableProperty.Create(nameof(NeedleColor), typeof(Color), typeof(GaugeView), Colors.RoyalBlue);

        
        public static readonly BindableProperty NeedleTypeProperty =
            BindableProperty.Create(nameof(NeedleType), typeof(GaugeNeedleType), typeof(GaugeView), GaugeNeedleType.Line, BindingMode.TwoWay);
      
        public static readonly BindableProperty CenterDotSizeProperty =
            BindableProperty.Create(
                nameof(CenterDotSize),
                typeof(float),
                typeof(GaugeView),
                defaultValue: 5.0f, 
                defaultBindingMode: BindingMode.TwoWay);

        public float CenterDotSize
        {
            get => (float)GetValue(CenterDotSizeProperty);
            set => SetValue(CenterDotSizeProperty, value);
        }

        public static readonly BindableProperty DrawValueTextProperty =
            BindableProperty.Create(
                nameof(DrawValueText), 
                typeof(bool), 
                typeof(GaugeView), 
                defaultValue: true,
                defaultBindingMode: BindingMode.TwoWay);

        public bool DrawValueText
        {
            get => (bool)GetValue(DrawValueTextProperty);
            set => SetValue(DrawValueTextProperty, value);
        }
        public static readonly BindableProperty UnitTypeProperty =
            BindableProperty.Create(
                nameof(UnitType), 
                typeof(string), 
                typeof(GaugeView), 
                defaultValue: string.Empty, 
                defaultBindingMode: BindingMode.TwoWay);
        
        public static readonly BindableProperty GaugeTypeProperty =
            BindableProperty.Create(
                nameof(GaugeType),
                typeof(GaugeStrokeType),
                typeof(GaugeView),
                defaultValue: GaugeStrokeType.Flat,
                defaultBindingMode: BindingMode.TwoWay);

        public GaugeStrokeType GaugeType
        {
            get => (GaugeStrokeType)GetValue(GaugeTypeProperty);
            set => SetValue(GaugeTypeProperty, value);
        }


        public string UnitType
        {
            get => (string)GetValue(UnitTypeProperty);
            set => SetValue(UnitTypeProperty, value);
        }

        public static readonly BindableProperty TextPositionProperty =
            BindableProperty.Create(
                nameof(TextPosition), 
                typeof(ValueTextPosition), 
                typeof(GaugeView), 
                defaultValue: ValueTextPosition.Center, 
                defaultBindingMode: BindingMode.TwoWay);

        public ValueTextPosition TextPosition
        {
            get => (ValueTextPosition)GetValue(TextPositionProperty);
            set => SetValue(TextPositionProperty, value);
        }

        public GaugeNeedleType NeedleType
        {
            get => (GaugeNeedleType)GetValue(NeedleTypeProperty);
            set => SetValue(NeedleTypeProperty, value);
        }

        public float Value
        {
            get => (float)GetValue(ValueProperty);
            set => SetValue(ValueProperty, Math.Min(MaxValue, Math.Max(MinValue, value)));
        }
        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public Color NeedleColor
        {
            get => (Color)GetValue(NeedleColorProperty);
            set => SetValue(NeedleColorProperty, value);
        }

        public GaugeView()
        {
            PaintSurface += OnPaintSurface;
            WidthRequest = 150;
            HeightRequest = 150;
        }

        private static async void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is GaugeView gaugeView) 
                await gaugeView.AnimateNeedleAsync(Math.Min(gaugeView.MaxValue, Math.Max(gaugeView.MinValue, (float)newValue)));
        }

        private async Task AnimateNeedleAsync(float toValue)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            float fromValue = _animatedValue;
            float step = (toValue - fromValue) / (AnimationDuration / 16);

            try
            {
                for (int i = 0; i < AnimationDuration / 16; i++)
                {
                    if (token.IsCancellationRequested)
                        return;

                    _animatedValue += step;
                    InvalidateSurface();
                    await Task.Delay(16, token);
                }

                _animatedValue = toValue;
                InvalidateSurface();
            }
            catch
            {
                
            }
        }
        
        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();

            float width = e.Info.Width;
            float height = e.Info.Height;
            float size = Math.Min(width, height);

            canvas.Translate(width / 2, height / 2);
            canvas.Scale(size / 200f);

            DrawBackground(canvas);
            DrawGauge(canvas);
            DrawNeedle(canvas);
            DrawText(canvas);
        }

        private void DrawBackground(SKCanvas canvas)
        {
            using var paint = new SKPaint { Style = SKPaintStyle.Fill, Color = GaugeBackgroundColor.ToSKColor() };
            canvas.DrawCircle(0, 0, 100, paint);
        }

        private void DrawGauge(SKCanvas canvas)
        {
            if (GaugeType == GaugeStrokeType.Flat)
                DrawGaugeLine(canvas);
            else if(GaugeType == GaugeStrokeType.Rounded)
                DrawGaugeRounded(canvas);
        }
        private void DrawGaugeLine(SKCanvas canvas)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 13,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Butt 
            };

            
            paint.Color = BarColor.ToSKColor();
            var rect = new SKRect(-90, -90, 90, 90);
            canvas.DrawArc(rect, 135, SweepAngle, false, paint);

            
            paint.Color = ProgressBarColor.ToSKColor();
            float sweepAngleForValue = (_animatedValue / MaxValue) * SweepAngle;
            canvas.DrawArc(rect, 135, sweepAngleForValue, false, paint);
        }

        private void DrawGaugeRounded(SKCanvas canvas)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 13,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round 
            };

            
            paint.Color = BarColor.ToSKColor();
            var rect = new SKRect(-90, -90, 90, 90);
            canvas.DrawArc(rect, 135, SweepAngle, false, paint);

            
            paint.Color = ProgressBarColor.ToSKColor();
            float sweepAngleForValue = (_animatedValue / MaxValue) * SweepAngle;
            canvas.DrawArc(rect, 135, sweepAngleForValue, false, paint);
        }
        private void DrawNeedle(SKCanvas canvas)
        {
            if (NeedleType == GaugeNeedleType.Arrow)
                DrawNeedleArrow(canvas);
            else if (NeedleType == GaugeNeedleType.Line)
                DrawNeedleLine(canvas);
            else if (NeedleType == GaugeNeedleType.RoundedLine)
                DrawNeedleRounded(canvas);
            else if (NeedleType == GaugeNeedleType.None) ;
                // do nothing
        }

        private void DrawNeedleArrow(SKCanvas canvas)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = NeedleColor.ToSKColor(),
                StrokeWidth = 5,
                IsAntialias = true
            };

            float angleInRadians = (135 + (_animatedValue / MaxValue) * SweepAngle) * (float)Math.PI / 180f;

            float needleLength = 70f;

            var needleEndPointX = needleLength * (float)Math.Cos(angleInRadians);
            var needleEndPointY = needleLength * (float)Math.Sin(angleInRadians);

            float adjustedNeedleLength =
                needleLength - 10f;
            var adjustedNeedleEndPointX = adjustedNeedleLength * (float)Math.Cos(angleInRadians);
            var adjustedNeedleEndPointY = adjustedNeedleLength * (float)Math.Sin(angleInRadians);

            canvas.DrawLine(0, 0, adjustedNeedleEndPointX, adjustedNeedleEndPointY, paint);

            float arrowLength = 15f;
            float arrowWidth = 6f;

            var arrowTipX =
                (needleLength + 2f) * (float)Math.Cos(angleInRadians); 
            var arrowTipY = (needleLength + 2f) * (float)Math.Sin(angleInRadians);

            
            float leftAngle = angleInRadians + (float)Math.PI / 24; 
            float rightAngle = angleInRadians - (float)Math.PI / 24;

            var arrowLeftX = (needleLength - arrowLength) * (float)Math.Cos(leftAngle);
            var arrowLeftY = (needleLength - arrowLength) * (float)Math.Sin(leftAngle);

            var arrowRightX = (needleLength - arrowLength) * (float)Math.Cos(rightAngle);
            var arrowRightY = (needleLength - arrowLength) * (float)Math.Sin(rightAngle);

            using var arrowPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = NeedleColor.ToSKColor(),
                IsAntialias = true
            };

            var path = new SKPath();
            path.MoveTo(arrowTipX, arrowTipY);  
            path.LineTo(arrowLeftX, arrowLeftY); 
            path.LineTo(arrowRightX, arrowRightY);
            path.Close();

            canvas.DrawPath(path, arrowPaint);

            paint.Style = SKPaintStyle.Fill;
            canvas.DrawCircle(0, 0, CenterDotSize, paint);
        }


        private void DrawNeedleLine(SKCanvas canvas)
        {
            using var paint = new SKPaint
                { Style = SKPaintStyle.StrokeAndFill, Color = NeedleColor.ToSKColor(), StrokeWidth = 5 };

            float angleInRadians = (135 + (_animatedValue / MaxValue) * SweepAngle) * (float)Math.PI / 180f;

            float needleLength = 70f;

            var needleEndPointX = needleLength * (float)Math.Cos(angleInRadians);
            var needleEndPointY = needleLength * (float)Math.Sin(angleInRadians);

            canvas.DrawLine(0, 0, needleEndPointX, needleEndPointY, paint);

            paint.Style = SKPaintStyle.Fill;
            paint.Color = NeedleColor.ToSKColor();

            canvas.DrawCircle(0, 0, CenterDotSize, paint);
        }
        private void DrawNeedleRounded(SKCanvas canvas)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = NeedleColor.ToSKColor(),
                StrokeWidth = 5,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true
            };

            float angleInRadians = (135 + (_animatedValue / MaxValue) * SweepAngle) * (float)Math.PI / 180f;

            float needleLength = 70f;

            var needleEndPointX = needleLength * (float)Math.Cos(angleInRadians);
            var needleEndPointY = needleLength * (float)Math.Sin(angleInRadians);

            canvas.DrawLine(0, 0, needleEndPointX, needleEndPointY, paint);

            paint.Style = SKPaintStyle.Fill; 
            paint.Color = NeedleColor.ToSKColor();

            canvas.DrawCircle(0, 0, CenterDotSize, paint);
        }

        private void DrawText(SKCanvas canvas)
        {
            if (!DrawValueText) return;
            
            
            using var valuePaint = new SKPaint { TextSize = 25f, Color = TextColor.ToSKColor(), IsAntialias = true };
            string textToDraw =
                _animatedValue >= MaxValue ? $"{_animatedValue:F0}+" : $"{_animatedValue:F0}";


            if (TextPosition == ValueTextPosition.Bottom)
            {
                var textBounds = new SKRect();
                valuePaint.MeasureText(textToDraw, ref textBounds);

                canvas.DrawText(textToDraw,
                    -textBounds.MidX,
                    textBounds.Height + 30,
                    valuePaint);

                using var unitPaint = new SKPaint { TextSize = 11f, Color = TextColor.ToSKColor(), IsAntialias = true };
                var unitBounds = new SKRect();
                unitPaint.MeasureText(UnitType, ref unitBounds);

                canvas.DrawText(UnitType,
                    -unitBounds.MidX,
                    textBounds.Height + 48,
                    unitPaint);
            }
            else if (TextPosition == ValueTextPosition.Center)
            {
                var textBounds = new SKRect();
                valuePaint.MeasureText(textToDraw, ref textBounds);

                canvas.DrawText(
                    textToDraw,
                    -textBounds.MidX, 
                    textBounds.Height / 2, 
                    valuePaint);

                using var unitPaint = new SKPaint { TextSize = 11f, Color = TextColor.ToSKColor(), IsAntialias = true };
                var unitBounds = new SKRect();
                unitPaint.MeasureText(UnitType, ref unitBounds);

                canvas.DrawText(
                    UnitType,
                    -unitBounds.MidX,
                    (textBounds.Height / 2) + unitBounds.Height + 10, 
                    unitPaint);
            }

        }
    }

    public enum ValueTextPosition
    {
        Center,
        Bottom
    }
    public enum GaugeStrokeType
    {
        Flat = 0,
        Rounded = 1
    }

    public enum GaugeNeedleType
    {
        Line = 0,
        RoundedLine = 1,
        Arrow = 2,
        None = 3
    }    
}