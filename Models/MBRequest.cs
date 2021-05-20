using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MBMaster.Models
{
    public class MBRequest :INotifyPropertyChanged
    {
        private byte _id = 1;
        public byte ID
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyProperyChanged("ID");
            }
        }


        private byte _fc = 3;
        public byte FC
        {
            get { return _fc; }
            set
            {
                _fc = value;
                NotifyProperyChanged("FC");
            }
        }

        private Dictionary<byte, string> _fCodes = new Dictionary<byte, string>()
        {
            {1, "Read discrete output-FC:1"},
            {2, "Read digital output-FC:2"},
            {3, "Read analog output-FC:3"},
            {4, "Read analog input-FC:4"},
            {5, "Write discrete output-FC:5"},
            {6, "Write analog output-FC:6"},
            {15, "Write multiple discrete pins-FC:15"},
            {10, "Write multiple analog outputs-FC:10"}
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


        private ushort _startAddr = 40000;
        public ushort StartAddr
        {
            get { return _startAddr; }
            set
            {
                _startAddr = value;
                NotifyProperyChanged("StartAddr");
            }
        }


        private ushort _nrReg = 10;
        public ushort NrReg
        {
            get { return _nrReg; }
            set
            {
                _nrReg = value;
                NotifyProperyChanged("NrReg");
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


        private List<byte> _mbPDU = new List<byte>();

        public List<byte> MBPDU
        {
            get { return _mbPDU; }
            set 
            { 
                _mbPDU = value;
                NotifyProperyChanged("MBPDU");
            }
        }


        public MBRequest()
        {

        }

        // Notify property changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyProperyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            if (name != "CRC")
            {
                byte[] crc = GetModbusCrc16(PDU().ToArray());
                CRC = (ushort)(256 * crc[1] + crc[0]);
            }

            if(name.Equals("FC"))
            {
                switch (FC)
                {
                    case 1:
                        var result = FC1_PDU();
                        if (!result)
                            MBPDU.Clear();
                        break;
                    default:
                        break;
                }
            }

            
        }


        private List<byte> PDU()
        {
            bool result = true;

            List<byte> tempPDU = new List<byte>();

            try
            {
                if (ID != null && ID >= 0 && ID <= 247)
                {
                    tempPDU.Add(ID);
                }
                else
                {
                    result = false;
                }

                if (FC == 1)
                {
                    tempPDU.Add(FC);
                }
                else
                {
                    result = false;
                }

                if (StartAddr != null)
                {
                    tempPDU.Add((byte)(StartAddr >> 8));
                    tempPDU.Add((byte)(StartAddr & 255));
                }
                else
                {
                    result = false;
                }

                if (NrReg != null)
                {
                    tempPDU.Add((byte)(NrReg >> 8));
                    tempPDU.Add((byte)(NrReg & 255));
                }
                else
                {
                    result = false;
                }

            }
            catch
            {
                result = false;
            }

             return tempPDU;
        }

        private bool FC1_PDU()
        {
            bool result = true;
            try
            {
                MBPDU.Clear();

                if(ID != null && ID >= 0 && ID <= 247)
                {
                    MBPDU.Add(ID);
                }
                else
                {
                    result = false;
                }

                if(FC==1)
                {
                    MBPDU.Add(FC);
                }
                else
                {
                    result = false;
                }

                if(StartAddr != null)
                {
                    MBPDU.Add((byte)(StartAddr >> 8));
                    MBPDU.Add((byte)(StartAddr & 255));
                }
                else
                {
                    result = false;
                }

                if(NrReg != null)
                {
                    MBPDU.Add((byte)(NrReg >> 8));
                    MBPDU.Add((byte)(NrReg & 255));
                }
                else
                {
                    result = false;
                }

                var tempPDU = MBPDU.ToArray();

                MBPDU.Add(GetModbusCrc16(tempPDU)[1]);
                MBPDU.Add(GetModbusCrc16(tempPDU)[0]);
            }
            catch
            {
                result = false;
            }
            return result;
        }


        public static byte[] GetModbusCrc16(byte[] bytes)
        {
            byte crcRegister_H = 0xFF, crcRegister_L = 0xFF; // presets a 16-bit register value 0xFFFF

            byte polynomialCode_H = 0xA0, polynomialCode_L = 0x01; // polynomial code 0xA001

            for (int i = 0; i < bytes.Length; i++)
            {
                crcRegister_L = (byte)(crcRegister_L ^ bytes[i]);

                for (int j = 0; j < 8; j++)
                {
                    byte tempCRC_H = crcRegister_H;
                    byte tempCRC_L = crcRegister_L;

                    crcRegister_H = (byte)(crcRegister_H >> 1);
                    crcRegister_L = (byte)(crcRegister_L >> 1);
                    // Finally, a bit after the first should be the lower right front upper right: If the last digit is a high-low 1 right up front
                    if ((tempCRC_H & 0x01) == 0x01)
                    {
                        crcRegister_L = (byte)(crcRegister_L | 0x80);
                    }

                    if ((tempCRC_L & 0x01) == 0x01)
                    {
                        crcRegister_H = (byte)(crcRegister_H ^ polynomialCode_H);
                        crcRegister_L = (byte)(crcRegister_L ^ polynomialCode_L);
                    }
                }
            }

            return new byte[] { crcRegister_L, crcRegister_H };

        }

    }
}
