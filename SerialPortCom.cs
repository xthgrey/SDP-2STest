using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace SDP_2S_Test
{
    class SerialPortCom
    {
        private SerialPort[] serialPort = null;

        private Queue<byte[]> txBuffer = null;

        private Queue<byte> rxSYCNTo760Queue = null;
        private Queue<byte> rxSYPCToCNQueue = null;
        private Queue<byte> rxSY760ToCNQueue = null;

        private byte[] rxSYCNTo760Array = null;
        private byte[] rxSYPCToCNArray = null;
        private byte[] rxSY760ToCNArray = null;

        private byte[] count = null;
        private Boolean rxSYCNTo760OverFlag = false;
        private Boolean rxSYPCToCNOverFlag = false;
        private Boolean rxSY760ToCNOverFlag = false;

        public Thread txThread = null;

        //private System.Timers.Timer timer;

        public Boolean[] recOver = { false, false, false };

        public SerialPortCom()
        {
            serialPort = new SerialPort[3];
            txBuffer = new Queue<byte[]>();
            rxSYCNTo760Queue = new Queue<byte>();
            rxSYPCToCNQueue = new Queue<byte>();
            rxSY760ToCNQueue = new Queue<byte>();

            rxSYCNTo760Array = new byte[10];
            rxSYPCToCNArray = new byte[10];
            rxSY760ToCNArray = new byte[10];

            count = new byte[3];

            ThreadStart txThreadStart = new ThreadStart(txSendData);
            txThread = new Thread(txThreadStart);
            txThread.Start();
        }

        public byte[] getRxSYCNTo760Array() {
            byte[] data = null;
            if (rxSYCNTo760OverFlag) {
                rxSYCNTo760OverFlag = false;
                data = rxSYCNTo760Array;
            }
            return data;
        }
        public byte[] getRxSYPCToCNArray()
        {
            byte[] data = null;
            if (rxSYPCToCNOverFlag) {
                rxSYPCToCNOverFlag = false;
                data = rxSYPCToCNArray;
            }
            return data;
        }
        public byte[] getRxSY760ToCNArray()
        {
            byte[] data = null;
            if (rxSY760ToCNOverFlag) {
                rxSY760ToCNOverFlag = false;
                data = rxSY760ToCNArray;
            }
            return data;
        }

        public void addTxDataToBuffer(byte[] txData)
        {
            txBuffer.Enqueue(txData);
        }

        public String[] getSerialPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public void openSerialPort(String serialPortName, int num)
        {
            try
            {
                serialPort[num] = new SerialPort(serialPortName, 115200, Parity.None, 8, StopBits.One);
                //serialPort[num].ReadTimeout = 1000;
                //serialPort[num].WriteTimeout = 1000;
                switch (num)
                {
                    case 0:
                        serialPort[0].DataReceived += rxSYCNTo760Data;
                        break;
                    case 1:
                        serialPort[1].DataReceived += rxSYPCToCNData;
                        break;
                    case 2:
                        serialPort[2].DataReceived += rxSY760ToCNData;
                        break;
                    default:
                        break;
                }
                try
                {
                        serialPort[num].Open();
                    
                    //timer = new System.Timers.Timer(100);
                    //timer.Elapsed += Timer_Elapsed;
                    //timer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
                    //timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；



                }
                catch (NullReferenceException)
                {
                    throw;

                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        //private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    dealRxSYCNTo760Data();
        //    dealRxSYPCToCNData();
        //    dealRxSY760ToCNData();
        //}

        public void closeSerialPort(int num)
        {
            try
            {
                
                if (serialPort[num] != null && serialPort[num].IsOpen) {
                    serialPort[num].Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void txSendData()
        {
            while (true)
            {
                if (txBuffer.Count > 0) {
                    byte[] txData = txBuffer.Dequeue();
                    if (txData != null) {
                        switch (txData[0])
                        {
                            case 0xA0:
                                serialPort[2].Write(txData, 0, txData[2]);
                                break;
                            case 0xA8:
                                serialPort[0].Write(txData, 0, txData[2]);
                                break;
                            case 0xA1:
                                serialPort[1].Write(txData, 0, txData[1]);
                                break;
                        }
                    }
                    
                }
                Thread.Sleep(100);
            }
        }

        //public void rxDealData()
        //{
        //    while (true)
        //    {
        //        dealRxSYCNTo760Data();
        //        dealRxSYPCToCNData();
        //        dealRxSY760ToCNData();
                
        //    }
        //}

        private void rxSYCNTo760Data(object sender, SerialDataReceivedEventArgs e)
        {
            byte len = (byte)serialPort[0].BytesToRead;
            for (int i = 0; i < len; i++)
            {
                rxSYCNTo760Queue.Enqueue((byte)serialPort[0].ReadByte());
            }
        }
        private void rxSYPCToCNData(object sender, SerialDataReceivedEventArgs e)
        {
            byte len = (byte)serialPort[1].BytesToRead;
            for (int i = 0; i < len; i++)
            {
                rxSYPCToCNQueue.Enqueue((byte)serialPort[1].ReadByte());
            }
        }
        private void rxSY760ToCNData(object sender, SerialDataReceivedEventArgs e)
        {
            byte len = (byte)serialPort[2].BytesToRead;
            for (int i = 0; i < len; i++)
            {
                rxSY760ToCNQueue.Enqueue((byte)serialPort[2].ReadByte());
            }
        }
        public void dealRxSYCNTo760Data()
        {
            if (rxSYCNTo760Queue.Count > 0) {
                rxSYCNTo760Array[count[0]] = rxSYCNTo760Queue.Dequeue();
                switch (count[0])
                {
                    case 0:
                        if (rxSYCNTo760Array[0] == 0xA0)
                        {
                            count[0]++;
                        }
                        else
                        {
                            count[0] = 0;
                        }
                        break;
                    case 1:
                        if (rxSYCNTo760Array[1] == 0xFF)
                        {
                            count[0]++;
                        }
                        else
                        {
                            count[0] = 0;
                        }
                        break;
                    case 2:
                        if (rxSYCNTo760Array[2] == 0x05)
                        {
                            count[0]++;
                        }
                        else
                        {
                            count[0] = 0;
                        }
                        break;
                    default:
                        count[0]++;
                        if (count[0] == rxSYCNTo760Array[2])
                        {
                            count[0] = 0;
                            rxSYCNTo760OverFlag = true;
                        }
                        break;
                }
            }

        }

        public void dealRxSYPCToCNData()
        {
            if (rxSYPCToCNQueue.Count > 0)
            {
                byte dataTemp = rxSYPCToCNQueue.Dequeue();
                rxSYPCToCNArray[count[1]] = dataTemp;
                switch (count[1])
                {
                    case 0:
                        if (rxSYPCToCNArray[0] == 0xA9)
                        {
                            count[1]++;
                        }
                        else
                        {
                            count[1] = 0;
                        }
                        break;
                    case 1:
                        if (rxSYPCToCNArray[1] == 0x05 || rxSYPCToCNArray[1] == 0x06 || rxSYPCToCNArray[1] == 0x07)
                        {
                            count[1]++;
                        }
                        else
                        {
                            count[1] = 0;
                        }
                        break;
                    default:
                        count[1]++;
                        if (count[1] == rxSYPCToCNArray[1])
                        {
                            byte[] crcData = SY_CN.CRC16(rxSYPCToCNArray, count[1] - 2);
                            if (crcData[0] == rxSYPCToCNArray[count[1] - 2] && crcData[1] == rxSYPCToCNArray[count[1] - 1]) {
                                rxSYPCToCNOverFlag = true;
                            }
                            count[1] = 0;
                        }
                        break;
                }
            }
        }
        public void dealRxSY760ToCNData()
        {
            if (rxSY760ToCNQueue.Count > 0) {
                rxSY760ToCNArray[count[2]] = rxSY760ToCNQueue.Dequeue();
                switch (count[2])
                {
                    case 0:
                        if (rxSY760ToCNArray[0] == 0xA8)
                        {
                            count[2]++;
                        }
                        else
                        {
                            count[2] = 0;
                        }
                        break;
                    case 1:
                        if (rxSY760ToCNArray[1] == 0x01)
                        {
                            count[2]++;
                        }
                        else
                        {
                            count[2] = 0;
                        }
                        break;
                    case 2:
                        if (rxSY760ToCNArray[2] == 0x06)
                        {
                            count[2]++;
                        }
                        else
                        {
                            count[2] = 0;
                        }
                        break;
                    default:
                        count[2]++;
                        if (count[2] == rxSY760ToCNArray[2])
                        {
                            count[2] = 0;
                            rxSY760ToCNOverFlag = true;
                        }
                        break;
                }
            }
        }
    }
}
