using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;

namespace MBMaster.Models.COMHandler
{
    public class COMPortHandler : SerialPort, INotifyPropertyChanged
    {

        #region Serial port properties

        private List<string> _portNameList = GetPortNames().ToList();
        public List<string> PortNameList
        {
            get { return _portNameList; }
            set
            {
                _portNameList = value;
                if (_portNameList == null)
                {
                    _portNameList.Add("-");
                }
                NotifyProperyChanged("PortNameList");
            }
        }



        private List<int> _baudrateList = new List<int>() { 600, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 128000 };
        public List<int> BaudrateList
        {
            get { return _baudrateList; }
            set
            {
                _baudrateList = value;
                NotifyProperyChanged("BaudrateList");
            }
        }

        private Dictionary<int, string> _dataBitsList = new Dictionary<int, string>()
        {
            { (int)DataBitsEnum.Eight, "Eight" },
            { (int)DataBitsEnum.Seven, "Seven" },
            { (int)DataBitsEnum.Six, "Six" },
            { (int)DataBitsEnum.Five, "Five" }
        };
        public Dictionary<int, string> DataBitsList
        {
            get { return _dataBitsList; }
            set
            {
                _dataBitsList = value;
                NotifyProperyChanged("DataBitsList");
            }
        }


        private Dictionary<int, string> _stopBitsList = new Dictionary<int, string>()
        {
            { (int)StopBits.One, "One" },
            { (int)StopBits.OnePointFive, "OnePointFive" },
            { (int)StopBits.Two, "Two" },
            { (int)StopBits.None, "None" }
        };
        public Dictionary<int, string> StopBitsList
        {
            get { return _stopBitsList; }
            set
            {
                _stopBitsList = value;
                NotifyProperyChanged("StopBitsList");
            }
        }

        private Dictionary<int, string> _parityList = new Dictionary<int, string>()
        {
            { (int)Parity.None, "None" },
            { (int)Parity.Even, "Even" },
            { (int)Parity.Odd ,"Odd" },
            { (int)Parity.Mark, "Mark" },
            { (int)Parity.Space, "Space" }
        };
        public Dictionary<int, string> ParityList
        {
            get { return _parityList; }
            set
            {
                _parityList = value;
                NotifyProperyChanged("ParityList");
            }
        }

        private List<string> _portNamesList;
        public List<string> PortNamesList
        {
            get { return _portNamesList; }
            set
            {
                _portNamesList = value;
                NotifyProperyChanged("PortNamesList");
            }
        }

        private string _pName;
        public string PName
        {
            get { return _pName; }
            set 
            { 
                _pName = value;
                if (!string.IsNullOrEmpty(_pName))
                {
                    PortName = _pName;
                }
                UpdatePortInfo();
                NotifyProperyChanged("PName");
            }
        }


        private int _baud = 9600;
        public int BRate
        {
            get { return _baud; }
            set
            {
                _baud = value;
                if (BaudrateList.Contains(_baud))
                {
                    BaudRate = _baud;
                }
                UpdatePortInfo();
                NotifyProperyChanged("BRate");
            }
        }

        private int _dBits = (int)DataBitsEnum.Eight;
        public int DBits
        {
            get { return _dBits; }
            set 
            { 
                _dBits = value;
                DataBits = (int)_dBits;
                UpdatePortInfo();
                NotifyProperyChanged("DBits");
            }
        }

        private int _sBits=(int)StopBits.One;
        public int SBits
        {
            get { return _sBits; }
            set 
            { 
                _sBits = value;
                StopBits = (StopBits)SBits;
                UpdatePortInfo();
                NotifyProperyChanged("SBits");
            }
        }

        private int _par = (int)Parity.None;
        public int ParityBit
        {
            get { return _par; }
            set
            { 
                _par = value;
                Parity = (Parity)_par;
                UpdatePortInfo();
                NotifyProperyChanged("ParityBit");
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

        #endregion


        private bool _portStatus;
        public bool PortStatus
        {
            get { return _portStatus; }
            set 
            { 
                _portStatus = value;

                if (_portStatus)
                {
                    scanPortsTmr.Stop();
                    if(PortNameList.Contains(PortName))
                    {
                        Open();
                    }
                    else
                    {
                        _portStatus = false;
                        scanPortsTmr.Start();
                    }
                }
                else 
                {
                    Close();
                    Dispose(true);
                    scanPortsTmr.Start();
                    
                }

                NotifyProperyChanged("PortStatus");
            }
        }


        private List<string> _portErrors;
        public List<string> PortErrors
        {
            get { return _portErrors; }
            set 
            { 
                _portErrors = value;
                NotifyProperyChanged("PortErrors");
            }
        }

        private List<string> _portExceptions;

        public List<string> PortExceptions
        {
            get { return _portExceptions; }
            set 
            { 
                _portExceptions = value;
                NotifyProperyChanged("PortExceptions");
            }
        }

        private byte[] _receiveArray;
        public byte[] ReceiveArray
        {
            get { return _receiveArray; }
            set 
            { 
                _receiveArray = value;
                NotifyProperyChanged("ReceiveArray");
            }
        }

        private byte[] _transmitArray;
        public byte[] TransmitArray
        {
            get { return _transmitArray; }
            set 
            {
                _transmitArray = value;

                MBHandler_Transmit();
                
                NotifyProperyChanged("TransmitArray");
            }
        }

        private Timer scanPortsTmr;

        // Notify property changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyProperyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public COMPortHandler()
        {
            // Initialize port parameters

            ErrorReceived += MBHandler_ErrorReceived;
            DataReceived += MBHandler_DataReceived;

            scanPortsTmr = new Timer(10000);
            scanPortsTmr.Enabled = true;
            scanPortsTmr.Elapsed += ScanPortsTmr_Elapsed;
            scanPortsTmr.Start();
        }


        private void UpdatePortInfo()
        {
            try
            {
                COMPortInfo = $"SerialPort->[{PortName} | {BaudRate} | {DataBits} | {StopBits} | {Parity} | Status:{PortStatus}";
            }
            catch (Exception ex)
            {

                PortErrors.Add("Error 'UpdatePortInfo': " + ex.Message);
            }
        }

        private void ScanPortsTmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                PortNameList = GetPortNames().ToList();
            }
            catch (Exception ex)
            {

                PortErrors.Add("Error 'ScanPortsTmr'" + ex.Message);
            }
        }

        private void MBHandler_Transmit()
        {
            try
            {
                if(IsOpen)
                {
                    Write(TransmitArray, 0, TransmitArray.Length);
                }
            }
            catch (Exception ex)
            {
                PortExceptions.Add("Exception 'MBHandler_Transmit': " + ex.Message);
            }
        }

        private void MBHandler_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sPort = sender as SerialPort;

                ReceiveArray = new byte[sPort.BytesToRead];
                sPort.Read(ReceiveArray, 0, ReceiveArray.Length);
                sPort.DiscardInBuffer();
            }
            catch (Exception ex)
            {
                PortExceptions.Add("Exception 'MBHandler_DataReceived': " + ex.Message);
            }
        }

        private void MBHandler_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            try
            {
                PortErrors.Add("Serial port error : " + e.EventType.ToString());
            }
            catch (Exception ex)
            {
                PortExceptions.Add("Exception 'MBHandler_ErrorReceived': " + ex.Message);
            }
        }

        public enum DataBitsEnum
        {
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8
        }
    }
}
