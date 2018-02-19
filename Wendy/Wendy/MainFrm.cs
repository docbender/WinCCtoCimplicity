using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Wendy
{
    public partial class MainFrm : Form, INotifyPropertyChanged
    {
        const int WM_CLIPBOARDUPDATE = 0x031D;
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool AddClipboardFormatListener(IntPtr hWndNewViewer);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool RemoveClipboardFormatListener(IntPtr hwnd);


        public MainFrm()
        {
            InitializeComponent();
        }

        bool _ontop = true;
        bool _autocopy = false;
        bool _valid = false;

        bool ClipBoardMonitoring { get; set; } = true;

        public bool Ontop
        {
            get
            {
                return _ontop;
            }
            set
            {

                _ontop = value;
                TopMost = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OntopImageIndex"));
            }
        }

        public bool AutoCopy
        {
            get
            {
                return _autocopy;
            }
            set
            {

                _autocopy = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("AutoCopyImageIndex"));
            }
        }

        public int OntopImageIndex
        {
            get
            {
                return _ontop ? 0 : 1;
            }
        }

        public int AutoCopyImageIndex
        {
            get
            {
                return _autocopy ? 2 : 3;
            }
        }

        bool Valid
        {
            get
            {
                return _valid;
            }
            set
            {
                if (_valid == value)
                    return;

                _valid = value;

                if (value)
                    toolStripStatusLabel2.Image = Properties.Resources.tick_circle_icon;
                else
                    toolStripStatusLabel2.Image = Properties.Resources.Button_stop_icon;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void MainFrm_Load(object sender, EventArgs e)
        {
            if (System.Environment.OSVersion.Version.Major < 6)
            {
                ClipBoardMonitoring = false;
                btnAutoCopy.Enabled = false;
                toolStripStatusLabel1.Text = $"Na OS starší než Vista není podporován monitoring schránky :-(";
            }
            else
            {
                if (!AddClipboardFormatListener(this.Handle))
                {
                    ClipBoardMonitoring = false;
                    btnAutoCopy.Enabled = false;
                    toolStripStatusLabel1.Text = $"Neporařilo se zaregistrovat monitoring schránky :-(";
                }
            }

            btnOntop.DataBindings.Add("ImageIndex", this, "OntopImageIndex");
            btnAutoCopy.DataBindings.Add("ImageIndex", this, "AutoCopyImageIndex");

            AutoCopy = Properties.Settings.Default.AutoCopy;
            Ontop = Properties.Settings.Default.OnTop;
            if (Properties.Settings.Default.Width >= this.MinimumSize.Width)
                this.Width = Properties.Settings.Default.Width;
            if (Properties.Settings.Default.Height >= this.MinimumSize.Height)
                this.Height = Properties.Settings.Default.Height;
            foreach (var sc in System.Windows.Forms.Screen.AllScreens)
            {
                if (Properties.Settings.Default.X >= sc.WorkingArea.Left && Properties.Settings.Default.X < sc.WorkingArea.Left + sc.WorkingArea.Width
                    && Properties.Settings.Default.Y >= sc.WorkingArea.Top && Properties.Settings.Default.Y < sc.WorkingArea.Top + sc.WorkingArea.Height
                    && !(Properties.Settings.Default.X == -1 && Properties.Settings.Default.X == -1))
                {
                    this.Left = Properties.Settings.Default.X;
                    this.Top = Properties.Settings.Default.Y;
                    break;
                }
            }
        }
        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ClipBoardMonitoring)
                RemoveClipboardFormatListener(this.Handle);

            Properties.Settings.Default.AutoCopy = AutoCopy;
            Properties.Settings.Default.OnTop = Ontop;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.Height = this.Height;
            Properties.Settings.Default.X = this.Left;
            Properties.Settings.Default.Y = this.Top;
            Properties.Settings.Default.Save();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                IDataObject iData = Clipboard.GetDataObject();

                if (iData.GetDataPresent(DataFormats.Text))
                {
                    string text = (string)iData.GetData(DataFormats.Text);

                    if (text.Equals(tbTranslation.Text))
                        return;

                    toolStripStatusLabel1.Text = "";
                    tbTranslation.Text = "";
                    if (text.Length > 10000)
                    {
                        toolStripStatusLabel1.Text = "Text ve schránce je příliš dlouhý. Zkrácen na 10000 znaků.";
                        tbOrigin.Text = text.Substring(0, 10000);
                    }
                    else
                        tbOrigin.Text = text;
                }
            }
        }

        private void btnOntop_Click(object sender, EventArgs e)
        {
            Ontop = !Ontop;
        }

        private void btnAutoCopy_Click(object sender, EventArgs e)
        {
            AutoCopy = !AutoCopy;            
        }
        private void tbOrigin_TextChanged(object sender, EventArgs e)
        {
            if (Valid = WinCCConvertor.Validate(tbOrigin.Text))
            {
                var translation = WinCCConvertor.Translate(tbOrigin.Text);
                tbTranslation.Text = translation;

                if (AutoCopy && translation.Length > 0)
                    try
                    {
                        Clipboard.SetText(translation);
                    }
                    catch
                    {
                        toolStripStatusLabel1.Text = "Ups :-(. Problém s uložením do schránky. Snad příště :-)";
                    }
            }
        }
    }
}

