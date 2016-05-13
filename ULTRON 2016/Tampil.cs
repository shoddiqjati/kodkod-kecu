using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.IO;

namespace ULTRON_2016
{
    class Tampil
    {

        private List<string> listString;
        private List<string> tempList = new List<string>();
        private List<KoleksiData.Sensor> mylist;

        public double accx = 0;
        public double accy = 0;
        public double accz = 0;
        public double tinggi = 0;
        public double suhu = 0;
        public double tekanan = 0;
        public double gyrox = 0;
        public double gyroy = 0;
        public double gyroz = 0;
        public string longitude = "0";
        public string latitude = "0";

        /*public void masukTerminal(string datanya, ref System.Windows.Controls.TextBox textInputan)
        {
            textInputan.AppendText(datanya);
            textInputan.SelectionStart = textInputan.Text.Length;
            textInputan.CaretIndex = textInputan.Text.Length;
            textInputan.ScrollToEnd();           
        }*/

        /*public void GrafikSensor(int b)
        {

        }*/

        private string isiDatabase;
        private string isiLogging;
        private bool statusDB;
        public double konterDB;
        private bool statusLog = true;
        private int konterLog = 0;

        public void SetDB()
        {
            statusDB = true;
            konterDB = 0;
        }

        public void UnsetDB()
        {
            statusDB = false;
        }

        private string konvertKalimatLog(string kalimat)
        {
            kalimat = kalimat.Replace(" ", " | ");
            kalimat = kalimat.Replace("MAESTRO", " ");
            kalimat = konterLog.ToString() + kalimat;
            return kalimat;
        }

        private string konvertKalimat(string kalimat)
        {
            kalimat = kalimat.Replace(" ", " | ");
            kalimat = kalimat.Replace("MAESTRO", " ");
            kalimat = konterDB.ToString() + kalimat;
            return kalimat;
        }

        public void convertDatatabelToCSV()
        {
            string contents;
            string tabelDB = Directory.GetCurrentDirectory() + "/Simpan/datatabel.dbgcs";
            string tabelDBCSV = Directory.GetCurrentDirectory() + "/Simpan/datatabel.dbgcs.csv";
            try
            {
                using (StreamReader reader = File.OpenText(tabelDB))
                {
                    contents = reader.ReadLine();
                    contents += "\n";
                    contents += reader.ReadToEnd();
                }
                contents.ToString().Replace("|", ",");

                FileStream fs = null;
                using (fs = File.Create(tabelDBCSV))
                {

                }

                using (StreamWriter writer = File.AppendText(tabelDBCSV))
                {
                    writer.WriteLine(contents);
                }

            }
            catch (Exception)
            {
            }

        }

        public void convertDatasekonToCSV()
        {
            try
            {
                string tabelDB = Directory.GetCurrentDirectory() + "/Simpan/datasekon.dbgcs";
                string tabelDBCSV = Directory.GetCurrentDirectory() + "/Simpan/datasekon.dbgcs.csv";
                string contents;
                using (StreamReader reader = File.OpenText(tabelDB))
                {
                    contents = reader.ReadLine();
                    contents += "\n";
                    contents += reader.ReadToEnd();
                }
                contents.ToString().Replace(" | ", ",");

                FileStream fs = null;
                using (fs = File.Create(tabelDBCSV))
                {

                }

                using (StreamWriter writer = File.AppendText(tabelDBCSV))
                {
                    writer.WriteLine(contents);
                }

            }
            catch (Exception)
            {
            }

        }

        public void loggingSerial(string isinya)
        {
            string tabelDB = Directory.GetCurrentDirectory() + "/Simpan/datatabel.dbgcs";
            string headerFile = "Data ke-  | Yaw | Pitch | Roll | Ketinggian | Suhu | Tekanan | Elevasi | Longitude | Latitude";
            isiLogging = konvertKalimatLog(isinya);// +System.Environment.NewLine;
            if (statusLog)
            {
                FileStream fs = null;
                using (fs = File.Create(tabelDB))
                {

                }

                using (StreamWriter writer = File.AppendText(tabelDB))
                {
                    writer.WriteLine(headerFile);
                    writer.WriteLine(isiLogging);

                }
                statusLog = false;
            }
            else
            {
                if (File.Exists(tabelDB))
                {
                    using (StreamWriter writer = File.AppendText(tabelDB)) //File.AppendText(tabelDB)
                    {
                        writer.WriteLine(isiLogging);
                    }
                }
            }
            konterLog++;

        }

        public void saveKeDatabase(string isinya)
        {
            string tabelDB = Directory.GetCurrentDirectory() + "/Simpan/datasekon.dbgcs";
            string headerFile = "Detik  | Yaw | Pitch | Roll | Ketinggian | Suhu | Tekanan | Elevasi | Longitude | Latitude";
            isiDatabase = konvertKalimat(isinya);// +System.Environment.NewLine;
            if (statusDB)
            {
                FileStream fs = null;
                using (fs = File.Create(tabelDB))
                {

                }

                using (StreamWriter writer = File.AppendText(tabelDB))
                {
                    writer.WriteLine(headerFile);
                    writer.WriteLine(isiDatabase);

                }
                UnsetDB();
            }
            else
            {
                if (File.Exists(tabelDB))
                {
                    using (StreamWriter writer = File.AppendText(tabelDB)) //File.AppendText(tabelDB)
                    {
                        writer.WriteLine(isiDatabase);
                    }
                }
            }

        }

        public void set(List<string> listString, List<KoleksiData.Sensor> mylist)
        {
            this.mylist = mylist;
            this.listString = listString;
        }

    }
}
