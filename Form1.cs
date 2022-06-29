using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ToML
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string version = "";
        public string serverVersion = "";
        public string path = @"Data\";
        public string exename = "ToM_Scripts.VDF";
        public string gmp = @"GMP_Client\";

        public string url = "https://drive.google.com/uc?export=download&confirm=no_antivirus&id=1YOs1G71gk0nZVnPzlBznfhEbZGWAmNre";
        public string url2 = "https://drive.google.com/uc?export=download&confirm=no_antivirus&id=1k6TGtaRLHnI4gL4MkS-zRLgY1lyhLCvM";
        public bool downloading = false;

        PrivateFontCollection pfc = new PrivateFontCollection();
        public void InitFont()
        {
            pfc.AddFontFile("Red October.ttf");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitFont();
            label1.Font = new Font(pfc.Families[0], 14, FontStyle.Regular);
            label2.Font = new Font(pfc.Families[0], 8, FontStyle.Regular);
            label3.Font = new Font(pfc.Families[0], 8, FontStyle.Regular);
            label1.BringToFront();
            button1.Font = new Font(pfc.Families[0], label1.Font.Size);




            Directory.CreateDirectory(Path.GetFullPath(path));
            if (!File.Exists(Path.GetFullPath(path + exename)))
            {
                RegistryKey key;
                key = Registry.CurrentUser.CreateSubKey("Shooter");
                key.SetValue("ver", "0");
                key.Close();
                version = "0";
            }
            else
            {
                version = (string)Registry.CurrentUser.OpenSubKey("Shooter").GetValue("ver");
            }


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream recivestream = response.GetResponseStream();
                StreamReader readstream = null;
                if (response.CharacterSet == null)
                {
                    readstream = new StreamReader(recivestream);
                }
                else
                {
                    readstream = new StreamReader(recivestream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readstream.ReadToEnd();
                serverVersion = data;
                response.Close();
                readstream.Close();
            }
            else
            {
                MessageBox.Show("Error");
            }

            label1.Text = "ver: " + version;
            if (!downloading)
            {
                if (version != serverVersion)
                {
                    if (File.Exists(Path.GetFullPath(path + exename)))
                    {
                        button1.Text = "Обновить";
                        button1.Click += UpdateGame;
                    }
                    else
                    {
                        button1.Text = "Скачать";
                        button1.Click += UpdateGame;
                    }
                }
                else
                {
                    button1.Text = "Играть";
                    button1.Click += Play;
                }
            }
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            int step = 0;//шаг
            try
            {
                WebClient webClient = new WebClient();
                this.progressBar1.Step = step;
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
                webClient.DownloadProgressChanged += (s, a) =>
                {
                    label3.Text = $"Загружено: {a.ProgressPercentage}% ({a.BytesReceived / 1048576}) МБ";
                    progressBar1.Value = a.ProgressPercentage;
                };
                webClient.DownloadFileAsync(new Uri(url2), "gg.zip");
                progressBar1.Show();
                File.Delete(@"Data\ToM_Anims.VDF");
                File.Delete(@"Data\ToM_Meshes.VDF");
                File.Delete(@"Data\ToM_Scripts.VDF");
                File.Delete(@"Data\ToM_Sounds.VDF");
                File.Delete(@"Data\ToM_Textures.VDF");
                File.Delete(@"Data\ToM_Worlds.VDF");
                webClient.DownloadProgressChanged += (s, g) =>
                {
                    if (g.ProgressPercentage == 100)
                    {
                        if (!File.Exists(Path.GetFullPath(path + exename)))
                        {
                            ZipFile.ExtractToDirectory("gg.zip", path);
                            RegistryKey key;
                            File.Delete("gg.zip");
                            key = Registry.CurrentUser.CreateSubKey("Shooter");
                            key.SetValue("ver", serverVersion);
                            key.Close();
                            version = serverVersion;
                            label1.Text = "ver: " + version;
                            button1.Text = "Играть";
                            button1.Click -= UpdateGame;
                            button1.Click += Play;
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not download file: " + ex.Message);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // MessageBox.Show(String.Format("{0} of {1} bytes downloaded ({2}% done)", e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage));
            if (this.progressBar1 != null)
                this.progressBar1.PerformStep();
        }
        void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
                label3.Text = "Файлы загружены";

            else
                MessageBox.Show("Could not download file: " + e.Error.Message);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/JhtZnueYnu");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("https://gothic2online.ru");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("https://vk.com/gothic_roleplay");
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Play(object sender, EventArgs e)
        {
            Application.Exit();
            Process.Start(gmp + "gml.exe");
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Show();
            textBox2.Show();
            label4.Show();
            label5.Show();
            label6.Show();
            label7.Show();
            label8.Show();
            label9.Show();
            trackBar1.Show();
            trackBar2.Show();
            checkBox1.Show();
            button7.Hide();
            button8.Show();
            button9.Show();
            pictureBox2.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox1.Hide();
            textBox2.Hide();
            label4.Hide();
            label5.Hide();
            label6.Hide();
            label7.Hide();
            label8.Hide();
            label9.Hide();
            trackBar1.Hide();
            trackBar2.Hide();
            checkBox1.Hide();
            button7.Show();
            button8.Hide();
            button9.Hide();
            pictureBox2.Hide();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number))
            {
                e.Handled = true;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {

            const string fileName = @"System\Gothic.ini";
            string[] lines = File.ReadAllLines(fileName);
            lines[24] = "sightValue=0." + trackBar2.Value;
            lines[149] = "zVidResFullscreenX=" + textBox1.Text;
            lines[150] = "zVidResFullscreenY=" + textBox2.Text;
            lines[154] = "zStartupWindowed=" + checkBox1.Text;
            lines[175] = "musicVolume=0." + trackBar1.Value;
            File.WriteAllLines(fileName, lines);
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.Text = "1";
            }
            else
            {
                checkBox1.Text = "0";
            }
        }
    }
}
