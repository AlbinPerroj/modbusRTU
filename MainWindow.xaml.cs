using MBMaster.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MBMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MyCOMPort _comPort;
        public MyCOMPort MyCOM
        {
            get { return _comPort; }
            set 
            { 
                _comPort = value;
                NotifyProperyChanged("MyCOM");
            }
        }

        private string _portInfo;
        public string COMPortInfo
        {
            get { return _portInfo; }
            set 
            { 
                _portInfo = value;
                NotifyProperyChanged("COMPortInfo");
            }
        }

        private MBRequest _mbReq;

        public MBRequest MBReq
        {
            get { return _mbReq; }
            set
            { 
                _mbReq = value;
                NotifyProperyChanged("MBReq");
            }
        }

        private string _rxString;

        public string RxString
        {
            get { return _rxString; }
            set 
            { 
                _rxString = value;
                NotifyProperyChanged("RxString");
            }
        }

        private bool _comOpened;

        public bool COMOpened
        {
            get { return _comOpened; }
            set 
            { 
                _comOpened = value;
                if(_comOpened)
                {
                    MyCOM.COMPortRT.Open();
                }
                else 
                {
                    MyCOM.COMPortRT.Close();
                }
                NotifyProperyChanged("COMOpened");
            }
        }




        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            MyCOM = new MyCOMPort();
            MyCOM_PropertyChanged(MyCOM, new PropertyChangedEventArgs("COMPortRT"));
            MyCOM.PropertyChanged += MyCOM_PropertyChanged;

            MyCOM.COMPortRT.DataReceived += COMPortRT_DataReceived;

            MBReq = new MBRequest();
 
        }

        private void COMPortRT_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = sender as SerialPort;

            RxString = string.Empty;
            if (MyCOM.COMPortRT.IsOpen)
            {
                byte[] data = new byte[sp.BytesToRead];
                sp.Read(data, 0, data.Length);
                data.ToList().ForEach(a => RxString = RxString + a.ToString() + " : ");
                sp.DiscardInBuffer();
            }
        }

        private void MyCOM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MyCOMPort myCOMPort = sender as MyCOMPort;

            COMPortInfo = $"COM Port:\t- PortName: {myCOMPort.COMPortRT.PortName}\t- BaudRate: {myCOMPort.COMPortRT.BaudRate}\t- DataBits: {myCOMPort.COMPortRT.DataBits}\t- StopBits: {myCOMPort.COMPortRT.StopBits}\t- Parity: {myCOMPort.COMPortRT.Parity}";
        }

        // Notify property changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyProperyChanged([CallerMemberName] string name = null )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        byte[] arr = new byte[10];

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MyCOM.COMPortRT.IsOpen)
            {
                byte[] byteArr = MBReq.MBPDU.ToArray();
                MyCOM.COMPortRT.Write(byteArr, 0, byteArr.Length);
            }
        }
    }

}
