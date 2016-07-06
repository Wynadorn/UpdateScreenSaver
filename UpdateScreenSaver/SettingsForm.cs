using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Permissions;

namespace ScreenSaver
{
    public partial class SettingsForm : Form
    {
        Dictionary<string, Uri> urls= new Dictionary<string, Uri>()
        {
            {"Windows 98", new Uri("http://fediafedia.com/prank/windows98/index.html")},
            {"Windows XP", new Uri("http://fediafedia.com/prank/xp/index.html")},
            {"Windows XP - BSOD", new Uri("http://fediafedia.com/prank/xp/bsod.html")},
            {"Windows Vista", new Uri("http://fediafedia.com/prank/vista/index.html")},
            {"Windows Vista - BSOD", new Uri("http://fediafedia.com/prank/vista/bsod.html")},
            {"Windows 7", new Uri("http://fediafedia.com/prank/win7/index.html")},
            {"Windows 8", new Uri("http://fediafedia.com/prank/win8/index.html")},
            {"Windows 8 - BSOD", new Uri("http://fediafedia.com/prank/win8/bsod.html")},
            {"Windows 10", new Uri("http://fediafedia.com/prank/win10/index.html")},
            {"Windows 10 - BSOD", new Uri("http://fediafedia.com/prank/win10/bsod.html")},
            {"OSX", new Uri("http://fediafedia.com/prank/apple/index.html")},
            {"Steam OS", new Uri("http://fediafedia.com/prank/steam/index.html")},
            {"Ubuntu", new Uri("http://fediafedia.com/prank/ubuntu/index.html")}
        };
    

    public SettingsForm()
        {
            InitializeComponent();
            FillComboBox();
            LoadSettings();
        }

        private void FillComboBox()
        {
            foreach(string option in urls.Keys)
            {
                comboBox1.Items.Add(option);
            }
        }

        /// <summary>
        /// Load display text from the Registry
        /// </summary>
        private void LoadSettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Update_ScreenSaver");
            if (key == null)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                comboBox1.Text = urls.FirstOrDefault(x => x.Value == new Uri((string)key.GetValue("url"))).Key;
            }
        }

        /// <summary>
        /// Save text into the Registry.
        /// </summary>
        private void SaveSettings()
        {
            // Create or get existing subkey
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Update_ScreenSaver");

            if(urls.ContainsKey(comboBox1.Text))
            {
                key.SetValue("url", urls[comboBox1.Text]);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
