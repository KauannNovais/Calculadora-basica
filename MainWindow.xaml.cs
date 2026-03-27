using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SimpleBlueCalculator;

public partial class MainWindow : Window
{
    private double _firstNumber;
    private string _currentOperator = string.Empty;
    private bool _isNewInput = true;
    private readonly CultureInfo _culture = new("pt-BR");

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Number_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;

        if (_isNewInput || DisplayText.Text == "0")
        {
            DisplayText.Text = button.Content.ToString() ?? "0";
            _isNewInput = false;
            return;
        }

        DisplayText.Text += button.Content.ToString();
    }

    private void Decimal_Click(object sender, RoutedEventArgs e)
    {
        var decimalSeparator = _culture.NumberFormat.NumberDecimalSeparator;

        if (_isNewInput)
        {
            DisplayText.Text = $"0{decimalSeparator}";
            _isNewInput = false;
            return;
        }

        if (!DisplayText.Text.Contains(decimalSeparator))
        {
            DisplayText.Text += decimalSeparator;
        }
    }

    private void Operator_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;

        if (!double.TryParse(DisplayText.Text, NumberStyles.Any, _culture, out _firstNumber))
        {
            DisplayText.Text = "Erro";
            return;
        }

        _currentOperator = button.Content.ToString() ?? string.Empty;
        OperationText.Text = $"{_firstNumber.ToString("N2", _culture)} {_currentOperator}";
        _isNewInput = true;
    }

    private void Equals_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_currentOperator)) return;

        if (!double.TryParse(DisplayText.Text, NumberStyles.Any, _culture, out var secondNumber))
        {
            DisplayText.Text = "Erro";
            return;
        }

        try
        {
            var result = _currentOperator switch
            {
                "+" => _firstNumber + secondNumber,
                "-" => _firstNumber - secondNumber,
                "×" => _firstNumber * secondNumber,
                "÷" when secondNumber == 0 => throw new DivideByZeroException(),
                "÷" => _firstNumber / secondNumber,
                _ => secondNumber
            };

            OperationText.Text = $"{_firstNumber.ToString("N2", _culture)} {_currentOperator} {secondNumber.ToString("N2", _culture)} =";
            DisplayText.Text = result.ToString("N2", _culture);
            _isNewInput = true;
            _currentOperator = string.Empty;
        }
        catch (DivideByZeroException)
        {
            DisplayText.Text = "Não é possível dividir por zero";
            OperationText.Text = string.Empty;
            _isNewInput = true;
            _currentOperator = string.Empty;
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        _firstNumber = 0;
        _currentOperator = string.Empty;
        _isNewInput = true;
        DisplayText.Text = "0";
        OperationText.Text = string.Empty;
    }

    private void Backspace_Click(object sender, RoutedEventArgs e)
    {
        if (_isNewInput || string.IsNullOrEmpty(DisplayText.Text) || DisplayText.Text.Length == 1)
        {
            DisplayText.Text = "0";
            return;
        }

        DisplayText.Text = DisplayText.Text[..^1];
    }

    private void Percent_Click(object sender, RoutedEventArgs e)
    {
        if (!double.TryParse(DisplayText.Text, NumberStyles.Any, _culture, out var currentNumber))
        {
            DisplayText.Text = "Erro";
            return;
        }

        currentNumber /= 100;
        DisplayText.Text = currentNumber.ToString("N2", _culture);
        _isNewInput = true;
    }
}
