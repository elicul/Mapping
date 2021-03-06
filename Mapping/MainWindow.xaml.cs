﻿using System;
using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.Data.SQLite;
using Ookii.Dialogs.Wpf;
using System.Collections.Generic;

namespace Mapping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        static string conString = @"Data Source=Resources\Cars.db;Version=3;FailIfMissing=True; Foreign Keys=True;";
        public string chkBox = "";
        public List<string> chkBoxL = new List<string>();
        public string sPath = @"C:\Users\Enzo\Google Drive\Mapping";

        OpenFileDialog ofdORI = new OpenFileDialog();
        OpenFileDialog ofdMOD = new OpenFileDialog();
        OpenFileDialog ofdKP = new OpenFileDialog();

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                NadjiMarcu();
                Init();
                txtLoc.Text = sPath;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Init()
        {
            int god = 1997;
            for (int i = 0; i < 19; i++)
            {
                god += 1;
                comboGod.Items.Add(god.ToString());
            }

            int hp = 60;
            for (int i = 0; i < 40; i++)
            {
                hp += 5;
                comboHP.Items.Add(hp.ToString());
            }
        }

        private void NadjiMarcu()
        {
            using (SQLiteConnection conn = new SQLiteConnection(conString))
            {
                conn.Open();
                string sql = "SELECT DISTINCT Marca FROM CarList";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboMarca.Items.Add(reader["Marca"].ToString());
                        }
                    }
                }
                conn.Close();
            }
        }

        private void NadjiModel(string Marca)
        {
            comboModelo.Items.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(conString))
            {
                conn.Open();
                string sql = "SELECT DISTINCT Modelli FROM CarList WHERE Marca LIKE '" + Marca + "'";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboModelo.Items.Add(reader["Modelli"].ToString());
                        }
                    }
                }
                conn.Close();
            }
        }

        private void NadjiMotor(string Model)
        {
            comboMotor.Items.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(conString))
            {
                conn.Open();
                string sql = "SELECT DISTINCT Engine FROM CarList WHERE Modelli LIKE '" + Model + "'";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboMotor.Items.Add(reader["Engine"].ToString());
                        }
                    }
                }
                conn.Close();
            }
        }

        private void NadjiECU(string Motor)
        {
            comboECU.Items.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(conString))
            {
                conn.Open();
                string sql = "SELECT DISTINCT ECU,Type FROM CarList WHERE Engine LIKE '" + Motor + "'";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboECU.Items.Add(reader["ECU"].ToString() + reader["Type"].ToString());
                        }
                    }
                }
                conn.Close();
            }
        }

        private void comboMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboMarca.Text))
            {
                NadjiModel(comboMarca.Text);
            }
        }

        private void comboModelo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboModelo.Text))
            {
                NadjiMotor(comboModelo.Text);
            }
        }

        private void comboMotor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboMarca.Text))
            {
                NadjiECU(comboMotor.Text);
            }
        }

        private void btnORI_Click(object sender, EventArgs e)
        {
            ofdORI.Filter = "ORI|*.ori";
            Nullable<bool> result = ofdORI.ShowDialog();

            if (result == true)
            {
                txtORI.Text = ofdORI.FileName;

            }
        }

        private void btnMOD_Click(object sender, EventArgs e)
        {
            ofdMOD.Filter = "MOD|*.mod";
            Nullable<bool> result = ofdMOD.ShowDialog();

            if (result == true)
            {
                txtMOD.Text = ofdMOD.FileName;

            }
        }

        private void btnKP_Click(object sender, EventArgs e)
        {
            ofdKP.Filter = "KP|*.kp";
            Nullable<bool> result = ofdKP.ShowDialog();

            if (result == true)
            {
                txtKP.Text = ofdKP.FileName;

            }
        }

        private void btnLoc_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog sFolder = new VistaFolderBrowserDialog();

            Nullable<bool> result = sFolder.ShowDialog();

            if (result == true)
            {
                sPath = sFolder.SelectedPath;
                txtLoc.Text = sPath;
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (var item in chkBoxL)
            {
                chkBox += item + " ";
            }
            string[] text = { "Make: " + comboMarca.Text, "Model: " + comboModelo.Text, "Engine: " + comboMotor.Text, "ECU: " + comboECU.Text, "SW: " + txtSW.Text, "HW: " + txtHW.Text, "HP " + comboHP.Text, "Production year: " + comboGod.Text, "Modification info: " + chkBox + txtOpis.Text };
            string time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            time = time.Replace(':', '.');
            string path = sPath + @"\" + comboMarca.Text.Replace('\\', '_') + "\\" + comboModelo.Text.Replace('\\', '_') + "\\" + txtVlasnik.Text + "\\" + time;
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string fileNameORI = comboMarca.Text.Replace('/', '_') + "_" + comboModelo.Text.Replace('/', '_') + "_" + comboECU.Text.Replace('/', '_') + "_" + comboHP.Text + "_" + time + ".ORI";
            string fileNameMOD = comboMarca.Text.Replace('/', '_') + "_" + comboModelo.Text.Replace('/', '_') + "_" + comboECU.Text.Replace('/', '_') + "_" + comboHP.Text + "_" + time + ".MOD";
            string fileNameKP = comboMarca.Text.Replace('/', '_') + "_" + comboModelo.Text.Replace('/', '_') + "_" + comboECU.Text.Replace('/', '_') + "_" + comboHP.Text + "_" + time + ".KP";
            string destFileORI = System.IO.Path.Combine(path, fileNameORI);
            string destFileMOD = System.IO.Path.Combine(path, fileNameMOD);
            string destFileKP = System.IO.Path.Combine(path, fileNameKP);
            try
            {
                System.IO.File.Copy(ofdORI.FileName, destFileORI, true);
                System.IO.File.Copy(ofdMOD.FileName, destFileMOD, true);
                System.IO.File.Copy(ofdKP.FileName, destFileKP, true);
                System.IO.File.WriteAllLines(path + "\\Info_" + time + ".txt", text);
                chkBox = "";
                chkBoxL.Clear();
                chkDPF.IsChecked = false;
                chkDTC.IsChecked = false;
                chkEGR.IsChecked = false;
                chkHS.IsChecked = false;
                chkS1.IsChecked = false;
                chkS2.IsChecked = false;
                chkS3.IsChecked = false;
                chkSS.IsChecked = false;
                MessageBox.Show("File saved successfully!");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void chkS1_Checked(object sender, RoutedEventArgs e)
        {
            if (chkS1.IsChecked == true)
            {
                chkBoxL.Add("Stage 1");
            }
            else
            {
                chkBoxL.Remove("Stage 1");
            }
        }

        private void chkS2_Checked(object sender, RoutedEventArgs e)
        {
            if (chkS2.IsChecked == true)
            {
                chkBoxL.Add("Stage 2");
            }
            else
            {
                chkBoxL.Remove("Stage 2");
            }
        }

        private void chkS3_Checked(object sender, RoutedEventArgs e)
        {
            if (chkS3.IsChecked == true)
            {
                chkBoxL.Add("Stage 3");
            }
            else
            {
                chkBoxL.Remove("Stage 3");
            }
        }

        private void chkEGR_Checked(object sender, RoutedEventArgs e)
        {
            if (chkEGR.IsChecked == true)
            {
                chkBoxL.Add("EGR OFF");
            }
            else
            {
                chkBoxL.Remove("EGR OFF");
            }
        }

        private void chkDPF_Checked(object sender, RoutedEventArgs e)
        {
            if (chkDPF.IsChecked == true)
            {
                chkBoxL.Add("DPF OFF");
            }
            else
            {
                chkBoxL.Remove("DPF OFF");
            }
        }

        private void chkHS_Checked(object sender, RoutedEventArgs e)
        {
            if (chkHS.IsChecked == true)
            {
                chkBoxL.Add("Hot Start");
            }
            else
            {
                chkBoxL.Remove("Hot Start");
            }
        }

        private void chkSS_Checked(object sender, RoutedEventArgs e)
        {
            if (chkSS.IsChecked == true)
            {
                chkBoxL.Add("Start and Stop OFF");
            }
            else
            {
                chkBoxL.Remove("Start and Stop OFF");
            }
        }

        private void chkDTC_Checked(object sender, RoutedEventArgs e)
        {
            if (chkDTC.IsChecked == true)
            {
                chkBoxL.Add("DTC OFF");
            }
            else
            {
                chkBoxL.Remove("DTC OFF");
            }
        }

        private void chkVlasnik_Checked(object sender, RoutedEventArgs e)
        {
            if (chkVlasnik.IsChecked == true)
            {
                txtVlasnik.Text = "Nepoznati";
                txtVlasnik.IsEnabled = false;
            }
            else
            {
                txtVlasnik.IsEnabled = true;
            }
        }


    }
}
