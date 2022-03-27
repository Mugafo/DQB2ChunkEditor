using System;
using System.Windows;
using System.Windows.Input;

namespace DQB2ChunkEditor.Windows;

public partial class InputValue : Window
{
    public int MinValue { get; set; } = 0;
    public int MaxValue { get; set; } = 95;
    public int CurrentValue { get; set; } = 0;
    public string ResponseText => InputText.Text;

    public InputValue()
    {
        InitializeComponent();

        InputText.Focus();
    }

    private void OkButton_OnClick(Object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void InputText_OnKeyDown(Object sender, KeyEventArgs e)
    {
        if (e.Key is Key.Return or Key.Enter)
        {
            DialogResult = true;
        }
    }
}
