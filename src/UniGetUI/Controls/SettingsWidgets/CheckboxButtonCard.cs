using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UniGetUI.Core.SettingsEngine;
using UniGetUI.Core.Tools;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Interface.Widgets
{
    public sealed class CheckboxButtonCard : SettingsCard
    {
        public CheckBox CheckBox;
        public Button Button;
        private bool IS_INVERTED;

        private string setting_name = "";
        public string SettingName
        {
            set {
                setting_name = value;
                IS_INVERTED = value.StartsWith("Disable");
                CheckBox.IsChecked = Settings.Get(setting_name) ^ IS_INVERTED ^ ForceInversion;
                Button.IsEnabled = (CheckBox.IsChecked ?? false) || _buttonAlwaysOn ;
            }
        }

        public bool ForceInversion { get; set; }

        public bool Checked
        {
            get => CheckBox.IsChecked ?? false;
        }
        public event EventHandler<EventArgs>? StateChanged;
        public new event EventHandler<RoutedEventArgs>? Click;

        public string CheckboxText
        {
            set => CheckBox.Content = CoreTools.Translate(value);
        }

        public string ButtonText
        {
            set => Button.Content = CoreTools.Translate(value);
        }

        private bool _buttonAlwaysOn;
        public bool ButtonAlwaysOn
        {
            set => _buttonAlwaysOn = value;
        }

        public CheckboxButtonCard()
        {
            Button = new Button();
            CheckBox = new CheckBox();
            IS_INVERTED = false;

            //ContentAlignment = ContentAlignment.Left;
            //HorizontalAlignment = HorizontalAlignment.Stretch;

            DefaultStyleKey = typeof(CheckboxCard);
            Description = CheckBox;
            Content = Button;
            CheckBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            CheckBox.Checked += (_, _) =>
            {
                Settings.Set(setting_name, true ^ IS_INVERTED ^ ForceInversion);
                StateChanged?.Invoke(this, EventArgs.Empty);
                Button.IsEnabled = true;
            };

            CheckBox.Unchecked += (_, _) =>
            {
                Settings.Set(setting_name, false ^ IS_INVERTED ^ ForceInversion);
                StateChanged?.Invoke(this, EventArgs.Empty);
                Button.IsEnabled = _buttonAlwaysOn;
            };

            Button.MinWidth = 200;
            Button.Click += (s, e) => Click?.Invoke(s, e);
        }
    }
}
