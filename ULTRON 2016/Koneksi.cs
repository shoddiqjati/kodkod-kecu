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
using System.Windows.Shapes;
using System.IO.Ports;

namespace ULTRON_2016
{
    class Koneksi
    {
        public class SerialDataEventArgs : EventArgs
        {
            public SerialDataEventArgs(string dataInByteArray)
            {
                Data = dataInByteArray;
            }

            /// <summary>
            /// Byte array containing data from serial port
            /// </summary>
            public string Data;
        }

        public Koneksi()
        {
            komSerial.DataReceived += new SerialDataReceivedEventHandler(komSerial_DataReceived);
            //serialLaunch.DataReceived += new SerialDataReceivedEventHandler(serialLaunch_DataReceived);
        }

        public EventHandler<SerialDataEventArgs> NewSerialDataReceived;

        public SerialPort komSerial = new SerialPort();
        //public SerialPort serialLaunch = new SerialPort();
        //private Tampil tampil = new Tampil();

        private void komInit()
        {
            komSerial.PortName = Komunikasi.Default.PortName;
            komSerial.BaudRate = Komunikasi.Default.BaudRate;
            komSerial.Parity = Komunikasi.Default.Parity;
            komSerial.DataBits = Komunikasi.Default.DataBits;
            komSerial.StopBits = Komunikasi.Default.StopBits;
            komSerial.Handshake = Komunikasi.Default.Handshake;
            komSerial.ReadTimeout = 100;
            komSerial.WriteTimeout = 8;
        }

        //private void launchInit()
        //{
        //    serialLaunch.PortName = Komunikasi.Default.Launcher;
        //}

        public bool statusKoneksi()
        {
            //if (komSerial.IsOpen)
            //    return true;
            //else
            //    return false;
            return komSerial.IsOpen; //&& serialLaunch.IsOpen;
        }

        public void buka()
        {
            if (!komSerial.IsOpen)
            {
                try
                {
                    komInit();
                    //launchInit();
                    komSerial.Open();
                    //serialLaunch.Open();
                    Komunikasi.Default.terkoneksi = true;

                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("empty"))
                    {
                        MessageBox.Show("Mohon dipastikan semua pengaturan telah diisi.", "Kesalahan");
                    }
                    else if (ex.Message.Contains("used"))
                    {
                        MessageBox.Show("Port " + Komunikasi.Default.PortName + " sedang dipakai aplikasi yang lain, mohon dicek kembali.", "Kesalahan");
                    }
                    else if (ex.Message.Contains("use"))
                    {
                        MessageBox.Show("Resource " + Komunikasi.Default.PortName + " sedang dipakai, mohon dicek kembali.", "Kesalahan");
                    }
                    else
                    {
                        MessageBox.Show(ex.Message + "\r\r(Mohon dicek kembali dan diulangi.)");
                    }
                    Komunikasi.Default.terkoneksi = false;
                }
            }
            else
            {
                Komunikasi.Default.terkoneksi = false;
                komSerial.Close();
                //serialLaunch.Close();
            }
        }

        //public void tulisLauncher(string textLaunch)
        //{
        //    try
        //    {
        //        serialLaunch.Write(textLaunch);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error");
        //    }
        //}

        public void tulis(string text)
        {
            try
            {
                komSerial.Write(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        void komSerial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {                       
            try
            {
            string hasilSerial = komSerial.ReadLine();
                lemparKeEvent(hasilSerial);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message.ToString());
            }
            
        }

        //private void serialLaunch_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    try
        //    {
        //        string launchSerial = serialLaunch.ReadLine();
        //        lemparKeEvent(launchSerial);
        //    }
        //    catch (Exception)
        //    {
        //        //MessageBox.Show(ex.Message.ToString());
        //    }
        //}

        void lemparKeEvent(string lempar)
        {
            if (NewSerialDataReceived != null)
            {
                NewSerialDataReceived(this, new SerialDataEventArgs(lempar));
            }
        }

    }
}
