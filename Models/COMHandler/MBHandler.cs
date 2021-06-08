using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MBMaster.Models.COMHandler
{
    public class MBHandler: INotifyPropertyChanged
    {
        private COMPortHandler _comPort;
        public COMPortHandler COMPort
        {
            get { return _comPort; }
            set 
            { 
                _comPort = value;
                NotifyProperyChanged("COMPort");
            }
        }


        private byte _slaveId=1;
        public byte SlaveId
        {
            get { return _slaveId; }
            set 
            { 
                _slaveId = value;
                NotifyProperyChanged("SlaveId");
            }
        }

        private byte _fc = 1;
        public byte FuncCode
        {
            get { return _fc; }
            set 
            { 
                _fc = value;
                NotifyProperyChanged("FuncCode");
            }
        }

        private Dictionary<byte, string> _fCodes = new Dictionary<byte, string>()
        {
            {1, "Read coils: 1"},
            {3, "Read registers: 3"},
            {15, "Write coils: 15"},
            {16, "Write registers: 16"}
        };
        public Dictionary<byte, string> FCodes
        {
            get { return _fCodes; }
            set
            {
                _fCodes = value;
                NotifyProperyChanged("FCodes");
            }
        }


        private ushort _startingAddr=0;
        public ushort StartingAddr
        {
            get { return _startingAddr; }
            set 
            { 
                _startingAddr = value;
                NotifyProperyChanged("StartingAddr");
            }
        }

        private ushort _coilOrRegsNr=1;
        public ushort CoilOrRegsNr
        {
            get { return _coilOrRegsNr; }
            set 
            { 
                _coilOrRegsNr = value;
                NotifyProperyChanged("CoilOrRegsNr");
            }
        }

        private ushort _crc;
        public ushort CRC
        {
            get { return _crc; }
            set 
            { 
                _crc = value;
                NotifyProperyChanged("CRC");
            }
        }


        private byte[] _tempRxBuffer=new byte[256];
        public byte[] TempRxBuffer
        {
            get { return _tempRxBuffer; }
            set 
            {
                _tempRxBuffer = value;
                NotifyProperyChanged("TempRxBuffer");
            }
        }

        private Dictionary<int, TextBox> _myTB;
        public Dictionary<int, TextBox> MyTB
        {
            get { return _myTB; }
            set 
            { 
                _myTB = value;
                NotifyProperyChanged("MyTB");
            }
        }

        private string _reqString = string.Empty;
        public string ReqString
        {
            get { return _reqString; }
            set 
            { 
                _reqString = value;
                NotifyProperyChanged("ReqString");
            }
        }


        public MBHelper mbHelper = new MBHelper();
        private MainWindow myMainWindow;
        private Timer updateReqString;
        private int updateReqStringIndex=0;

        // Notify property changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyProperyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

            switch (name)
            {
                case "SlaveId":
                    {
                        if (SlaveId > 0 && SlaveId < 247)
                        {
                            TempRxBuffer[0] = SlaveId;
                        }
                        else
                        {
                            MessageBox.Show("Invaled slave address!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            SlaveId = 1;
                        }
                        break;
                    }
                case "FuncCode":
                    {
                        if (FuncCode == 1 || FuncCode == 3 || FuncCode == 15 || FuncCode == 16)
                        {
                            TempRxBuffer[1] = FuncCode;
                        }
                        else
                        {
                            MessageBox.Show("Invaled MB function code!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            FuncCode = 1;
                        }
                        break;
                    }
                case "StartingAddr":
                    {
                        if (FuncCode == 1 || FuncCode == 15)
                        {
                            if (StartingAddr >= 0 && StartingAddr < 20)
                            {
                                TempRxBuffer[2] = mbHelper.Get_uint8_t(StartingAddr)[1];
                                TempRxBuffer[3] = mbHelper.Get_uint8_t(StartingAddr)[0];
                            }
                            else
                            {
                                MessageBox.Show("Invaled MB function code!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                StartingAddr = 0;
                            }
                        }
                        else if (FuncCode == 3 || FuncCode == 16)
                        {
                            if (StartingAddr >= 0 && StartingAddr < 50)
                            {
                                TempRxBuffer[2] = mbHelper.Get_uint8_t(StartingAddr)[1];
                                TempRxBuffer[3] = mbHelper.Get_uint8_t(StartingAddr)[0];
                            }
                            else
                            {
                                MessageBox.Show("Invaled MB function code!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                StartingAddr = 0;
                            }
                        }
                        break;
                    }
                case "CoilOrRegsNr":
                    {
                        if (FuncCode == 1 || FuncCode == 15)
                        {
                            if (CoilOrRegsNr <= 20 && CoilOrRegsNr > 0)
                            {
                                TempRxBuffer[4] = mbHelper.Get_uint8_t(CoilOrRegsNr)[1];
                                TempRxBuffer[5] = mbHelper.Get_uint8_t(CoilOrRegsNr)[0];
                                    
                                for(int i=1; i<=CoilOrRegsNr;i++)
                                {
                                    MyTB[i].Text = "0";
                                    MyTB[i].Visibility = Visibility.Visible;
                                }
                                for(int j=CoilOrRegsNr + 1; j<=50;j++)
                                {
                                    MyTB[j].Visibility = Visibility.Collapsed;
                                }

                            }
                            else
                            {
                                MessageBox.Show("Invaled coils or register number!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                CoilOrRegsNr = 1;
                            }

                        }
                        else if(FuncCode == 16)
                        {
                            if (CoilOrRegsNr <= 50 && CoilOrRegsNr >= 0)
                            {
                                TempRxBuffer[4] = mbHelper.Get_uint8_t(CoilOrRegsNr)[1];
                                TempRxBuffer[5] = mbHelper.Get_uint8_t(CoilOrRegsNr)[0];
                                TempRxBuffer[6] = (byte)(2 * CoilOrRegsNr);

                                for (int i = 1; i <= CoilOrRegsNr; i++)
                                {
                                    MyTB[i].Text = "-";
                                    MyTB[i].Visibility = Visibility.Visible;
                                }
                                for (int j = CoilOrRegsNr + 1; j <= 50; j++)
                                {
                                    MyTB[j].Visibility = Visibility.Collapsed;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invaled coils or register number!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                CoilOrRegsNr = 1;
                            }
                        }
                        break;
                    }
                default:
                    break;
            }

            if(FuncCode==16)
            {
                updateReqStringIndex = 1 + 1 + 2 + 2 + 1 + 2 * CoilOrRegsNr + 2;
            }
        }

        public MBHandler(MainWindow mainWindow)
        {
           COMPort = new COMPortHandler();

            myMainWindow = mainWindow;

            MyTB = new Dictionary<int, TextBox>()
            {
                {1, mainWindow.TB1 },
                {2, mainWindow.TB2 },
                {3, mainWindow.TB3 },
                {4, mainWindow.TB4 },
                {5, mainWindow.TB5 },
                {6, mainWindow.TB6 },
                {7, mainWindow.TB7 },
                {8, mainWindow.TB8 },
                {9, mainWindow.TB9 },
                {10, mainWindow.TB10 },
                {11, mainWindow.TB11 },
                {12, mainWindow.TB12 },
                {13, mainWindow.TB13 },
                {14, mainWindow.TB14 },
                {15, mainWindow.TB15 },
                {16, mainWindow.TB16 },
                {17, mainWindow.TB17 },
                {18, mainWindow.TB18 },
                {19, mainWindow.TB19 },
                {20, mainWindow.TB20 },
                {21, mainWindow.TB21 },
                {22, mainWindow.TB22 },
                {23, mainWindow.TB23 },
                {24, mainWindow.TB24 },
                {25, mainWindow.TB25 },
                {26, mainWindow.TB26 },
                {27, mainWindow.TB27 },
                {28, mainWindow.TB28 },
                {29, mainWindow.TB29 },
                {30, mainWindow.TB30 },
                {31, mainWindow.TB31 },
                {32, mainWindow.TB32 },
                {33, mainWindow.TB33 },
                {34, mainWindow.TB34 },
                {35, mainWindow.TB35 },
                {36, mainWindow.TB36 },
                {37, mainWindow.TB37 },
                {38, mainWindow.TB38 },
                {39, mainWindow.TB39 },
                {40, mainWindow.TB40 },
                {41, mainWindow.TB41 },
                {42, mainWindow.TB42 },
                {43, mainWindow.TB43 },
                {44, mainWindow.TB44 },
                {45, mainWindow.TB45 },
                {46, mainWindow.TB46 },
                {47, mainWindow.TB47 },
                {48, mainWindow.TB48 },
                {49, mainWindow.TB49 },
                {50, mainWindow.TB50 }
            };

            foreach(var item in MyTB)
            {
                item.Value.TextChanged += Value_TextChanged;
                item.Value.Visibility = Visibility.Collapsed;
            }

            updateReqString = new Timer(1000);
            updateReqString.Enabled = true;
            updateReqString.Elapsed += UpdateReqString_Elapsed;
            updateReqString.Start();

            myMainWindow.SEND_Btn.Click += SEND_Btn_Click;
        }

        private void SEND_Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch(FuncCode)
                {
                    case 16:
                        {
                            int frameLength = 1 + 1 + 2 + 2 + 1 + 2 * CoilOrRegsNr + 2;

                            byte[] crcArr = mbHelper.GetModbusCrc16(TempRxBuffer, frameLength-2);

                            TempRxBuffer[frameLength-2] = crcArr[1];
                            TempRxBuffer[frameLength - 1] = crcArr[0];

                            byte[] tempFrame = new byte[frameLength];

                            for (int k = 0; k < frameLength; k++)
                            {
                                tempFrame[k] = TempRxBuffer[k];
                            }


                            COMPort.TransmitArray = tempFrame;
                            

                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void UpdateReqString_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ReqString = "->";
                for (int i = 0; i < updateReqStringIndex; i++)
                {
                    ReqString = ReqString + TempRxBuffer[i].ToString() + " ";
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox textBox = sender as TextBox;

                TempRxBuffer[6 + 2*MyTB.ToList().Find(p => p.Value == textBox).Key] = mbHelper.Get_uint8_t(ushort.Parse(textBox.Text))[0];
                TempRxBuffer[6 + 2*MyTB.ToList().Find(p => p.Value == textBox).Key - 1] = mbHelper.Get_uint8_t(ushort.Parse(textBox.Text))[1];

            }
            catch ( Exception ex)
            {

            }
        }
    }
}
