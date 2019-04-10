using System;
using System.Text;

namespace SDP_2S_Test
{
    class SY_CN
    {
        public static byte[] CRC16(byte[] data, int len)
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

                return new byte[] { hi, lo };
            }
            return new byte[] { 0, 0 };
        }
        public byte[] txGetRandomData()
        {

            byte[] txDataArray = new byte[5];
            txDataArray[0] = 0xA1;
            txDataArray[1] = 0x05;
            txDataArray[2] = 0x01;
            byte[] crc16 = CRC16(txDataArray, 3);
            txDataArray[3] = crc16[0];
            txDataArray[4] = crc16[1];
            return txDataArray;
        }
        public byte[] txSendFilms(byte data1, byte data2, UInt16 filmsAmount)
        {
            byte[] filmsArray = { (byte)(filmsAmount >> 8), (byte)filmsAmount };
            byte[] txDataArray = new byte[9];
            txDataArray[0] = 0xA1;
            txDataArray[1] = 0x09;
            txDataArray[2] = 0x02;

            txDataArray[3] = (byte)(data1 ^ 0xE5);
            txDataArray[4] = (byte)(data2 ^ 0xC8);
            txDataArray[5] = (byte)((txDataArray[3] + txDataArray[4]) ^ filmsArray[0]);
            txDataArray[6] = (byte)((txDataArray[3] ^ txDataArray[4]) ^ filmsArray[1]);

            byte[] crc16 = CRC16(txDataArray, 7);
            txDataArray[7] = crc16[0];
            txDataArray[8] = crc16[1];
            return txDataArray;
        }
        public byte[] txGetFilmsAmount()
        {

            byte[] txDataArray = new byte[5];
            txDataArray[0] = 0xA1;
            txDataArray[1] = 0x05;
            txDataArray[2] = 0x03;

            byte[] crc16 = CRC16(txDataArray, 3);
            txDataArray[3] = crc16[0];
            txDataArray[4] = crc16[1];
            return txDataArray;
        }
        public byte[] txClearFilms()
        {

            byte[] txDataArray = new byte[5];
            txDataArray[0] = 0xA1;
            txDataArray[1] = 0x05;
            txDataArray[2] = 0x04;

            byte[] crc16 = CRC16(txDataArray, 3);
            txDataArray[3] = crc16[0];
            txDataArray[4] = crc16[1];
            return txDataArray;
        }

        //private void txError(byte error)
        //{

        //    txDataArray = new byte[6];
        //    txDataArray[0] = 0xA1;
        //    txDataArray[1] = 0x06;
        //    txDataArray[2] = 0xee;
        //    txDataArray[3] = error;
        //    byte[] crc16 = CRC16(txDataArray, 4);
        //    txDataArray[4] = crc16[0];
        //    txDataArray[5] = crc16[1];

        //    startRxThread();

        //    serialPort.Write(txDataArray, 0, txDataArray[1]);
        //    txTextBox.Text = ToHexString(txDataArray, txDataArray[1]);
        //}

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
