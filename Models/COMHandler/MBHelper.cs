using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MBMaster.Models.COMHandler
{
    public class MBHelper
    {
        private List<string> _helperError = new List<string>();
        public List<string> HelperError
        {
            get { return _helperError; }
            set 
            { 
                _helperError = value;
                NotifyProperyChanged("HelperError");
            }
        }



        // Notify property changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyProperyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public MBHelper()
        {

        }

        public static ushort Get_uint16_t(byte loByte,byte hoByte)
        {
            ushort result = 0;
            try
            {
                byte[] temp = new byte[2];

                temp[0] = loByte;
                temp[1] = hoByte;

                result = (ushort)BitConverter.ToUInt16(temp,0);

            }
            catch (Exception ex)
            {
                result = 0;
            }
            return result;
        }

        public byte[] Get_uint8_t(ushort register)
        {

            byte[] resultArr = new byte[2];

            try
            {
                resultArr = BitConverter.GetBytes(register);
            }
            catch (Exception ex)
            {
                resultArr[0] = 0;
                resultArr[1] = 0;
            }
            return resultArr;
             
        }

        public byte[] GetModbusCrc16(byte[] bytes, int length)
        {
            ushort calcCRC;
            ushort temp;
            ushort flag;

            calcCRC = 0xFFFF;
            for (byte a = 0; a < length; a++)
            {
                calcCRC = (ushort)(calcCRC ^ bytes[a]);

                for (byte b = 1; b <= 8; b++)
                {
                    flag = (ushort)(calcCRC & 0x0001);
                    calcCRC >>= 1;
                    if (flag == 1)
                        calcCRC ^= 0xA001;
                }
            }

            temp = (ushort)(calcCRC >> 8);
            calcCRC = (ushort)((calcCRC << 8) | temp);
            calcCRC &= 0xFFFF;

            return new byte[] { (byte)(calcCRC & 255), (byte)(calcCRC >> 8) };

        }

    }
}
