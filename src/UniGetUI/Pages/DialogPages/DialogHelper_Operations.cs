using Windows.UI;
using ExternalLibraries.Clipboard;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using UniGetUI.Controls.OperationWidgets;
using UniGetUI.Core.Tools;
using UniGetUI.Interface.Enums;
using UniGetUI.Interface.Widgets;
using UniGetUI.PackageOperations;

namespace UniGetUI.Pages.DialogPages;

public static partial class DialogHelper
{
    public static async Task ShowOperationFailedDialog(AbstractOperation operation, OperationControl opControl)
    {
        ContentDialog dialog = new()
        {
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style, XamlRoot = Window.XamlRoot,
            Resources = { ["ContentDialogMaxWidth"] = 850,
                ["ContentDialogMaxHeight"] = 800,
            },
            Title = operation.Metadata.FailureTitle,
        };

        Grid grid = new()
        {
            RowSpacing = 16,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };

        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        TextBlock headerContent = new()
        {
            TextWrapping = TextWrapping.WrapWholeWords,
            Text = $"{operation.Metadata.FailureMessage}.\n"
                   + CoreTools.Translate("Please see the Command-line Output or refer to the Operation History for further information about the issue.")
        };

        StackPanel HeaderPanel = new() { Orientation = Orientation.Horizontal, Spacing = 8 };

        HeaderPanel.Children.Add(new LocalIcon(IconType.Console)
        {
            VerticalAlignment = VerticalAlignment.Center,
            Height = 24,
            Width = 24,
            HorizontalAlignment = HorizontalAlignment.Left
        });

        HeaderPanel.Children.Add(new TextBlock
        {
            Text = CoreTools.Translate("Command-line Output"),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });

        RichTextBlock CommandLineOutput = new()
        {
            Padding = new Thickness(6),
            FontFamily = new FontFamily("Consolas"),
            TextWrapping = TextWrapping.Wrap,
        };

        ScrollViewer ScrollView = new()
        {
            MaxHeight = MainApp.Instance.MainWindow.NavigationPage.ActualHeight > 800? 500: 300,
            MaxWidth = 800,
            CornerRadius = new CornerRadius(6),
            Background = (Brush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"],
            Content = CommandLineOutput,
            VerticalScrollMode = ScrollMode.Enabled,
            HorizontalScrollMode = ScrollMode.Disabled,
        };

        Paragraph par = new();

        SolidColorBrush errorColor = (SolidColorBrush)Application.Current.Resources["SystemFillColorCriticalBrush"];
        SolidColorBrush debugColor = (SolidColorBrush)Application.Current.Resources["SystemFillColorNeutralBrush"];

        foreach (var line in operation.GetOutput())
        {
            if (line.Item2 is AbstractOperation.LineType.Information)
            {
                par.Inlines.Add(new Run { Text = line.Item1 + "\x0a" });
            }
            else if (line.Item2 is AbstractOperation.LineType.VerboseDetails)
            {
                par.Inlines.Add(new Run { Text = line.Item1 + "\x0a", Foreground = debugColor });
            }
            else
            {
                par.Inlines.Add(new Run { Text = line.Item1 + "\x0a", Foreground = errorColor });
            }
        }

        CommandLineOutput.Blocks.Add(par);

        grid.Children.Add(headerContent);
        grid.Children.Add(ScrollView);
        Grid.SetRow(headerContent, 0);
        Grid.SetRow(ScrollView, 1);

        var CloseButton = new Button
        {
            Content = CoreTools.Translate("Close"), HorizontalAlignment = HorizontalAlignment.Stretch, Height = 30,
        };
        CloseButton.Click += (_, _) =>
        {
            dialog.Hide();
        };
        Control _retryButton;

        var retryOptions = opControl.GetRetryOptions(dialog.Hide);
        if (retryOptions.Any())
        {
            SplitButton RetryButton = new SplitButton
            {
                Content = CoreTools.Translate("Retry"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 30,
            };
            RetryButton.Click += (_, _) =>
            {
                operation.Retry(AbstractOperation.RetryMode.Retry);
                dialog.Hide();
            };
            BetterMenu menu = new();
            RetryButton.Flyout = menu;
            foreach (var opt in retryOptions)
            {
                menu.Items.Add(opt);
            }

            _retryButton = RetryButton;
        }
        else
        {
            var RetryButton = new Button
            {
                Content = CoreTools.Translate("Retry"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 30,
            };
            RetryButton.Click += (_, _) =>
            {
                operation.Retry(AbstractOperation.RetryMode.Retry);
                dialog.Hide();
            };
            _retryButton = RetryButton;
        }

        Grid sp = new()
        {
            Margin = new Thickness(-25, 0, -25, -25),
            ColumnSpacing = 8,
            Padding = new Thickness(30),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Background = (Brush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"]
        };
        sp.ColumnDefinitions.Add(new ColumnDefinition {Width = new(1, GridUnitType.Star)});
        sp.ColumnDefinitions.Add(new ColumnDefinition {Width = new(1, GridUnitType.Star)});
        sp.Children.Add(_retryButton);
        Grid.SetColumn(CloseButton, 1);
        sp.Children.Add(CloseButton);
        Grid.SetRow(sp, 3);
        Grid.SetColumn(sp, 0);
        grid.Children.Add(sp);
        dialog.Content = grid;

        await Window.ShowDialogAsync(dialog);
    }

    public static async Task ShowLiveLogDialog(AbstractOperation operation)
    {
        bool LastLineWasProgress = false;

        ContentDialog OutputDialog = new ContentDialog
        {
            Style = (Style)Application.Current.Resources["DefaultContentDialogStyle"],
            XamlRoot = Window.XamlRoot
        };
        OutputDialog.Resources["ContentDialogMaxWidth"] = 1200;
        OutputDialog.Resources["ContentDialogMaxHeight"] = 1000;

        var LiveOutputTextBlock = new RichTextBlock
        {
            Margin = new Thickness(8),
            FontFamily = new FontFamily("Consolas")
        };

        var LiveOutputScrollBar = new ScrollViewer
        {
            CornerRadius = new CornerRadius(6),
            Background = (Brush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"],
            Height = 400,
            Width = 600,
            Content = LiveOutputTextBlock
        };

        OutputDialog.Title = CoreTools.Translate("Live output");
        OutputDialog.CloseButtonText = CoreTools.Translate("Close");

        OutputDialog.SizeChanged += (_, _) =>
        {
            LiveOutputScrollBar.MinWidth = MainApp.Instance.MainWindow.NavigationPage.ActualWidth - 400;
            LiveOutputScrollBar.MinHeight = MainApp.Instance.MainWindow.NavigationPage.ActualHeight - 200;
        };

        OutputDialog.Content = LiveOutputScrollBar;
        LiveOutputTextBlock.Blocks.Clear();
        Paragraph par = new()
        {
            LineHeight = 4.8
        };
        SolidColorBrush errorColor = (SolidColorBrush)Application.Current.Resources["SystemFillColorCriticalBrush"];
        SolidColorBrush debugColor = (SolidColorBrush)Application.Current.Resources["SystemFillColorNeutralBrush"];

        foreach (var line in operation.GetOutput())
        {
            if (line.Item2 is AbstractOperation.LineType.Information)
            {
                par.Inlines.Add(new Run { Text = line.Item1 + "\x0a" });
            }
            else if (line.Item2 is AbstractOperation.LineType.VerboseDetails)
            {
                par.Inlines.Add(new Run { Text = line.Item1 + "\x0a", Foreground = debugColor });
            }
            else
            {
                par.Inlines.Add(new Run { Text = line.Item1 + "\x0a", Foreground = errorColor });
            }
        }

        EventHandler<(string, AbstractOperation.LineType)> AddLineLambda = (_, line) => MainApp.Dispatcher.TryEnqueue(() =>
        {
            if(LastLineWasProgress) par.Inlines.RemoveAt(par.Inlines.Count-1);

            LastLineWasProgress = false;
            if (line.Item2 is AbstractOperation.LineType.Information)
            {
                par.Inlines.Add(new Run { Text = line.Item1 + "\x0a" });
            }
            else if (line.Item2 is AbstractOperation.LineType.VerboseDetails)
            {
                par.Inlines.Add(new Run { Text = line.Item1 + "\x0a", Foreground = debugColor });
            }
            else if (line.Item2 is AbstractOperation.LineType.Error)
            {
                par.Inlines.Add(new Run { Text = line.Item1 + "\x0a", Foreground = errorColor });
            }
            else
            {
                LastLineWasProgress = true;
                par.Inlines.Add(new Run { Text = line.Item1 + "\x0a" });
            }

            LiveOutputTextBlock.Blocks.Clear();
            LiveOutputTextBlock.Blocks.Add(par);
            LiveOutputScrollBar.ScrollToVerticalOffset(LiveOutputScrollBar.ScrollableHeight);
        });

        LiveOutputTextBlock.Blocks.Add(par);

        operation.LogLineAdded += AddLineLambda;
        if (await MainApp.Instance.MainWindow.ShowDialogAsync(OutputDialog) == ContentDialogResult.Secondary)
        {
            LiveOutputScrollBar.ScrollToVerticalOffset(LiveOutputScrollBar.ScrollableHeight);
            string text = "";
            foreach (var line in par.Inlines)
            {
                if (line is Run run)
                    text += run + "\n";
            }
            WindowsClipboard.SetText(text);
        }
        operation.LogLineAdded -= AddLineLambda;
    }
}

