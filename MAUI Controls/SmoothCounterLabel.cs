using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace MAUI_Controls;

public class SmoothCounterLabel : Grid
{
    private readonly List<Label> _digitLabels = new();
    private readonly Random _random = new Random();
    private decimal _currentValue;
    private int _animationCount = 0;

    #region Bindable Properties

    // --- Value & Formatting ---
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value), typeof(decimal), typeof(SmoothCounterLabel), 0.00m,
        propertyChanged: OnValueChanged);

    public decimal Value
    {
        get => (decimal)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
        nameof(Format), typeof(string), typeof(SmoothCounterLabel), "0.00",
        propertyChanged: OnFormatChanged);

    public string Format
    {
        get => (string)GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    // --- Appearance ---
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(SmoothCounterLabel), Colors.Black,
        propertyChanged: OnTextColorChanged);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize), typeof(double), typeof(SmoothCounterLabel), 17.0,
        propertyChanged: OnFontPropertyChanged);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily), typeof(string), typeof(SmoothCounterLabel), "OpenSans-Semibold",
        propertyChanged: OnFontPropertyChanged); // Corrected callback name

    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    // --- Layout & Sizing Configuration ---
    public static readonly BindableProperty ColumnSpacingValueProperty = BindableProperty.Create(
        nameof(ColumnSpacingValue), typeof(double), typeof(SmoothCounterLabel), 0.0, // Default to 0, adjust as needed
        propertyChanged: OnLayoutPropertyChanged);

    public double ColumnSpacingValue
    {
        get => (double)GetValue(ColumnSpacingValueProperty);
        set => SetValue(ColumnSpacingValueProperty, value);
    }

    public static readonly BindableProperty DigitHeightMultiplierProperty = BindableProperty.Create(
        nameof(DigitHeightMultiplier), typeof(double), typeof(SmoothCounterLabel), 1.8, // Centralized multiplier
        propertyChanged: OnLayoutPropertyChanged);

    public double DigitHeightMultiplier
    {
        get => (double)GetValue(DigitHeightMultiplierProperty);
        set => SetValue(DigitHeightMultiplierProperty, value);
    }

    public static readonly BindableProperty DigitWidthMultiplierProperty = BindableProperty.Create(
        nameof(DigitWidthMultiplier), typeof(double), typeof(SmoothCounterLabel), 0.7, // Centralized multiplier
        propertyChanged: OnLayoutPropertyChanged);

    public double DigitWidthMultiplier
    {
        get => (double)GetValue(DigitWidthMultiplierProperty);
        set => SetValue(DigitWidthMultiplierProperty, value);
    }

    public static readonly BindableProperty CommaWidthMultiplierProperty = BindableProperty.Create(
        nameof(CommaWidthMultiplier), typeof(double), typeof(SmoothCounterLabel), 0.4, // Centralized multiplier
        propertyChanged: OnLayoutPropertyChanged);

    public double CommaWidthMultiplier
    {
        get => (double)GetValue(CommaWidthMultiplierProperty);
        set => SetValue(CommaWidthMultiplierProperty, value);
    }


    // --- Animation Configuration ---
    public static readonly BindableProperty AnimationDurationProperty = BindableProperty.Create(
        nameof(AnimationDuration), typeof(uint), typeof(SmoothCounterLabel), (uint)300);

    public uint AnimationDuration
    {
        get => (uint)GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    public static readonly BindableProperty AnimationTranslationYProperty = BindableProperty.Create(
        nameof(AnimationTranslationY), typeof(double), typeof(SmoothCounterLabel), 20.0); // Centralized animation value

    public double AnimationTranslationY
    {
        get => (double)GetValue(AnimationTranslationYProperty);
        set => SetValue(AnimationTranslationYProperty, value);
    }

     public static readonly BindableProperty AnimationRotationXFactorProperty = BindableProperty.Create(
        nameof(AnimationRotationXFactor), typeof(double), typeof(SmoothCounterLabel), -2.0); // Centralized animation value

    public double AnimationRotationXFactor
    {
        get => (double)GetValue(AnimationRotationXFactorProperty);
        set => SetValue(AnimationRotationXFactorProperty, value);
    }

    public static readonly BindableProperty AnimationScaleDivisorProperty = BindableProperty.Create(
        nameof(AnimationScaleDivisor), typeof(double), typeof(SmoothCounterLabel), 100.0); // Centralized animation value

    public double AnimationScaleDivisor
    {
        get => (double)GetValue(AnimationScaleDivisorProperty);
        set => SetValue(AnimationScaleDivisorProperty, value);
    }

     public static readonly BindableProperty AnimationKeyFrame1Property = BindableProperty.Create(
        nameof(AnimationKeyFrame1), typeof(double), typeof(SmoothCounterLabel), 0.3); // Centralized animation timing

    public double AnimationKeyFrame1
    {
        get => (double)GetValue(AnimationKeyFrame1Property);
        set => SetValue(AnimationKeyFrame1Property, value);
    }

    public static readonly BindableProperty AnimationKeyFrame2Property = BindableProperty.Create(
        nameof(AnimationKeyFrame2), typeof(double), typeof(SmoothCounterLabel), 0.4); // Centralized animation timing

    public double AnimationKeyFrame2
    {
        get => (double)GetValue(AnimationKeyFrame2Property);
        set => SetValue(AnimationKeyFrame2Property, value);
    }

    public static readonly BindableProperty EnableRandomDurationProperty = BindableProperty.Create(
        nameof(EnableRandomDuration), typeof(bool), typeof(SmoothCounterLabel), true); // Option to disable random factor

    public bool EnableRandomDuration
    {
        get => (bool)GetValue(EnableRandomDurationProperty);
        set => SetValue(EnableRandomDurationProperty, value);
    }

    public static readonly BindableProperty MinRandomDurationFactorProperty = BindableProperty.Create(
        nameof(MinRandomDurationFactor), typeof(double), typeof(SmoothCounterLabel), 0.7); // Min factor for random duration

    public double MinRandomDurationFactor
    {
        get => (double)GetValue(MinRandomDurationFactorProperty);
        set => SetValue(MinRandomDurationFactorProperty, value);
    }

     public static readonly BindableProperty MaxRandomDurationFactorProperty = BindableProperty.Create(
        nameof(MaxRandomDurationFactor), typeof(double), typeof(SmoothCounterLabel), 1.3); // Max factor for random duration

    public double MaxRandomDurationFactor
    {
        get => (double)GetValue(MaxRandomDurationFactorProperty);
        set => SetValue(MaxRandomDurationFactorProperty, value);
    }


    #endregion

    public SmoothCounterLabel()
    {
        // Use the BindableProperty for ColumnSpacing
        this.SetBinding(Grid.ColumnSpacingProperty, new Binding(nameof(ColumnSpacingValue), source: this));
        HorizontalOptions = LayoutOptions.Center;
        VerticalOptions = LayoutOptions.Center; // Added for completeness
        // ColumnDefinitions are handled dynamically in SetupDigitLabels
    }

    #region Property Changed Handlers

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SmoothCounterLabel)bindable;
        control.CancelAllAnimations();
        control.AnimateValueChange((decimal)oldValue, (decimal)newValue);
    }

    private static void OnFormatChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SmoothCounterLabel)bindable;
        control.UpdateDisplay(control._currentValue); // Re-render with new format
    }

    private static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SmoothCounterLabel)bindable;
        var newColor = (Color)newValue;
        foreach (var label in control._digitLabels)
        {
            label.TextColor = newColor;
        }
    }

    // Combined handler for Font Size and Family changes
    private static void OnFontPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SmoothCounterLabel)bindable;
        control.UpdateFont();
        control.UpdateContainerSizes(); // Font changes affect size
    }

    // Handler for layout-related property changes
    private static void OnLayoutPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SmoothCounterLabel)bindable;
        // ColumnSpacing is handled by binding
        control.UpdateContainerSizes(); // Update sizes if multipliers change
    }

    #endregion

    #region Update Methods

    private void UpdateFont()
    {
        foreach (var label in _digitLabels)
        {
            label.FontSize = FontSize;
            label.FontFamily = FontFamily;
        }
        // Container sizes depend on font size, so update them too.
        // This is now called from OnFontPropertyChanged after UpdateFont.
    }

    private void UpdateContainerSizes()
    {
        string valueText = _currentValue.ToString(Format); // Use current value and format

        for (int i = 0; i < Children.Count; i++)
        {
            if (Children[i] is Grid digitContainer && i < _digitLabels.Count)
            {
                bool isComma = i < valueText.Length && (valueText[i] == ',' || valueText[i] == '.'); // Handle both separators
                double containerHeight = FontSize * DigitHeightMultiplier;
                double containerWidth = isComma ? FontSize * CommaWidthMultiplier : FontSize * DigitWidthMultiplier;

                // Prevent zero or negative sizes which can cause layout issues
                digitContainer.HeightRequest = Math.Max(1, containerHeight);
                digitContainer.WidthRequest = Math.Max(1, containerWidth);
            }
        }
         // Force layout update
        InvalidateMeasure();
    }

    // Clears and rebuilds the grid structure if needed
    private void ResetGrid(int requiredLength)
    {
        if (_digitLabels.Count != requiredLength)
        {
             // Clear existing children and labels
            foreach (var child in Children.ToList())
            {
                Remove(child);
            }
            _digitLabels.Clear();

            // Clear existing column definitions explicitly
            ColumnDefinitions.Clear();

            // Add new column definitions
            for (int i = 0; i < requiredLength; i++)
            {
                ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            // Setup labels (will add children back)
            SetupDigitLabels(requiredLength);
        }
    }


    private void UpdateDisplay(decimal value)
    {
        _currentValue = value; // Keep track of the current value
        string text = value.ToString(Format);

        ResetGrid(text.Length); // Ensure grid structure matches text length

        for (int i = 0; i < text.Length; i++)
        {
            if (i < _digitLabels.Count)
            {
                _digitLabels[i].Text = text[i].ToString();
            }
        }
        UpdateContainerSizes(); // Ensure sizes are correct after potential reset
    }

    #endregion

    #region Animation Logic

    private void AnimateValueChange(decimal oldValue, decimal newValue)
    {
        string oldText = oldValue.ToString(Format);
        string newText = newValue.ToString(Format);

        // Ensure Grid structure matches the NEW text length before animating
        ResetGrid(newText.Length);

        _animationCount = 0;
        bool isIncreasing = newValue > oldValue;

        for (int i = 0; i < newText.Length; i++)
        {
            char oldChar = i < oldText.Length ? oldText[i] : ' '; // Use space if old text was shorter
            char newChar = newText[i];

            // Ensure the label exists (it should after ResetGrid)
            if (i >= _digitLabels.Count) continue;

             var label = _digitLabels[i];

            // If the character is not changing, just set the text directly
            if (oldChar == newChar)
            {
                 label.Text = newChar.ToString();
                 // Reset any potential leftover animation artifacts
                 label.TranslationY = 0;
                 label.Opacity = 1;
                 label.RotationX = 0;
                 label.Scale = 1;
                continue;
            }

             // Check if it's a digit or separator that needs the standard animation
            bool isNumericOrSeparator = char.IsDigit(oldChar) || oldChar == ',' || oldChar == '.' || oldChar == ' ' ||
                                       char.IsDigit(newChar) || newChar == ',' || newChar == '.';

            if(isNumericOrSeparator)
            {
                 uint positionDuration = AnimationDuration;
                 if (EnableRandomDuration)
                 {
                     double factor = MinRandomDurationFactor + (_random.NextDouble() * (MaxRandomDurationFactor - MinRandomDurationFactor));
                     positionDuration = (uint)(AnimationDuration * factor);
                 }
                 AnimateDigit(i, oldChar, newChar, positionDuration, isIncreasing);
                 _animationCount++;
            }
            else // Handle non-numeric/non-separator transitions differently if needed (e.g., simple fade)
            {
                 // Fallback: Just set the text directly if it's an unexpected character change
                 label.Text = newChar.ToString();
            }
        }

        _currentValue = newValue;
        UpdateContainerSizes(); // Update sizes after potential text changes
    }

     private void SetupDigitLabels(int count)
    {
         // This method now assumes ColumnDefinitions are already set by ResetGrid
        string valueText = _currentValue.ToString(Format); // Use current value for initial setup hint

        while (_digitLabels.Count < count)
        {
            int position = _digitLabels.Count;
            bool isComma = position < valueText.Length && (valueText[position] == ',' || valueText[position] == '.');

            var digitContainer = new Grid
            {
                // Use bindable properties for sizing
                HeightRequest = Math.Max(1, FontSize * DigitHeightMultiplier),
                WidthRequest = Math.Max(1, isComma ? FontSize * CommaWidthMultiplier : FontSize * DigitWidthMultiplier),
                IsClippedToBounds = true,
                VerticalOptions = LayoutOptions.Center, // Center the container vertically
                HorizontalOptions = LayoutOptions.Center // Center the container horizontally
            };

            var label = new Label
            {
                Text = position < valueText.Length ? valueText[position].ToString() : "0", // Initial text
                FontSize = FontSize,
                FontFamily = FontFamily,
                TextColor = TextColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center, // Ensure text is centered within the label
                VerticalTextAlignment = TextAlignment.Center
            };

            digitContainer.Add(label);

            Grid.SetColumn(digitContainer, position);
            // Grid.SetRow(digitContainer, 0); // Already default
            Add(digitContainer); // Add the container to the main SmoothCounterLabel grid
            _digitLabels.Add(label); // Add the label itself to our tracking list
        }
    }


    private void CancelAllAnimations()
    {
        this.AbortAnimation("DigitAnimation"); // Use a single key for easier cancellation across all digits if needed
        // Or cancel individually if that's more robust for your logic:
        // for (int i = 0; i < _digitLabels.Count; i++)
        // {
        //     this.AbortAnimation($"DigitAnimation{i}");
        // }
        _animationCount = 0;

        // Reset visual state immediately on cancel
         foreach (var label in _digitLabels)
        {
             label.TranslationY = 0;
             label.Opacity = 1;
             label.RotationX = 0;
             label.Scale = 1;
        }
    }

   private void AnimateDigit(int position, char oldDigit, char newDigit, uint duration, bool isIncreasing)
    {
        if (position >= _digitLabels.Count) return;

        var label = _digitLabels[position];
        label.Text = oldDigit.ToString(); // Start with the old digit visible

        // Use configured animation parameters
        double translationY = AnimationTranslationY;
        double rotationXFactor = AnimationRotationXFactor;
        double scaleDivisor = AnimationScaleDivisor;
        double keyFrame1 = AnimationKeyFrame1;
        double keyFrame2 = AnimationKeyFrame2;

        // Define the animation
        var animation = new Animation();
        Action<double> stepAction;

        if (isIncreasing)
        {
            // Animate out (upwards)
            animation.Add(0, keyFrame1, new Animation(v =>
            {
                label.TranslationY = v; // Starts at 0, goes to -translationY
                label.Opacity = Math.Max(0, 1 + v / translationY); // Fade out
                label.RotationX = (v * rotationXFactor);
                label.Scale = 1 - (Math.Abs(v) / scaleDivisor); // Scale down slightly
            }, 0, -translationY, Easing.CubicIn));

            // Switch text halfway through the pause/switch phase
            animation.Add(keyFrame1, keyFrame2, new Animation(v =>
            {
                 // Switch text at the midpoint of this short phase
                if (v >= 0.5 && label.Text != newDigit.ToString())
                {
                    label.Text = newDigit.ToString();
                }
            }, 0, 1, Easing.Linear)); // Linear progress from 0 to 1 during this phase

            // Animate in (from below)
            animation.Add(keyFrame2, 1.0, new Animation(v =>
            {
                label.TranslationY = v; // Starts at translationY, goes to 0
                label.Opacity = Math.Min(1, 1 - v / translationY); // Fade in
                label.RotationX = v * rotationXFactor;
                label.Scale = 1 - (Math.Abs(v) / scaleDivisor); // Scale back to normal
            }, translationY, 0, Easing.CubicOut));
        }
        else // Decreasing
        {
             // Animate out (downwards)
            animation.Add(0, keyFrame1, new Animation(v =>
            {
                label.TranslationY = v; // Starts at 0, goes to translationY
                label.Opacity = Math.Max(0, 1 - v / translationY); // Fade out
                label.RotationX = v * rotationXFactor;
                label.Scale = 1 - (Math.Abs(v) / scaleDivisor); // Scale down slightly
            }, 0, translationY, Easing.CubicIn));

            // Switch text halfway through the pause/switch phase
            animation.Add(keyFrame1, keyFrame2, new Animation(v =>
            {
                 // Switch text at the midpoint of this short phase
                if (v >= 0.5 && label.Text != newDigit.ToString())
                {
                   label.Text = newDigit.ToString();
                }
            }, 0, 1, Easing.Linear));

            // Animate in (from above)
            animation.Add(keyFrame2, 1.0, new Animation(v =>
            {
                label.TranslationY = v; // Starts at -translationY, goes to 0
                label.Opacity = Math.Min(1, 1 + v / translationY); // Fade in
                label.RotationX = v * rotationXFactor;
                label.Scale = 1 - (Math.Abs(v) / scaleDivisor); // Scale back to normal
            }, -translationY, 0, Easing.CubicOut));
        }


        // Commit the animation for this specific digit
        string animationHandle = $"DigitAnimation{position}"; // Unique handle per digit
        label.Animate(animationHandle, animation, 16, duration, Easing.Linear,
            finished: (v, cancelled) =>
            {
                 // Ensure final state is clean, especially if cancelled
                if (!cancelled)
                {
                    label.TranslationY = 0;
                    label.Opacity = 1;
                    label.RotationX = 0;
                    label.Scale = 1;
                    label.Text = newDigit.ToString(); // Ensure final text is correct
                }
                 _animationCount = Math.Max(0, _animationCount - 1); // Decrement safely
            });
    }


    #endregion
}
