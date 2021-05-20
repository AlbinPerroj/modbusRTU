using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MBMaster.Models
{
    public class MyCOMPort
    {

        private int _baud = 9600;
        public int Baudrate
        {
            get { return _baud; }
            set
            {
                _baud = value;
                COMPortRT.BaudRate = _baud;
                NotifyProperyChanged("Baudrate");
            }
        }

        private List<int> _baudList = new List<int>() { 600, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 128000 };
        public List<int> BaudrateList
        {
            get { return _baudList; }
            set
            {
                _baudList = value;
                NotifyProperyChanged("BaudrateList");
            }
        }



        private List<string> _comList = SerialPort.GetPortNames().ToList();
        public List<string> COMList
        {
            get { return _comList; }
            set
            {
                _comList = value;
                if (_comList != null)
                {
                    if (_comList.Count > 0)
                        _comPort = _comList[0];
                }
                NotifyProperyChanged("COMList");
            }
        }

        private string _comPort;
        public string ComPort
        {
            get { return _comPort; }
            set
            {
                _comPort = value;
                if (_comPort != null)
                    COMPortRT.PortName = _comPort;
                NotifyProperyChanged("ComPort");
            }
        }



        private int _dataBits = 8;
        public int DataBits
        {
            get { return _dataBits; }
            set
            {
                _dataBits = value;
                COMPortRT.DataBits = _dataBits;
                NotifyProperyChanged("DataBits");
            }
        }

        private List<int> _dataBitsList = new List<int>() { 5, 6, 7, 8 };
        public List<int> DataBitsList
        {
            get { return _dataBitsList; }
            set
            {
                _dataBitsList = value;
                NotifyProperyChanged("DataBitsList");
            }
        }



        private string _stopBits = "One";
        public string StopBits
        {
            get { return _stopBits; }
            set
            {
                _stopBits = value;
                switch (_stopBits)
                {
                    case "One":
                        {
                            COMPortRT.StopBits = System.IO.Ports.StopBits.One;
                            break;
                        }
                    case "OnePointFive":
                        {
                            COMPortRT.StopBits = System.IO.Ports.StopBits.OnePointFive;
                            break;
                        }
                    case "Two":
                        {
                            COMPortRT.StopBits = System.IO.Ports.StopBits.Two;
                            break;
                        }
                    default:
                        break;
                }
                NotifyProperyChanged("StopBits");
            }
        }

        private List<string> _stopbitsList = new List<string>() { "One", "OnePointFive", "Two" };
        public List<string> StopBitsList
        {
            get { return _stopbitsList; }
            set
            {
                _stopbitsList = value;
                NotifyProperyChanged("StopBitsList");
            }
        }



        private string _parity = "None";
        public string ParityControl
        {
            get { return _parity; }
            set
            {
                _parity = value;
                switch (_parity)
                {
                    case "None":
                        {
                            COMPortRT.Parity = Parity.None;
                            break;
                        }
                    case "Even":
                        {
                            COMPortRT.Parity = Parity.Even;
                            break;
                        }
                    case "Mark":
                        {
                            COMPortRT.Parity = Parity.Mark;
                            break;
                        }
                    case "Odd":
                        {
                            COMPortRT.Parity = Parity.Odd;
                            break;
                        }
                    case "Space":
                        {
                            COMPortRT.Parity = Parity.Space;
                            break;
                        }
                    default:
                        break;

                }
                NotifyProperyChanged("ParityControl");
            }
        }

        private List<string> _parityList = new List<string>() { "None", "Even", "Mark", "Odd", "Space" };
        public List<string> ParityList
        {
            get { return _parityList; }
            set
            {
                _parityList = value;
                NotifyProperyChanged("ParityList");
            }
        }



        private  SerialPort _comPortRT = new SerialPort();
        public  SerialPort COMPortRT
        {
            get { return _comPortRT; }
            set
            {
                _comPortRT = value;
                NotifyProperyChanged("StopBitsList");
            }
        }


        public MyCOMPort()
        { 
        }

        // Notify property changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyProperyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
