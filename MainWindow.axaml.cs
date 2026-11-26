using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace minutnik;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;
    private TimeSpan _remainingTime;
    private bool _isRunning;
    private bool _isWorkPhase = true;
    private bool _hasStartedCurrentPhase;
    private int _completedCycles;

    public MainWindow()
    {
        InitializeComponent();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += OnTimerTick;

        ResetToWorkPhase();
    }

    private void OnStartPauseClick(object? sender, RoutedEventArgs e)
    {
        if (_isRunning)
        {
            PauseTimer();
            return;
        }

        if (!_hasStartedCurrentPhase)
        {
            if (!TryGetConfiguredMinutes(_isWorkPhase ? WorkMinutesInput : BreakMinutesInput, out double minutes))
            {
                return;
            }

            _remainingTime = TimeSpan.FromSeconds(Math.Round(minutes * 60));
            UpdateTimerDisplay();
        }

        ClearStatusMessage();
        _hasStartedCurrentPhase = true;
        _isRunning = true;
        StartPauseButton.Content = "Pauza";
        _timer.Start();
    }

    private void OnResetClick(object? sender, RoutedEventArgs e)
    {
        ResetToWorkPhase();
    }

    private void OnDurationChanged(object? sender, TextChangedEventArgs e)
    {
        if (_timer is null || _isRunning || _hasStartedCurrentPhase)
        {
            return;
        }

        if (TryGetConfiguredMinutes(_isWorkPhase ? WorkMinutesInput : BreakMinutesInput, out double minutes))
        {
            _remainingTime = TimeSpan.FromSeconds(Math.Round(minutes * 60));
            UpdateTimerDisplay();
            ClearStatusMessage();
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (_remainingTime > TimeSpan.Zero)
        {
            _remainingTime -= TimeSpan.FromSeconds(1);
            UpdateTimerDisplay();
        }

        if (_remainingTime <= TimeSpan.Zero)
        {
            CompleteCurrentPhase();
        }
    }

    private void CompleteCurrentPhase()
    {
        if (_isWorkPhase)
        {
            _completedCycles++;
            CyclesText.Text = $"Skończone cykle: {_completedCycles}";

            _isWorkPhase = false;
            PhaseText.Text = "Faza: Przerwa";

            double breakMinutes = TryGetConfiguredMinutes(BreakMinutesInput, out double parsedBreak)
                ? parsedBreak
                : 5;

            _remainingTime = TimeSpan.FromSeconds(Math.Round(breakMinutes * 60));
            _hasStartedCurrentPhase = true;
            UpdateTimerDisplay();
            // Break starts automatically right after work ends.
        }
        else
        {
            _timer.Stop();
            _isRunning = false;
            _isWorkPhase = true;
            _hasStartedCurrentPhase = false;
            PhaseText.Text = "Faza: Praca";
            StartPauseButton.Content = "Start";

            double workMinutes = TryGetConfiguredMinutes(WorkMinutesInput, out double parsedWork)
                ? parsedWork
                : 25;

            _remainingTime = TimeSpan.FromSeconds(Math.Round(workMinutes * 60));
            UpdateTimerDisplay();
        }
    }

    private void PauseTimer()
    {
        _timer.Stop();
        _isRunning = false;
        StartPauseButton.Content = "Start";
    }

    private void ResetToWorkPhase()
    {
        _timer.Stop();
        _isRunning = false;
        _isWorkPhase = true;
        _hasStartedCurrentPhase = false;

        PhaseText.Text = "Faza: Praca";
        StartPauseButton.Content = "Start";

        double workMinutes = 25;
        if (TryGetConfiguredMinutes(WorkMinutesInput, out double parsedWork))
        {
            workMinutes = parsedWork;
            ClearStatusMessage();
        }

        _remainingTime = TimeSpan.FromSeconds(Math.Round(workMinutes * 60));
        UpdateTimerDisplay();
    }

    private bool TryGetConfiguredMinutes(TextBox input, out double minutes)
    {
        string raw = input.Text?.Trim() ?? string.Empty;
        if (double.TryParse(raw, NumberStyles.Float, CultureInfo.CurrentCulture, out minutes) && minutes > 0 && minutes <= 999)
        {
            return true;
        }

        ShowStatusMessage("Podaj poprawną długość (liczba większa od 0, np. 25 lub 0,5).");
        minutes = 0;
        return false;
    }

    private void UpdateTimerDisplay()
    {
        int totalMinutes = (int)_remainingTime.TotalMinutes;
        int seconds = _remainingTime.Seconds;
        TimerText.Text = $"{totalMinutes:D2}:{seconds:D2}";
    }

    private void ShowStatusMessage(string message)
    {
        StatusMessageText.Text = message;
        StatusMessageText.IsVisible = true;
    }

    private void ClearStatusMessage()
    {
        StatusMessageText.Text = string.Empty;
        StatusMessageText.IsVisible = false;
    }
}
