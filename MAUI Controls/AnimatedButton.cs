using System;
using System.Threading;
using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using ViewExtensions = Microsoft.Maui.Controls.ViewExtensions;

namespace MAUI_Controls
{
    public enum ButtonMode
    {
        Normal,
        Loading,
        Success,
        Failed
    }

    public enum DotsAnimationType
    {
        Fade,
        Scale,
        Bounce
    }

    public class AnimatedButton : Border
    {
        #region Bindable Properties

        public static readonly BindableProperty ModeProperty = BindableProperty.Create(
            nameof(Mode),
            typeof(ButtonMode),
            typeof(AnimatedButton),
            ButtonMode.Normal,
            propertyChanged: OnModeChanged);

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(AnimatedButton),
            string.Empty);

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(AnimatedButton),
            Colors.White);

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            nameof(CornerRadius),
            typeof(Thickness),
            typeof(AnimatedButton),
            new Thickness(8),
            propertyChanged: OnCornerRadiusChanged);

        public static readonly BindableProperty IsEnabledProperty = BindableProperty.Create(
            nameof(IsEnabled),
            typeof(bool),
            typeof(AnimatedButton),
            true,
            propertyChanged: OnIsEnabledChanged);

        public static readonly BindableProperty EnabledBackgroundColorProperty = BindableProperty.Create(
            nameof(EnabledBackgroundColor),
            typeof(Color),
            typeof(AnimatedButton),
            Colors.DodgerBlue,
            propertyChanged: OnBackgroundColorChanged);

        public static readonly BindableProperty DisabledBackgroundColorProperty = BindableProperty.Create(
            nameof(DisabledBackgroundColor),
            typeof(Color),
            typeof(AnimatedButton),
            Colors.Gray,
            propertyChanged: OnBackgroundColorChanged);

        public static readonly BindableProperty DotsAnimationTypeProperty = BindableProperty.Create(
            nameof(DotsAnimationType),
            typeof(DotsAnimationType),
            typeof(AnimatedButton),
            DotsAnimationType.Fade,
            propertyChanged: OnDotsAnimationTypeChanged);

        #endregion

        #region Properties

        public ButtonMode Mode
        {
            get => (ButtonMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public Thickness CornerRadius
        {
            get => (Thickness)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public Color EnabledBackgroundColor
        {
            get => (Color)GetValue(EnabledBackgroundColorProperty);
            set => SetValue(EnabledBackgroundColorProperty, value);
        }

        public Color DisabledBackgroundColor
        {
            get => (Color)GetValue(DisabledBackgroundColorProperty);
            set => SetValue(DisabledBackgroundColorProperty, value);
        }

        public DotsAnimationType DotsAnimationType
        {
            get => (DotsAnimationType)GetValue(DotsAnimationTypeProperty);
            set => SetValue(DotsAnimationTypeProperty, value);
        }

        #endregion

        private Grid _contentGrid;
        private Label _textLabel;
        private DotsLoadingView _dotsLoadingView;
        private CheckmarkView _checkmarkView;
        private CrossView _crossView;
        private readonly TapGestureRecognizer _tapGestureRecognizer;

        public event EventHandler Clicked;

        public AnimatedButton()
        {
            _tapGestureRecognizer = new TapGestureRecognizer();
            _tapGestureRecognizer.Tapped += OnButtonTapped;
            GestureRecognizers.Add(_tapGestureRecognizer);

            InitializeComponents();
            UpdateCornerRadius(CornerRadius);
        }

        private static void OnDotsAnimationTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AnimatedButton button && button._dotsLoadingView != null)
            {
                button._dotsLoadingView.AnimationType = (DotsAnimationType)newValue;
            }
        }

        private void InitializeComponents()
        {
            _contentGrid = new Grid
            {
                BackgroundColor = EnabledBackgroundColor,
                Padding = new Thickness(10),
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Star }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            _textLabel = new Label
            {
                Text = Text,
                TextColor = TextColor,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            _dotsLoadingView = new DotsLoadingView
            {
                DotsColor = TextColor,
                AnimationType = DotsAnimationType,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = false
            };

            _checkmarkView = new CheckmarkView
            {
                StrokeColor = TextColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = false
            };

            _crossView = new CrossView
            {
                StrokeColor = TextColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = false
            };

            _contentGrid.Add(_textLabel);
            _contentGrid.Add(_dotsLoadingView);
            _contentGrid.Add(_checkmarkView);
            _contentGrid.Add(_crossView);
            Content = _contentGrid;

            UpdateVisualState();
        }

        private static void OnModeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AnimatedButton button)
            {
                button.UpdateVisualState();
            }
        }

        private static void OnCornerRadiusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AnimatedButton button && newValue is Thickness radius)
            {
                button.UpdateCornerRadius(radius);
            }
        }

        private void UpdateCornerRadius(Thickness radius)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.StrokeShape = new RoundRectangle
                    { CornerRadius = new CornerRadius(radius.Left, radius.Top, radius.Right, radius.Bottom) };
            });
        }

        private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AnimatedButton button)
            {
                button.UpdateVisualState();
            }
        }

        private static void OnBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AnimatedButton button)
            {
                button.UpdateVisualState();
            }
        }

        private void UpdateVisualState()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                uint animation = 150;
                var color = IsEnabled ? EnabledBackgroundColor : DisabledBackgroundColor;
                _contentGrid.BackgroundColorTo(color, length: animation);
                
                switch (Mode)
                {
                    case ButtonMode.Normal:
                        _dotsLoadingView.StopAnimation();
                        _dotsLoadingView.IsVisible = false;
                        _checkmarkView.IsVisible = false;
                        _crossView.IsVisible = false;
                        _textLabel.IsVisible = true;
                        break;
                    case ButtonMode.Loading:
                        _textLabel.IsVisible = false;
                        _checkmarkView.IsVisible = false;
                        _crossView.IsVisible = false;
                        _dotsLoadingView.IsVisible = true;
                        _dotsLoadingView.StartAnimation();
                        break;
                    case ButtonMode.Success:
                        _textLabel.IsVisible = false;
                        _dotsLoadingView.StopAnimation();
                        _dotsLoadingView.IsVisible = false;
                        _crossView.IsVisible = false;
                        _checkmarkView.IsVisible = true;
                        _checkmarkView.StartAnimation();
                        break;
                    case ButtonMode.Failed:
                        _textLabel.IsVisible = false;
                        _dotsLoadingView.StopAnimation();
                        _dotsLoadingView.IsVisible = false;
                        _checkmarkView.IsVisible = false;
                        _crossView.IsVisible = true;
                        _crossView.StartAnimation();
                        break;
                }
            });
        }

        private void OnButtonTapped(object sender, EventArgs e)
        {
            if (IsEnabled && Mode == ButtonMode.Normal)
            {
                Clicked?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Text) && _textLabel != null)
            {
                _textLabel.Text = Text;
            }
            else if (propertyName == nameof(TextColor) && _textLabel != null)
            {
                _textLabel.TextColor = TextColor;
                _dotsLoadingView.DotsColor = TextColor;
                _checkmarkView.StrokeColor = TextColor;
                _crossView.StrokeColor = TextColor;
            }
        }

        // DotsLoadingView implementation
        public class DotsLoadingView : Grid
        {
            private readonly Frame _dot1;
            private readonly Frame _dot2;
            private readonly Frame _dot3;
            private bool _isAnimating;
            private CancellationTokenSource _animationCancellation;

            public static readonly BindableProperty DotsColorProperty = BindableProperty.Create(
                nameof(DotsColor),
                typeof(Color),
                typeof(DotsLoadingView),
                Colors.White,
                propertyChanged: OnDotsColorChanged);

            public static readonly BindableProperty AnimationTypeProperty = BindableProperty.Create(
                nameof(AnimationType),
                typeof(DotsAnimationType),
                typeof(DotsLoadingView),
                DotsAnimationType.Fade);

            public Color DotsColor
            {
                get => (Color)GetValue(DotsColorProperty);
                set => SetValue(DotsColorProperty, value);
            }

            public DotsAnimationType AnimationType
            {
                get => (DotsAnimationType)GetValue(AnimationTypeProperty);
                set => SetValue(AnimationTypeProperty, value);
            }

            private static void OnDotsColorChanged(BindableObject bindable, object oldValue, object newValue)
            {
                if (bindable is DotsLoadingView view && newValue is Color color)
                {
                    view._dot1.BackgroundColor = color;
                    view._dot2.BackgroundColor = color;
                    view._dot3.BackgroundColor = color;
                }
            }

            public DotsLoadingView()
            {
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                _dot1 = CreateDot();
                _dot2 = CreateDot();
                _dot3 = CreateDot();

                Grid.SetColumn(_dot1, 0);
                Grid.SetColumn(_dot2, 1);
                Grid.SetColumn(_dot3, 2);
                Add(_dot1);
                Add(_dot2);
                Add(_dot3);

                HorizontalOptions = LayoutOptions.Center;
                VerticalOptions = LayoutOptions.Center;
                ColumnSpacing = 10;
            }

            private Frame CreateDot()
            {
                return new Frame
                {
                    CornerRadius = 4,
                    WidthRequest = 8,
                    HeightRequest = 8,
                    Padding = 0,
                    BackgroundColor = DotsColor,
                    HasShadow = false,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
            }

            public void StartAnimation()
            {
                if (_isAnimating)
                    return;

                _isAnimating = true;
                _animationCancellation = new CancellationTokenSource();
                
                switch (AnimationType)
                {
                    case DotsAnimationType.Fade:
                        StartFadeAnimation(_animationCancellation.Token);
                        break;
                    case DotsAnimationType.Scale:
                        StartScaleAnimation(_animationCancellation.Token);
                        break;
                    case DotsAnimationType.Bounce:
                        StartBounceAnimation(_animationCancellation.Token);
                        break;
                }
            }

            public void StopAnimation()
            {
                if (!_isAnimating)
                    return;

                _animationCancellation?.Cancel();
                _animationCancellation = null;
                _isAnimating = false;

                // Zurücksetzen der Dots auf ihren Ausgangszustand
                _dot1.Opacity = 1;
                _dot2.Opacity = 1;
                _dot3.Opacity = 1;
                _dot1.Scale = 1;
                _dot2.Scale = 1;
                _dot3.Scale = 1;
                _dot1.TranslationY = 0;
                _dot2.TranslationY = 0;
                _dot3.TranslationY = 0;
            }

            private async void StartFadeAnimation(CancellationToken cancellationToken)
            {
                const uint duration = 200;
                await Task.Delay(TimeSpan.FromMilliseconds(duration), cancellationToken);
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Dot 1 fade out/in
                    await _dot1.FadeTo(0.3, duration, Easing.SinInOut);
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot1.FadeTo(1, duration, Easing.SinInOut);
                    
                    // Dot 2 fade out/in
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot2.FadeTo(0.3, duration, Easing.SinInOut);
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot2.FadeTo(1, duration, Easing.SinInOut);
                    
                    // Dot 3 fade out/in
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot3.FadeTo(0.3, duration, Easing.SinInOut);
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot3.FadeTo(1, duration, Easing.SinInOut);
                }
            }

            private async void StartScaleAnimation(CancellationToken cancellationToken)
            {
                const uint duration = 200;
                await Task.Delay(TimeSpan.FromMilliseconds(duration), cancellationToken);
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Dot 1 scale up/down
                    await _dot1.ScaleTo(1.5, duration, Easing.CubicOut);
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot1.ScaleTo(1, duration, Easing.CubicIn);
                    
                    // Dot 2 scale up/down
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot2.ScaleTo(1.5, duration, Easing.CubicOut);
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot2.ScaleTo(1, duration, Easing.CubicIn);
                    
                    // Dot 3 scale up/down
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot3.ScaleTo(1.5, duration, Easing.CubicOut);
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot3.ScaleTo(1, duration, Easing.CubicIn);
                }
            }

            private async void StartBounceAnimation(CancellationToken cancellationToken)
            {
                const uint duration = 200;
                const double bounceHeight = -8;
                await Task.Delay(TimeSpan.FromMilliseconds(duration), cancellationToken);
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Dot 1 bounce
                    await _dot1.TranslateTo(0, bounceHeight, duration, Easing.CubicOut);
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot1.TranslateTo(0, 0, duration, Easing.BounceOut);
                    
                    // Dot 2 bounce
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot2.TranslateTo(0, bounceHeight, duration, Easing.CubicOut);
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot2.TranslateTo(0, 0, duration, Easing.BounceOut);
                    
                    // Dot 3 bounce
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot3.TranslateTo(0, bounceHeight, duration, Easing.CubicOut);
                    if (cancellationToken.IsCancellationRequested) break;
                    await _dot3.TranslateTo(0, 0, duration, Easing.BounceOut);
                }
            }
        }

        // Checkmark View implementation
        public class CheckmarkView : GraphicsView
        {
            private readonly CheckmarkDrawable _drawable;

            public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
                nameof(StrokeColor),
                typeof(Color),
                typeof(CheckmarkView),
                Colors.White,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    if (bindable is CheckmarkView view && newValue is Color color)
                    {
                        view._drawable.StrokeColor = color;
                        view.Invalidate();
                    }
                });

            public Color StrokeColor
            {
                get => (Color)GetValue(StrokeColorProperty);
                set => SetValue(StrokeColorProperty, value);
            }

            public CheckmarkView()
            {
                _drawable = new CheckmarkDrawable();
                Drawable = _drawable;
                HeightRequest = 25;
                WidthRequest = 25;
            }

            public void StartAnimation()
            {
                Animate();
            }

            private async void Animate()
            {
                // Reset progress
                _drawable.Progress = 0;

                // Animate from 0 to 1 over 500ms
                var animation = new Animation(v =>
                {
                    _drawable.Progress = (float)v;
                    Invalidate();
                }, 0, 1);

                animation.Commit(this, "CheckmarkAnimation", 16, 650, Easing.CubicOut);

                // Wait for animation to complete
                await Task.Delay(655);
            }

            private class CheckmarkDrawable : IDrawable
            {
                public Color StrokeColor { get; set; } = Colors.White;
                public float Progress { get; set; } = 0;

                public void Draw(ICanvas canvas, RectF dirtyRect)
                {
                    canvas.StrokeColor = StrokeColor;
                    canvas.StrokeSize = 3;
                    canvas.StrokeLineCap = LineCap.Round;

                    // Calculate checkmark points
                    float width = dirtyRect.Width;
                    float height = dirtyRect.Height;

                    // First point of checkmark
                    PointF point1 = new PointF(width * 0.2f, height * 0.5f);

                    // Second point (bottom of checkmark)
                    PointF point2 = new PointF(width * 0.45f, height * 0.7f);

                    // Third point (top right of checkmark)
                    PointF point3 = new PointF(width * 0.8f, height * 0.3f);

                    if (Progress <= 0.5f)
                    {
                        // Draw first half of checkmark (normalized progress 0-1 for first segment)
                        float normalizedProgress = Progress * 2;

                        PointF currentPoint = new PointF(
                            point1.X + (point2.X - point1.X) * normalizedProgress,
                            point1.Y + (point2.Y - point1.Y) * normalizedProgress
                        );

                        canvas.DrawLine(point1, currentPoint);
                    }
                    else
                    {
                        // Draw complete first segment
                        canvas.DrawLine(point1, point2);

                        // Draw second half of checkmark (normalized progress 0-1 for second segment)
                        float normalizedProgress = (Progress - 0.5f) * 2;

                        PointF currentPoint = new PointF(
                            point2.X + (point3.X - point2.X) * normalizedProgress,
                            point2.Y + (point3.Y - point2.Y) * normalizedProgress
                        );

                        canvas.DrawLine(point2, currentPoint);
                    }
                }
            }
        }

        // Cross View implementation
        public class CrossView : GraphicsView
        {
            private readonly CrossDrawable _drawable;

            public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
                nameof(StrokeColor),
                typeof(Color),
                typeof(CrossView),
                Colors.White,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    if (bindable is CrossView view && newValue is Color color)
                    {
                        view._drawable.StrokeColor = color;
                        view.Invalidate();
                    }
                });

            public Color StrokeColor
            {
                get => (Color)GetValue(StrokeColorProperty);
                set => SetValue(StrokeColorProperty, value);
            }

            public CrossView()
            {
                _drawable = new CrossDrawable();
                Drawable = _drawable;
                HeightRequest = 25;
                WidthRequest = 25;
            }

            public void StartAnimation()
            {
                Animate();
            }

            private async void Animate()
            {
                // Reset progress
                _drawable.Progress = 0;

                // Animate from 0 to 1 over 500ms
                var animation = new Animation(v =>
                {
                    _drawable.Progress = (float)v;
                    Invalidate();
                }, 0, 1);

                animation.Commit(this, "CrossAnimation", 16, 650, Easing.CubicOut);

                // Wait for animation to complete
                await Task.Delay(655);
            }

            private class CrossDrawable : IDrawable
            {
                public Color StrokeColor { get; set; } = Colors.White;
                public float Progress { get; set; } = 0;

                public void Draw(ICanvas canvas, RectF dirtyRect)
                {
                    canvas.StrokeColor = StrokeColor;
                    canvas.StrokeSize = 3;
                    canvas.StrokeLineCap = LineCap.Round;

                    float width = dirtyRect.Width;
                    float height = dirtyRect.Height;

                    // First line of the cross (top-left to bottom-right)
                    PointF point1 = new PointF(width * 0.2f, height * 0.2f);
                    PointF point2 = new PointF(width * 0.8f, height * 0.8f);

                    // Second line of the cross (top-right to bottom-left)
                    PointF point3 = new PointF(width * 0.8f, height * 0.2f);
                    PointF point4 = new PointF(width * 0.2f, height * 0.8f);

                    if (Progress <= 0.5f)
                    {
                        // Draw first line of the cross
                        float normalizedProgress = Progress * 2;

                        PointF currentPoint = new PointF(
                            point1.X + (point2.X - point1.X) * normalizedProgress,
                            point1.Y + (point2.Y - point1.Y) * normalizedProgress
                        );

                        canvas.DrawLine(point1, currentPoint);
                    }
                    else
                    {
                        // Draw complete first line
                        canvas.DrawLine(point1, point2);

                        // Draw second line of the cross
                        float normalizedProgress = (Progress - 0.5f) * 2;

                        PointF currentPoint = new PointF(
                            point3.X + (point4.X - point3.X) * normalizedProgress,
                            point3.Y + (point4.Y - point3.Y) * normalizedProgress
                        );

                        canvas.DrawLine(point3, currentPoint);
                    }
                }
            }
        }
    }
}
