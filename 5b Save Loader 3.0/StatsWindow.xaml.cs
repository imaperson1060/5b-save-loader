using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace _5b_Save_Loader_3._0
{
    /// <summary>
    /// Interaction logic for StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        public string[] Saves;
        public string FilePath;
        public string SWFPath;
        public int Selected;


        public StatsWindow()
        {
            InitializeComponent();
        }

        private void WinToken_MouseEnter(object sender, RoutedEventArgs e)
        {
            var WTGet = new BitmapImage();
            WTGet.BeginInit();
            WTGet.UriSource = new Uri(@"images/win_token_get.gif", UriKind.Relative);
            WTGet.EndInit();
            ImageBehavior.SetAnimatedSource(WinToken, WTGet);
            ImageBehavior.SetRepeatBehavior(WinToken, new System.Windows.Media.Animation.RepeatBehavior(1));
        }

        private void WinToken_AnimationCompleted(object sender, RoutedEventArgs e)
        {
            if (WinToken.Source.ToString().Split(';')[1] == "component/images/win_token_get.gif")
            {
                var WTHover = new BitmapImage();
                WTHover.BeginInit();
                WTHover.UriSource = new Uri(@"images/win_token_hover.gif", UriKind.Relative);
                WTHover.EndInit();
                ImageBehavior.SetAnimatedSource(WinToken, WTHover);
                ImageBehavior.SetRepeatBehavior(WinToken, new System.Windows.Media.Animation.RepeatBehavior(0));
            }
        }

        private void ContinueButton_MouseEnter(object sender, RoutedEventArgs e)
        {
            BitmapImage ClickShadow = new BitmapImage();
            ClickShadow.BeginInit();
            ClickShadow.UriSource = new Uri(@"images/btn_hover.png", UriKind.Relative);
            ClickShadow.EndInit();
            ContinueShadow.Source = ClickShadow;
        }

        private void ContinueButton_MouseLeave(object sender, RoutedEventArgs e)
        {
            ContinueShadow.Source = null;
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage ClickShadow = new BitmapImage();
            ClickShadow.BeginInit();
            ClickShadow.UriSource = new Uri(@"images/btn_click.png", UriKind.Relative);
            ClickShadow.EndInit();
            ContinueShadow.Source = ClickShadow;

            if (Selected == -1)
            {
                Process.Start(SWFPath);
                return;
            }

            if (MessageBox.Show("Are you sure you want to replace your current save with this one?", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                File.Copy(Path.Combine(Saves[Selected], "bfdia5b.sol"), Path.Combine(FilePath, "bfdia5b.sol"), true);

                MessageBox.Show(Path.GetFileName(Saves[Selected]) + " has been set as the current save!");

                Process.Start(SWFPath);
            }
        }

        private void ContinueButton_MouseUp(object sender, RoutedEventArgs e)
        {
            ContinueShadow.Source = null;
        }
    }
}
