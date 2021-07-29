using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using Microsoft.VisualBasic;

namespace _5b_Save_Loader_2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string FilePath;
        string SWFPath;

        public MainWindow()
        {
            InitializeComponent();

            ToggleButtons(0);
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            SWFName.Text = "";

            ToggleButtons(0);

            FilePath = null;

            OpenFileDialog OpenFile = new OpenFileDialog();

            OpenFile.FileName = "5b.swf";
            OpenFile.Filter = "Adobe Flash movie (*.swf)|*.swf";
            OpenFile.Title = "Select SWF";

            if (OpenFile.ShowDialog() == true)
            {
                FilePath = Directory.GetDirectories(Environment.GetEnvironmentVariable("AppData") + @"\Macromedia\Flash Player\#SharedObjects\")[0] + @"\localhost\" + OpenFile.FileName.Remove(0, 3);
                SWFPath = OpenFile.FileName;

                if (!File.Exists(Path.Combine(FilePath, "bfdia5b.sol")))
                {
                    MessageBox.Show("Please start level 1 for this to work!");
                    return;
                }

                SWFName.Text = Path.GetFileName(FilePath);

                ToggleButtons(1);

                RefreshSaves();
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (SavesList.SelectedIndex == -1) return;

            if (MessageBox.Show("Are you sure you want to replace your current save with this one?", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string[] Saves = Directory.GetDirectories(FilePath);
                File.Copy(Path.Combine(Saves[SavesList.SelectedIndex], "bfdia5b.sol"), Path.Combine(FilePath, "bfdia5b.sol"), true);

                MessageBox.Show(Path.GetFileName(Saves[SavesList.SelectedIndex]) + " has been set as the current save!");
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            MakeBackup();

            SaveButton.IsEnabled = false;
            Thread.Sleep(1000);
            SaveButton.IsEnabled = true;

            RefreshSaves();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(SWFPath);
        }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            if (SavesList.SelectedIndex == -1) return;

            string[] Saves = Directory.GetDirectories(FilePath);

            var Save = Interaction.InputBox("Save File Name", "Save Game", GetTime(), 100, 100);
            if (Save == "") return;

            if (File.Exists(Path.Combine(FilePath, Save, "bfdia5b.sol")))
            {
                if (MessageBox.Show("A save by this name already exists. Do you want to overwrite it?", "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }
            else
            {
                Directory.Move(Path.Combine(FilePath, Saves[SavesList.SelectedIndex]), Path.Combine(FilePath, Save));
            }

            ToggleButtons(2);

            RefreshSaves();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (SavesList.SelectedIndex == -1) return;

            string[] Saves = Directory.GetDirectories(FilePath);

            if (MessageBox.Show("Are you sure you want to PERMANENTLY delete this save?", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            MessageBox.Show(Path.GetFileName(Saves[SavesList.SelectedIndex]) + " has been deleted!");

            Directory.Delete(Path.Combine(FilePath, Saves[SavesList.SelectedIndex]), true);

            ToggleButtons(2);

            RefreshSaves();
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFile = new OpenFileDialog();

            OpenFile.FileName = "bfdia5b.sol";
            OpenFile.Filter = "Shared Object Local Files (*.sol)|*.sol";
            OpenFile.Title = "Import Save";

            if (OpenFile.ShowDialog() == true)
            {
                var Save = Interaction.InputBox("Save File Name", "Save Game", GetTime(), 100, 100);
                if (Save == "") return;

                if (File.Exists(Path.Combine(FilePath, Save, "bfdia5b.sol")))
                {
                    if (MessageBox.Show("A save by this name already exists. Do you want to overwrite it?", "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(FilePath, Save));
                }

                File.Copy(OpenFile.FileName, Path.Combine(FilePath, Save, "bfdia5b.sol"), true);

                File.WriteAllText(Path.Combine(FilePath, Save, "desc.txt"), "Description...");

                ImportButton.IsEnabled = false;
                Thread.Sleep(1000);
                ImportButton.IsEnabled = true;

                RefreshSaves();
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (SavesList.SelectedIndex == -1) return;

            string[] Saves = Directory.GetDirectories(FilePath);

            SaveFileDialog SaveFile = new SaveFileDialog();

            SaveFile.DefaultExt = "sol";
            SaveFile.FileName = "bfdia5b.sol";
            SaveFile.Filter = "Shared Object Local Files (*.sol)|*.sol";
            SaveFile.Title = "Export Save";

            SaveFile.ShowDialog();

            if (SaveFile.FileName == "") return;

            File.Copy(Path.Combine(Saves[SavesList.SelectedIndex], "bfdia5b.sol"), SaveFile.FileName, true);
        }

        private void SavesList_SelectionChanged(object sender, EventArgs e)
        {
            if (SavesList.SelectedIndex == -1) return;

            ToggleButtons(3);

            string[] Saves = Directory.GetDirectories(FilePath);
            var Description = File.ReadAllText(Path.Combine(Saves[SavesList.SelectedIndex], "desc.txt"));

            DescriptionText.Text = File.ReadAllText(Path.Combine(Saves[SavesList.SelectedIndex], "desc.txt"));
        }

        private string GetTime()
        {
            Regex DateRegex = new Regex(@"\b[1-9][1-2]?\/[1-3]?[1-9]\/\d*\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex TimeRegex = new Regex(@"\b[0-2]?[0-9](:[0-5][0-9]){2}\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var DateString = DateTime.Now.ToString();

            var Date = DateRegex.Matches(DateString)[0].Groups[0].ToString();
            var Time = TimeRegex.Matches(DateString)[0].Groups[0].ToString();
            var ampm = DateString.Remove(0, DateString.Length - 2);

            return Date.Replace("/", "-") + "_" + Time.Replace(":", "-") + "_" + ampm;
        }

        private void MakeBackup()
        {
            var Save = Interaction.InputBox("Save File Name", "Save Game", GetTime(), 100, 100);
            if (Save == "") return;

            if (File.Exists(Path.Combine(FilePath, Save, "bfdia5b.sol")))
            {
                if (MessageBox.Show("A save by this name already exists. Do you want to overwrite it?", "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(FilePath, Save));
            }

            File.Copy(Path.Combine(FilePath, "bfdia5b.sol"), Path.Combine(FilePath, Save, "bfdia5b.sol"), true);

            File.WriteAllText(Path.Combine(FilePath, Save, "desc.txt"), "Description...");

            ToggleButtons(2);
        }

        private string ConvertTimestamp(string Timestamp)
        {
            Regex TimestampRegex = new Regex(@"\b[1-9][1-2]?-[1-3]?[1-9]-\d*_[0-2]?[0-9](-[0-5][0-9]){2}_(AM|PM)\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (TimestampRegex.Matches(Timestamp).Count == 0) return Timestamp;

            return Timestamp.Split('_')[0].Replace("-", "/") + " " + Timestamp.Split('_')[1].Replace("-", ":") + " " + Timestamp.Split('_')[2];
        }

        private void DescriptionText_TextChanged(object sender, EventArgs e)
        {
            if (SavesList.SelectedIndex == -1) return;

            string[] Saves = Directory.GetDirectories(FilePath);
            File.WriteAllText(Path.Combine(Saves[SavesList.SelectedIndex], "desc.txt"), DescriptionText.Text);
        }

        private void RefreshSaves()
        {
            SavesList.Items.Clear();

            string[] Saves = Directory.GetDirectories(FilePath);
            for (int i = 0; i < Saves.Length; i++)
            {
                SavesList.Items.Add(ConvertTimestamp(Saves[i].Split('\\')[Saves[i].Split('\\').Length - 1]));
            }

            ToggleButtons(2);
        }

        private void ToggleButtons(int Reason)
        {
            if (Reason == 0) // No file chosen
            {
                Step2.Visibility = Visibility.Hidden;
                LoadButton.Visibility = Visibility.Hidden;
                SaveButton.Visibility = Visibility.Hidden;
                OpenButton.Visibility = Visibility.Hidden;
                SavesList.Visibility = Visibility.Hidden;
                DescriptionText.Visibility = Visibility.Hidden;
                LoadButton.Visibility = Visibility.Hidden;
                RenameButton.Visibility = Visibility.Hidden;
                DeleteButton.Visibility = Visibility.Hidden;
                ImportButton.Visibility = Visibility.Hidden;
                ExportButton.Visibility = Visibility.Hidden;
            }

            if (Reason == 1) // File chosen
            {
                Step2.Visibility = Visibility.Visible;
                LoadButton.Visibility = Visibility.Visible;
                SaveButton.Visibility = Visibility.Visible;
                OpenButton.Visibility = Visibility.Visible;
                SavesList.Visibility = Visibility.Visible;
                DescriptionText.Visibility = Visibility.Visible;
                LoadButton.Visibility = Visibility.Visible;
                RenameButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
                ImportButton.Visibility = Visibility.Visible;
                ExportButton.Visibility = Visibility.Visible;
            }

            if (Reason == 2) // Nothing selected
            {
                LoadButton.IsEnabled = false;
                RenameButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                ExportButton.IsEnabled = false;
                DescriptionText.IsEnabled = false;
            }

            if (Reason == 3) // Something selected
            {
                LoadButton.IsEnabled = true;
                RenameButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;
                ExportButton.IsEnabled = true;
                DescriptionText.IsEnabled = true;
            }
        }
    }
}
