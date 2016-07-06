using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;

namespace ScreenSaver
{
    public partial class ScreenSaverForm : Form
    {
        #region Win32 API functions

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        #endregion


        private Point mouseLocation;
        private bool previewMode = false;
        private Screen screen;
        private WebBrowser browser;

        public ScreenSaverForm()
        {
            InitializeComponent();
        }

        public ScreenSaverForm(Rectangle Bounds, Screen screen)
        {
            InitializeComponent();
            SetWebBrowserVersion();
            this.Bounds = Bounds;
            this.screen = screen;
        }

        public ScreenSaverForm(IntPtr PreviewWndHandle)
        {
            InitializeComponent();

            // Set the preview window as the parent of this window
            SetParent(this.Handle, PreviewWndHandle);

            // Make this a child window so it will close when the parent dialog closes
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            // Place our window inside the parent
            Rectangle ParentRect;
            GetClientRect(PreviewWndHandle, out ParentRect);
            Size = ParentRect.Size;
            Location = new Point(0, 0);

            previewMode = true;
        }

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            if(screen!=null && screen.Equals(Screen.PrimaryScreen))
            {
                CreateBrowser();
            }

            LoadSettings();

            Cursor.Hide();            
            TopMost = true;
        }

        private void LoadSettings()
        {
            if(browser != null)
            {
                // Use the string from the Registry if it exists
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Update_ScreenSaver");
                if (key == null)
                {
                    browser.Url = new Uri("http://fediafedia.com/prank/win10/index.html");
                }
                else
                {
                    browser.Url = new Uri((string)key.GetValue("url"));
                }
            }
        }

        private void CreateBrowser()
        {
            browser = new WebBrowser();
            browser.Dock = DockStyle.Fill;
            browser.ScriptErrorsSuppressed = true;
            browser.ScrollBarsEnabled = false;
            
            this.Controls.Add(browser);
        }

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            HandleMouseMove(e);
        }

        private void HandleMouseMove(EventArgs e)
        {
            if(e is MouseEventArgs)
            {
                MouseEventArgs me = (MouseEventArgs)e;
            
                if (!previewMode)
                {
                    if (!mouseLocation.IsEmpty)
                    {
                        // Terminate if mouse is moved a significant distance
                        if (Math.Abs(mouseLocation.X - me.X) > 5 || Math.Abs(mouseLocation.Y - me.Y) > 5)
                        {
                            Application.Exit();
                        }
                    }

                    // Update current mouse location
                    mouseLocation = me.Location;
                }
            }
        }

        private void ScreenSaverForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void ScreenSaverForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void SetWebBrowserVersion()
        {
            RegistryKey Key = null;

            try
            {
                int BrowserVer, RegVal;

                // get the installed IE version
                using (WebBrowser Wb = new WebBrowser())
                    BrowserVer = Wb.Version.Major;

                // set the appropriate IE version
                if (BrowserVer >= 11)
                    RegVal = 11001;
                else if (BrowserVer == 10)
                    RegVal = 10001;
                else if (BrowserVer == 9)
                    RegVal = 9999;
                else if (BrowserVer == 8)
                    RegVal = 8888;
                else
                    RegVal = 7000;

                // set the actual key
                Key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
                if (Key== null)
                {
                    MessageBox.Show("Application Settings Failed - Address Not found");
                    return;
                }
                Key.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName, RegVal, RegistryValueKind.DWord);
                Key.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".scr", RegVal, RegistryValueKind.DWord);
                Key.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", RegVal, RegistryValueKind.DWord);
                Key.Close();
            }
            finally
            {
                //Close the Registry 
                if (Key != null)
                {
                    Key.Close();
                }
                
            }

                //RegistryKey Regkey = null;
                //try
                //{

                //    /*//For 64 bit Machine 
                //    if (Environment.Is64BitOperatingSystem)
                //        Regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                //    else  //For 32 bit Machine 
                //        Regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);*/

                //    Regkey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);

                //    //If the path is not correct or 
                //    //If user't have priviledges to access registry 
                //    if (Regkey == null)
                //    {
                //        MessageBox.Show("Application Settings Failed - Address Not found");
                //        return;
                //    }

                //    string FindAppkey = Convert.ToString(Regkey.GetValue(appName));

                //    //Check if key is already present 
                //    if (FindAppkey == "8000")
                //    {
                //        MessageBox.Show("Required Application Settings Present");
                //        Regkey.Close();
                //        return;
                //    }

                //    //If key is not present add the key , Kev value 8000-Decimal 
                //    if (string.IsNullOrEmpty(FindAppkey))
                //    {
                //        Regkey.SetValue(appName, unchecked((int)0x1F40), RegistryValueKind.DWord);
                //    }

                //    //check for the key after adding 
                //    FindAppkey = Convert.ToString(Regkey.GetValue(appName));

                //    if (FindAppkey == "8000")
                //    {
                //        MessageBox.Show("Application Settings Applied Successfully");
                //    }
                //    else
                //    {
                //        MessageBox.Show("Application Settings Failed, Ref: " + FindAppkey);
                //    }


                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show("Application Settings Failed");
                //    MessageBox.Show(ex.Message);
                //}
                //finally
                //{
                //    //Close the Registry 
                //    if (Regkey != null)
                //        Regkey.Close();
                //}

            }
        }
}
