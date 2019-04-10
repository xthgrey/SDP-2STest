using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDP_2S_Test
{
    class SY_760
    {
        private Random rd;
        private byte[] txDataArray;
        private byte count;
        public SY_760()
        {
            rd = new Random();
        }

        public static byte CRC16(byte[] data, int len)
        {
            if (len > 0)
            {
                ushort crc = 0xFFFF;

                for (int i = 0; i < len; i++)
                {
                    crc = (ushort)(crc ^ (data[i]));
                    for (int j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                    }
                }
                byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
                byte lo = (byte)(crc & 0x00FF);         //低位置

                return lo;
            }
            return 0;
        }
        public byte[] SY760ToCNPrint() {
            txDataArray = new byte[5];
            txDataArray[0] = 0xA0;
            txDataArray[1] = 0x01;
            txDataArray[2] = 0x05;
            txDataArray[3] = (byte)rd.Next(0, 256);
            txDataArray[4] = CRC16(txDataArray, txDataArray[2] - 1);
            return txDataArray;
        }
        public byte[] SY760ToCNRepeat()
        {
            txDataArray = new byte[5];
            txDataArray[0] = 0xA0;
            txDataArray[1] = 0x55;
            txDataArray[2] = 0x05;
            txDataArray[3] = (byte)rd.Next(0, 256);
            txDataArray[4] = CRC16(txDataArray, txDataArray[2] - 1);
            return txDataArray;
        }
        public byte[] SYCNTo760Repeat()
        {
            txDataArray = new byte[6];
            txDataArray[0] = 0xA8;
            txDataArray[1] = 0x55;
            txDataArray[2] = 0x06;
            txDataArray[3] = (byte)rd.Next(0, 256);
            count++;
            txDataArray[4] = count;
            txDataArray[5] = CRC16(txDataArray, txDataArray[2] - 1);
            return txDataArray;
        }
        public byte[] SY760ToCNHeart()
        {
            txDataArray = new byte[5];
            txDataArray[0] = 0xA0;
            txDataArray[1] = 0xFF;
            txDataArray[2] = 0x05;
            txDataArray[3] = (byte)rd.Next(0, 256);
            txDataArray[4] = CRC16(txDataArray, txDataArray[2] - 1);
            return txDataArray;
        }
        public byte[] SYCNTo760Heart( byte rxRandom)
        {
            
            txDataArray = new byte[6];
            txDataArray[0] = 0xA8;
            txDataArray[1] = 0xFF;
            txDataArray[2] = 0x06;
            txDataArray[3] = (byte)rd.Next(0, 256);
            byte xx = (byte)(rxRandom + 1 - txDataArray[3]);
            txDataArray[4] = xx;
            txDataArray[5] = CRC16(txDataArray, txDataArray[2] - 1);
            return txDataArray;
        }

        public string ToHexString(byte[] bytes, int length)

        {
            string hexString = string.Empty;

            if (bytes != null)

            {

                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < length; i++)

                {

                    strB.Append("0x" + bytes[i].ToString("X2") + " ");

                }

                hexString = strB.ToString();

            }
            return hexString;

        }
        public string ToHexString(byte[] bytes)

        {
            string hexString = string.Empty;
            int length = bytes.Length;
            if (bytes != null)

            {

                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < length; i++)

                {

                    strB.Append("0x" + bytes[i].ToString("X2") + " ");

                }

                hexString = strB.ToString();

            }
            return hexString;

        }
    }
}
