using System.Globalization;

namespace FantoApp
{
    public partial class MainPage : ContentPage
    {
        private static readonly TimeSpan CoffeeBreakOffset = TimeSpan.FromHours(1);
        private static readonly TimeSpan LunchStartOffset = TimeSpan.FromHours(4);
        private static readonly TimeSpan LunchDuration = TimeSpan.FromHours(1);
        private static readonly TimeSpan NormalWorkDuration = new(8, 48, 0);
        private static readonly TimeSpan MaximumWorkDuration = TimeSpan.FromHours(10);

        private double _lastAppliedWidth = -1;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ApplyResponsiveLayout(width);
        }

        private void ApplyResponsiveLayout(double width)
        {
            if (width <= 0 || Math.Abs(width - _lastAppliedWidth) < 1)
                return;

            _lastAppliedWidth = width;

            // Scale the border padding and the title font size to fit small phone screens.
            double titleFontSize;
            double titleSpacing;
            double borderPadding;

            switch (width)
            {
                case < 360:
                    titleFontSize = 26;
                    titleSpacing = 1;
                    borderPadding = 10;
                    break;
                case < 420:
                    titleFontSize = 32;
                    titleSpacing = 1.5;
                    borderPadding = 12;
                    break;
                case < 600:
                    titleFontSize = 40;
                    titleSpacing = 2.5;
                    borderPadding = 16;
                    break;
                default:
                    titleFontSize = 52;
                    titleSpacing = 4;
                    borderPadding = 24;
                    break;
            }

            TitleShadowLabel.FontSize = titleFontSize;
            TitleShadowLabel.CharacterSpacing = titleSpacing;
            TitleForegroundLabel.FontSize = titleFontSize;
            TitleForegroundLabel.CharacterSpacing = titleSpacing;
            RootBorder.Padding = borderPadding;
        }

        private void OnStartTapped(object? sender, TappedEventArgs e)
        {
            GoToEntryScreen();
        }

        private void OnBackToStartTapped(object? sender, TappedEventArgs e)
        {
            GoToStartScreen();
        }

        private void OnBackToConsoleTapped(object? sender, TappedEventArgs e)
        {
            GoToEntryScreen();
        }

        private void OnStartTimeEntryCompleted(object? sender, EventArgs e)
        {
            CalculateAndShowResults();
        }

        private void GoToStartScreen()
        {
            StartPanel.IsVisible = true;
            EntryPanel.IsVisible = false;
            ResultPanel.IsVisible = false;
        }

        private void GoToEntryScreen()
        {
            StartPanel.IsVisible = false;
            EntryPanel.IsVisible = true;
            ResultPanel.IsVisible = false;

            ErrorLabel.IsVisible = false;
            StartTimeEntry.Text = string.Empty;
            StartTimeEntry.Focus();
        }

        private void CalculateAndShowResults()
        {
            var input = StartTimeEntry.Text?.Trim() ?? string.Empty;

            if (!TryParseTime(input, out var startTime))
            {
                ErrorLabel.IsVisible = true;
                return;
            }

            ErrorLabel.IsVisible = false;

            var coffeeBreak = startTime + CoffeeBreakOffset;
            var lunchStart = startTime + LunchStartOffset;
            var lunchEnd = lunchStart + LunchDuration;
            var normalExit = startTime + LunchDuration + NormalWorkDuration;
            var maximumExit = startTime + LunchDuration + MaximumWorkDuration;

            EntryValueLabel.Text = FormatTime(startTime);
            CoffeeBreakValueLabel.Text = FormatTime(coffeeBreak);
            LunchStartValueLabel.Text = FormatTime(lunchStart);
            LunchEndValueLabel.Text = FormatTime(lunchEnd);
            NormalExitValueLabel.Text = $"{FormatTime(normalExit)}  ({FormatDuration(NormalWorkDuration)} worked)";
            MaximumExitValueLabel.Text = $"{FormatTime(maximumExit)}  ({FormatDuration(MaximumWorkDuration)} worked)";

            StartPanel.IsVisible = false;
            EntryPanel.IsVisible = false;
            ResultPanel.IsVisible = true;
        }

        private static bool TryParseTime(string input, out TimeSpan time)
        {
            return TimeSpan.TryParseExact(input, @"hh\:mm", CultureInfo.InvariantCulture, out time)
                   && time >= TimeSpan.Zero
                   && time < TimeSpan.FromDays(1);
        }

        private static string FormatTime(TimeSpan time)
        {
            var normalized = TimeSpan.FromMinutes(((time.TotalMinutes % 1440) + 1440) % 1440);
            return $"{(int)normalized.TotalHours:D2}:{normalized.Minutes:D2}";
        }

        private static string FormatDuration(TimeSpan duration)
        {
            return duration.Minutes == 0
                ? $"{(int)duration.TotalHours}h"
                : $"{(int)duration.TotalHours}h{duration.Minutes:D2}";
        }
    }
}
