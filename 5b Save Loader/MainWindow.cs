using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace _5b_Save_Loader
{
    public partial class MainWindow : Form
    {
        string FilePath;

        public MainWindow()
        {
            InitializeComponent();

            step2.Hide();
            ToggleButtons(0);
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            step2.Hide();
            ToggleButtons(0);

            FilePath = null;

            OpenFile.FileName = "5b.swf";
            OpenFile.Filter = "Adobe Flash movie (*.swf)|*.swf";
            OpenFile.Title = "Upload Process (1/6)";

            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                FilePath = Environment.GetEnvironmentVariable("AppData") + "\\Macromedia\\Flash Player\\#SharedObjects\\VSAWG93S\\localhost\\" + OpenFile.FileName.Remove(0, 3);

                if (!File.Exists(Path.Combine(FilePath, "bfdia5b.sol")))
                {
                    MessageBox.Show("Please start level 1 for this to work!");
                    return;
                }

                step2.Show();
                ToggleButtons(1);

                RefreshSaves();
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (SavesList.SelectedIndex == -1) return;

            if (MessageBox.Show("Are you sure you want to replace your current save with this one?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string[] Saves = Directory.GetDirectories(FilePath);
                File.Copy(Path.Combine(Saves[SavesList.SelectedIndex], "bfdia5b.sol"), Path.Combine(FilePath, "bfdia5b.sol"), true);

                MessageBox.Show(Saves[SavesList.SelectedIndex] + " has been set as the current save!");
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            MakeBackup();

            SaveButton.Enabled = false;
            Thread.Sleep(1000);
            SaveButton.Enabled = true;

            RefreshSaves();
        }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            if (SavesList.SelectedIndex == -1) return;

            string[] Saves = Directory.GetDirectories(FilePath);

            var Save = Interaction.InputBox("Save File Name", "Save Game", GetTime(), 100, 100);
            if (Save == "") return;

            if (File.Exists(Path.Combine(FilePath, Save, "bfdia5b.sol")))
            {
                if (MessageBox.Show("A save by this name already exists. Do you want to overwrite it?", "Warning!", MessageBoxButtons.YesNo) == DialogResult.No)
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

            if (MessageBox.Show("Are you sure you want to PERMANENTLY delete this save?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            MessageBox.Show(Saves[SavesList.SelectedIndex].Split('\\')[Saves[SavesList.SelectedIndex].Split('\\').Length - 1] + " has been deleted!");

            Directory.Delete(Path.Combine(FilePath, Saves[SavesList.SelectedIndex]), true);

            ToggleButtons(2);

            RefreshSaves();
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            OpenFile.FileName = "bfdia5b.sol";
            OpenFile.Filter = "Shared Object Local Files (*.sol)|*.sol";
            OpenFile.Title = "Import Save";

            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                var Save = Interaction.InputBox("Save File Name", "Save Game", GetTime(), 100, 100);
                if (Save == "") return;

                if (File.Exists(Path.Combine(FilePath, Save, "bfdia5b.sol")))
                {
                    if (MessageBox.Show("A save by this name already exists. Do you want to overwrite it?", "Warning!", MessageBoxButtons.YesNo) == DialogResult.No)
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

                ImportButton.Enabled = false;
                Thread.Sleep(1000);
                ImportButton.Enabled = true;

                RefreshSaves();
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (SavesList.SelectedIndex == -1) return;

            string[] Saves = Directory.GetDirectories(FilePath);

            SaveFile.DefaultExt = "sol";
            SaveFile.FileName = "bfdia5b.sol";
            SaveFile.Filter = "Shared Object Local Files (*.sol)|*.sol";
            SaveFile.Title = "Export Save";

            SaveFile.ShowDialog();

            if (SaveFile.FileName == "") return;

            File.Copy(Path.Combine(Saves[SavesList.SelectedIndex], "bfdia5b.sol"), SaveFile.FileName, true);
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
                if (MessageBox.Show("A save by this name already exists. Do you want to overwrite it?", "Warning!", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            } else
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

        private void SavesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SavesList.SelectedIndex == -1) return;

            ToggleButtons(3);

            string[] Saves = Directory.GetDirectories(FilePath);
            var Description = File.ReadAllText(Path.Combine(Saves[SavesList.SelectedIndex], "desc.txt"));
            
            DescriptionText.Text = File.ReadAllText(Path.Combine(Saves[SavesList.SelectedIndex], "desc.txt"));
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
                LoadButton.Hide();
                SaveButton.Hide();
                SavesList.Hide();
                DescriptionText.Hide();
                LoadButton.Hide();
                RenameButton.Hide();
                DeleteButton.Hide();
                ImportButton.Hide();
                ExportButton.Hide();
            }

            if (Reason == 1) // File chosen
            {
                LoadButton.Show();
                SaveButton.Show();
                SavesList.Show();
                DescriptionText.Show();
                LoadButton.Show();
                RenameButton.Show();
                DeleteButton.Show();
                ImportButton.Show();
                ExportButton.Show();
            }

            if (Reason == 2) // Nothing selected
            {
                LoadButton.Enabled = false;
                RenameButton.Enabled = false;
                DeleteButton.Enabled = false;
                ExportButton.Enabled = false;
                DescriptionText.Enabled = false;
            }

            if (Reason == 3) // Something selected
            {
                LoadButton.Enabled = true;
                RenameButton.Enabled = true;
                DeleteButton.Enabled = true;
                ExportButton.Enabled = true;
                DescriptionText.Enabled = true;
            }
        }
    }
}
