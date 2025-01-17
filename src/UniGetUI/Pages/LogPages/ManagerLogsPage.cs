using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using UniGetUI.Core.Tools;
using UniGetUI.PackageEngine;
using UniGetUI.PackageEngine.Interfaces;
using UniGetUI.PackageEngine.ManagerClasses.Classes;

namespace UniGetUI.Interface.Pages.LogPage
{
    public class ManagerLogsPage : BaseLogPage
    {

        public ManagerLogsPage() : base(true)
        {

        }

        public void LoadForManager(IPackageManager manager)
        {
            bool IS_DARK = this.ActualTheme == ElementTheme.Dark;
            bool verbose = LogLevelCombo.SelectedValue?.ToString()?.Contains(CoreTools.Translate("Verbose")) ?? false;

            if(!verbose) SelectLogLevelByName(manager.DisplayName);

            IManagerLogger TaskLogger = manager.TaskLogger;
            LogTextBox.Blocks.Clear();
            Paragraph versionParagraph = new();
            versionParagraph.Inlines.Add(new Run { Text = $"Manager {manager.DisplayName} with version:\n" });
            versionParagraph.Inlines.Add(new Run { Text = manager.Status.Version });
            versionParagraph.Inlines.Add(new Run { Text = "\n\n——————————————————————————————————————————\n\n" });
            LogTextBox.Blocks.Add(versionParagraph);

            foreach (ITaskLogger operation in TaskLogger.Operations)
            {
                Paragraph p = new();
                foreach (string line in operation.AsColoredString(verbose))
                {
                    Brush color = line[0] switch
                    {
                        '0' => new SolidColorBrush { Color = IS_DARK ? DARK_WHITE : LIGHT_WHITE },
                        '1' => new SolidColorBrush { Color = IS_DARK ? DARK_LIGHT_GREY : LIGHT_LIGHT_GREY },
                        '2' => new SolidColorBrush { Color = IS_DARK ? DARK_RED : LIGHT_RED },
                        '3' => new SolidColorBrush { Color = IS_DARK ? DARK_BLUE : LIGHT_BLUE },
                        '4' => new SolidColorBrush { Color = IS_DARK ? DARK_GREEN : LIGHT_GREEN },
                        '5' => new SolidColorBrush { Color = IS_DARK ? DARK_YELLOW : LIGHT_YELLOW },
                        _ => new SolidColorBrush { Color = IS_DARK ? DARK_YELLOW : LIGHT_YELLOW },
                    };
                    p.Inlines.Add(new Run { Text = line[1..] + "\n", Foreground = color });
                }
                ((Run)p.Inlines[^1]).Text = ((Run)p.Inlines[^1]).Text.TrimEnd();
                LogTextBox.Blocks.Add(p);
            }
        }

        public override void LoadLog(bool isReload = false)
        {
            foreach (IPackageManager manager in PEInterface.Managers)
            {
                if (LogLevelCombo.SelectedValue?.ToString()?.Contains(manager.DisplayName) ?? false)
                {
                    LoadForManager(manager);
                    break;
                }

                if(isReload) MainScroller.ScrollToVerticalOffset(MainScroller.ScrollableHeight);
            }
        }

        protected override void LoadLogLevels()
        {
            LogLevelCombo.Items.Clear();
            foreach (IPackageManager manager in PEInterface.Managers)
            {
                LogLevelCombo.Items.Add(manager.DisplayName);
                LogLevelCombo.Items.Add($"{manager.DisplayName} ({CoreTools.Translate("Verbose")})");
            }
            LogLevelCombo.SelectedIndex = 0;
        }
    }
}
