using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using Microsoft.VisualBasic;

namespace _5b_Save_Loader_4._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FilePath;
        public string SWFPath;
        public List<Levelpack> levelpack;
        //SharedObject so;

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

                var SaveDirectory = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "5bsl");

                if (!Directory.Exists(SaveDirectory))
                {
                    Directory.CreateDirectory(SaveDirectory);
                }

                File.WriteAllText(Path.Combine(SaveDirectory, "last.dat"), SWFPath);

                //so = SharedObjectParser.Parse(Path.Combine(FilePath, "bfdia5b.sol"));
            }
        }

        private void SelectButton_Right_Click(object sender, EventArgs e)
        {
            var SaveDirectory = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "5bsl");

            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }

            if (File.Exists(Path.Combine(SaveDirectory, "last.dat")))
            {
                var Last = File.ReadAllText(Path.Combine(SaveDirectory, "last.dat"));

                if (!File.Exists(Last))
                {
                    MessageBox.Show("The last SWF opened no longer exists.");
                    return;
                }

                FilePath = Directory.GetDirectories(Environment.GetEnvironmentVariable("AppData") + @"\Macromedia\Flash Player\#SharedObjects\")[0] + @"\localhost\" + Last.Remove(0, 3);
                SWFPath = Last;

                if (!File.Exists(Path.Combine(FilePath, "bfdia5b.sol")))
                {
                    MessageBox.Show("Please start level 1 for this to work!");
                    return;
                }

                SWFName.Text = Path.GetFileName(FilePath);

                ToggleButtons(1);

                RefreshSaves();
            }

            // Chrome deleted all Flash saves which makes this pretty pointless.
            /*var OnlineSWF = Interaction.InputBox("Enter the address of the SWF file, NOT the website it was embedded on (DM imaperson#1060 for help).", "Export Web Save", "http://battlefordreamisland.com/5b/5b.swf", 100, 100);
            if ((OnlineSWF == "") || (OnlineSWF.Substring(OnlineSWF.Length - 4) != ".swf"))
            {
                MessageBox.Show("This is not a valid file. Please DM imaperson#1060 for more help.");
                return;
            }

            var Browser = Interaction.InputBox("Please enter the name of the browser you played on. Supported browsers: \"Chrome\", \"Firefox\"\nIf your browser was not listed, DM imaperson#1060 for help.", "Export Web Save", "Chrome", 100, 100);
            string[] SupportedBrowsers = { "Chrome", "Firefox" };
            if (!SupportedBrowsers.Contains(Browser))
            {
                MessageBox.Show("That browser is not currently supported. DM imaperson#1060 for more help.");
                return;
            }

            if (Browser == "Chrome")
            {
                if (!File.Exists(Path.Combine(Directory.GetDirectories(Environment.GetEnvironmentVariable("LocalAppData") + @"\Google\Chrome\User Data\Default\Pepper Data\Shockwave Flash\WritableRoot\#SharedObjects")[0], OnlineSWF.Replace("http://", "").Replace("https://", ""), "bfdia5b.sol")))
                {
                    MessageBox.Show(Path.Combine(Directory.GetDirectories(Environment.GetEnvironmentVariable("LocalAppData") + @"\Google\Chrome\User Data\Default\Pepper Data\Shockwave Flash\WritableRoot\#SharedObjects")[0], OnlineSWF, "bfdia5b.sol"));
                    MessageBox.Show("There is no save file for that SWF.");
                    return;
                }

                SaveFileDialog SaveFile = new SaveFileDialog();

                SaveFile.DefaultExt = "sol";
                SaveFile.FileName = "bfdia5b.sol";
                SaveFile.Filter = "Shared Object Local Files (*.sol)|*.sol";
                SaveFile.Title = "Export Save";

                SaveFile.ShowDialog();

                if (SaveFile.FileName == "") return;

                File.Copy(Path.Combine(Directory.GetDirectories(Environment.GetEnvironmentVariable("LocalAppData") + @"\Google\Chrome\User Data\Default\Pepper Data\Shockwave Flash\WritableRoot\#SharedObjects")[0], OnlineSWF.Replace("http://", "").Replace("https://", ""), "bfdia5b.sol"), SaveFile.FileName, true);
            }
            else if (Browser == "Firefox")
            {
                if (!File.Exists(Path.Combine(Directory.GetDirectories(Environment.GetEnvironmentVariable("AppData") + @"\Macromedia\Flash Player\#SharedObjects")[0], OnlineSWF.Replace("http://", "").Replace("https://", ""), "bfdia5b.sol")))
                {
                    MessageBox.Show("There is no save file for that SWF.");
                    return;
                }

                SaveFileDialog SaveFile = new SaveFileDialog();

                SaveFile.DefaultExt = "sol";
                SaveFile.FileName = "bfdia5b.sol";
                SaveFile.Filter = "Shared Object Local Files (*.sol)|*.sol";
                SaveFile.Title = "Export Save";

                SaveFile.ShowDialog();

                if (SaveFile.FileName == "") return;

                File.Copy(Path.Combine(Directory.GetDirectories(Environment.GetEnvironmentVariable("AppData") + @"\Macromedia\Flash Player\#SharedObjects")[0], OnlineSWF.Replace("http://", "").Replace("https://", ""), "bfdia5b.sol"), SaveFile.FileName, true);
            }*/
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

        private void ViewButton_Click(object sender, EventArgs e)
        {
            StatsWindow Stats = new StatsWindow();
            Stats.Show();

            string[] Saves = Directory.GetDirectories(FilePath);
            SharedObject so;

            Stats.Saves = Saves;
            Stats.FilePath = FilePath;
            Stats.SWFPath = SWFPath;
            Stats.Selected = SavesList.SelectedIndex;

            if (SavesList.SelectedIndex == -1)
            {
                so = SharedObjectParser.Parse(Path.Combine(FilePath, "bfdia5b.sol"));
                Stats.Title = "Current Stats for " + Path.GetFileNameWithoutExtension(SWFPath);
            }
            else
            {
                so = SharedObjectParser.Parse(Path.Combine(Saves[SavesList.SelectedIndex], "bfdia5b.sol"));
                Stats.Title = "Stats for " + Path.GetFileName(Saves[SavesList.SelectedIndex]);
            }


            int Necessary = 0;
            int levelCount = 0;

            var LevelpackPath = GetLevelpack();
            if (LevelpackPath == null)
            {
                MessageBox.Show("A valid levelpack could not be found so the stats that rely on a levelpack will be set to their defaults.");
            }
            else
            {
                levelpack = LevelpackParser.Parse(LevelpackPath);

                for (var i = 0; i < so.Get("levelProgress").int_val; i++)
                {
                    Necessary += levelpack[i].NecessaryDeaths;
                }

                levelCount = levelpack.Count;
            }

            Stats.UpToText.Text = "Up to level " + (so.Get("levelProgress").int_val + 1);
            if (levelCount != 0) Stats.UpToText.Text += (" out of " + levelCount);

            Stats.DeathCount.Text = "Died " + so.Get("deathCount").int_val + " times";
            if (so.Get("deathCount").int_val != 0) Stats.NecessaryDeathCount.Text = "With " + (so.Get("deathCount").int_val - Necessary) + " being unnecessary";

            Stats.Timer.Text = "Spent " + toHMS(so.Get("timer").int_val);

            Stats.WinTokensTotal.Text += "Got " + so.Get("coins").int_val + " Win Tokens (total)";

            Stats.WT = Stats.WTNew = so.Get("coins").int_val;

            for (var i = 0; i < so.Get("gotCoin").array_val.Length; i++)
            {
                if (so.Get("levelProgress").int_val > i)
                {
                    if (so.Get("gotCoin").array_val[i])
                    {
                        Stats.WinTokensIndividual.Text += "Level " + (i + 1) + ": Yes\n";
                    }
                    else
                    {
                        Stats.WinTokensIndividual.Text += "Level " + (i + 1) + ": No\n";
                    }
                }
                else
                {
                    Stats.WinTokensIndividual.Text += "Level " + (i + 1) + ": Not played\n";
                }
            }

            Stats.WinTokensIndividual.Height = 16 * so.Get("gotCoin").array_val.Length;
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
                ViewButton.Visibility = Visibility.Hidden;
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
                ViewButton.Visibility = Visibility.Visible;
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

        public string toHMS(int i)
        {
            var Hours = Math.Floor((double)i/3600000).ToString();

            var Minutes = (Math.Floor((double)i/60000) % 60).ToString();
            if (Convert.ToInt32(Minutes) < 10)
            {
                Minutes = "0" + Minutes;
            }

            var Seconds = (Math.Floor((double)i/1000) % 60).ToString();
            if (Convert.ToInt32(Seconds) < 10)
            {
                Seconds = "0" + Seconds;
            }

            var Milliseconds = (Math.Floor((double)i/100) % 10).ToString();

            return Hours + ":" + Minutes + ":" + Seconds + "." + Milliseconds;
        }

        public string GetLevelpack()
        {
            if (File.Exists(Path.Combine(Path.GetDirectoryName(SWFPath), "levels.txt")))
            {
                return Path.Combine(Path.GetDirectoryName(SWFPath), "levels.txt");
            }
            else
            {
                OpenFileDialog OpenFile = new OpenFileDialog();

                OpenFile.FileName = "levels.txt";
                OpenFile.Filter = "Text file (*.txt)|*.txt";
                OpenFile.Title = "Select Levelpack";

                if (OpenFile.ShowDialog() == true)
                {
                    File.Copy(OpenFile.FileName, Path.Combine(Path.GetDirectoryName(SWFPath), "levels.txt"));

                    return Path.Combine(Path.GetDirectoryName(SWFPath), "levels.txt");
                }
                else
                {
                    MessageBox.Show("A levelpack was not found to extract necessary information from. Defaulting to the vanilla levelpack.");

                    if (File.Exists(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "5bsl", "levels.txt"))) return Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "5bsl", "levels.txt");

                    using (var client = new WebClient())
                    {
                        try
                        {
                            client.DownloadFile("http://battlefordreamisland.com/5b/levels.txt", Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "5bsl", "levels.txt"));
                        }
                        catch (WebException)
                        {
                            MessageBox.Show("You are not connected to the internet, so the default levelpack could not be downloaded.");
                            File.Delete(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "5bsl", "levels.txt"));
                            return null;
                        }

                        return Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "5bsl", "levels.txt");
                    }
                }
            }
        }
    }
}
