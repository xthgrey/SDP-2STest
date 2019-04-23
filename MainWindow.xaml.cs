using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SDP_2S_Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPortCom serialPortCom;
        private String[] serialPortNames;
        private SY_760 sY_760;
        private SY_CN sY_CN;
        private Timer rxTimer;
        private Timer j1AccreditFilmsButton_ClickTimer, j1InquireFilmsButton_ClickTimer, j3MinusButton_ClickTimer, j7HeartButton_ClickTimer;
        private byte[] dataArray;
        private byte J3MinusRandomData;
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dataArray = new byte[10];
            sY_760 = new SY_760();
            sY_CN = new SY_CN();
            serialPortCom = new SerialPortCom();

            rxTimer = new Timer(10);
            rxTimer.Elapsed += Timer_Elapsed;//到达时间的时候执行事件；
            rxTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            rxTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

            
            
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            serialPortCom.dealRxSYCNTo760Data();
            serialPortCom.dealRxSYPCToCNData();
            serialPortCom.dealRxSY760ToCNData();
            Dispatcher.Invoke(new Action(delegate
            {
                if ((dataArray = serialPortCom.getRxSYCNTo760Array()) != null)
                {
                    j7RxTextBlock.Text = sY_760.ToHexString(dataArray, dataArray[2]);
                    j7TextBlock.Text = "心跳成功";
                }
                if ((dataArray = serialPortCom.getRxSYPCToCNArray()) != null)
                {
                    switch (dataArray[2])
                    {
                        case 0x01:
                            j1RxAccreditTextBlock.Text = sY_CN.ToHexString(dataArray, dataArray[1]);
                            byte[] data = sY_CN.txSendFilms(dataArray[3], dataArray[4], ushort.Parse(j1AccreditFilmsTextBlock.Text));
                            serialPortCom.addTxDataToBuffer(data);
                            j1TxAccreditTextBlock.Text = sY_CN.ToHexString(data);
                            j1ComBorder.Background = Brushes.Green;
                            j1TextBlock.Text = "获取随机数成功";
                            break;
                        case 0x02:
                            j1RxAccreditTextBlock.Text = sY_CN.ToHexString(dataArray, dataArray[1]);
                            j1ComBorder.Background = Brushes.Green;
                            j1TextBlock.Text = "授权成功";
                            break;
                        case 0x03:
                            j1RxInquireTextBlock.Text = sY_CN.ToHexString(dataArray, dataArray[1]);
                            j1ComBorder.Background = Brushes.Green;
                            j1TextBlock.Text = "查询成功";
                            j1InquireTextBlock.Text = ((dataArray[3] << 8) + dataArray[4]).ToString();
                            break;
                        case 0x04:
                            j1RxClearTextBlock.Text = sY_CN.ToHexString(dataArray, dataArray[1]);
                            j1ComBorder.Background = Brushes.Green;
                            //J1ClearTextBlock.Text = "片量已清零";
                            j1TextBlock.Text = "清零成功";
                            break;
                        default:
                            j1ComBorder.Background = Brushes.Red;
                            j1TextBlock.Text = "数据错误";
                            break;
                    }
                }
                if ((dataArray = serialPortCom.getRxSY760ToCNArray()) != null)
                {
                    byte result = (byte)(dataArray[4] + dataArray[3] - J3MinusRandomData);
                    if (result == 0) {
                        j3MinusTextBlock.Background = Brushes.Red;
                        j3MinusTextBlock.Text = "没片量了";
                        j3TextBlock.Text = j3MinusTextBlock.Text;
                    }
                    else {
                        j3MinusTextBlock.Background = Brushes.Green;
                        j3MinusTextBlock.Text = "还有片量";
                        j3TextBlock.Text = j3MinusTextBlock.Text;
                    }
                    j3RxMinusTextBlock.Text = sY_CN.ToHexString(dataArray, dataArray[2]);
                    
                }
            }));
        }

        private void J7SerialPortButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.Equals(j7SerialPortButton.Content, "打开串口"))
            {
                try
                {
                    serialPortCom.openSerialPort(j7ComboBox.SelectedValue.ToString(), 0);
                    j7SerialPortButton.Content = "关闭串口";

                    j7HeartButton.IsEnabled = true;

                    j7ComboBox.IsEnabled = false;

                    j7ComBorder.Background = Brushes.Green;
                    j7TextBlock.Text = "串口打开成功";


                }
                catch (NullReferenceException)
                {
                    j7ComBorder.Background = Brushes.Red;
                    j7TextBlock.Text = "未选择串口";
                }
                catch (Exception)
                {
                    j7ComBorder.Background = Brushes.Red;
                    j7TextBlock.Text = "打开串口失败";
                }

            }
            else if (String.Equals(j7SerialPortButton.Content, "关闭串口"))
            {
                serialPortCom.closeSerialPort(0);
                j7SerialPortButton.Content = "打开串口";

                j7HeartButton.IsEnabled = false;

                j7ComboBox.IsEnabled = true;

                j7TxTextBlock.Text = "";
                j7RxTextBlock.Text = "";

                j7ComBorder.Background = Brushes.Green;
                j7TextBlock.Text = "串口关闭成功";

            }
        }

        private void J1SerialPortButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.Equals(j1SerialPortButton.Content, "打开串口"))
            {
                try
                {
                    serialPortCom.openSerialPort(j1ComboBox.SelectedValue.ToString(), 1);
                    j1SerialPortButton.Content = "关闭串口";

                    j1AccreditFilmsButton.IsEnabled = true;
                    j1InquireFilmsButton.IsEnabled = true;
                    j1clearFilmsButton.IsEnabled = true;

                    j1ComboBox.IsEnabled = false;

                    j1ComBorder.Background = Brushes.Green;
                    j1TextBlock.Text = "串口打开成功";

                }
                catch (NullReferenceException)
                {
                    j1ComBorder.Background = Brushes.Red;
                    j1TextBlock.Text = "未选择串口";
                }
                catch (Exception)
                {
                    j1ComBorder.Background = Brushes.Red;
                    j1TextBlock.Text = "打开串口失败";
                }

            }
            else if (String.Equals(j1SerialPortButton.Content, "关闭串口"))
            {
                serialPortCom.closeSerialPort(1);
                j1SerialPortButton.Content = "打开串口";

                j1AccreditFilmsButton.IsEnabled = false;
                j1InquireFilmsButton.IsEnabled = false;
                j1clearFilmsButton.IsEnabled = false;

                j1ComboBox.IsEnabled = true;

                j1InquireTextBlock.Text = "";
                J1ClearTextBlock.Text = "";
                j1TxAccreditTextBlock.Text = "";
                j1RxAccreditTextBlock.Text = "";
                j1TxInquireTextBlock.Text = "";
                j1RxInquireTextBlock.Text = "";
                j1TxClearTextBlock.Text = "";
                j1RxClearTextBlock.Text = "";

                j1ComBorder.Background = Brushes.Green;
                j1TextBlock.Text = "串口关闭成功";

            }
        }

        private void J3SerialPortButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.Equals(j3SerialPortButton.Content, "打开串口"))
            {
                try
                {
                    serialPortCom.openSerialPort(j3ComboBox.SelectedValue.ToString(), 2);
                    j3SerialPortButton.Content = "关闭串口";

                    j3MinusButton.IsEnabled = true;

                    j3ComboBox.IsEnabled = false;

                    j3ComBorder.Background = Brushes.Green;
                    j3TextBlock.Text = "串口打开成功";


                }
                catch (NullReferenceException)
                {
                    j3ComBorder.Background = Brushes.Red;
                    j3TextBlock.Text = "未选择串口";
                }
                catch (Exception)
                {
                    j3ComBorder.Background = Brushes.Red;
                    j3TextBlock.Text = "打开串口失败";
                }

            }
            else if (String.Equals(j3SerialPortButton.Content, "关闭串口"))
            {
                serialPortCom.closeSerialPort(2);
                j3SerialPortButton.Content = "打开串口";

                j3MinusButton.IsEnabled = false;

                j3ComboBox.IsEnabled = true;

                j3TxMinusTextBlock.Text = "";
                j3RxMinusTextBlock.Text = "";
                j3MinusTextBlock.Text = "";

                j3ComBorder.Background = Brushes.Green;
                j3TextBlock.Text = "串口关闭成功";

                
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            serialPortCom.closeSerialPort(0);
            serialPortCom.closeSerialPort(1);
            serialPortCom.closeSerialPort(2);
            rxTimer.Close();
            serialPortCom.txThread.Abort();
        }

        private void J1AccreditFilmsButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = sY_CN.txGetRandomData();
            serialPortCom.addTxDataToBuffer(data);
            j1TxAccreditTextBlock.Text = sY_CN.ToHexString(data);
            j1RxAccreditTextBlock.Text = "";
            j1AccreditFilmsButton.IsEnabled = false;

            j1AccreditFilmsButton_ClickTimer = new Timer(1000);
            j1AccreditFilmsButton_ClickTimer.Elapsed += J1AccreditFilmsButton_ClickTimer_Elapsed;//到达时间的时候执行事件；
            j1AccreditFilmsButton_ClickTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            j1AccreditFilmsButton_ClickTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }

        private void J1AccreditFilmsButton_ClickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            j1AccreditFilmsButton.Dispatcher.Invoke(new Action(delegate
            {
                if (String.Equals(j1SerialPortButton.Content, "关闭串口")) {
                    j1AccreditFilmsButton.IsEnabled = true;
                }
                
            }));
            
        }

        private void J7HeartButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = sY_760.SYCNTo760Heart(0);
            serialPortCom.addTxDataToBuffer(data);
            j7TxTextBlock.Text = sY_CN.ToHexString(data);
            j7RxTextBlock.Text = "";

            j7HeartButton.IsEnabled = false;

            j7HeartButton_ClickTimer = new Timer(8000);
            j7HeartButton_ClickTimer.Elapsed += J7HeartButton_ClickTimer_Elapsed;//到达时间的时候执行事件；
            j7HeartButton_ClickTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            j7HeartButton_ClickTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

        }

        private void J7HeartButton_ClickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (String.Equals(j7SerialPortButton.Content, "关闭串口")) {
                    j7HeartButton.IsEnabled = true;
                }
                
            }));
            
        }

        private void J1InquireFilmsButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = sY_CN.txGetFilmsAmount();
            serialPortCom.addTxDataToBuffer(data);
            j1TxInquireTextBlock.Text = sY_CN.ToHexString(data);
            j1RxInquireTextBlock.Text = "";
            
        }

        private void J1clearFilmsButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = sY_CN.txClearFilms();
            serialPortCom.addTxDataToBuffer(data);
            j1TxClearTextBlock.Text = sY_CN.ToHexString(data);
            j1RxClearTextBlock.Text = "";
            j1clearFilmsButton.IsEnabled = false;

            j1InquireFilmsButton_ClickTimer = new Timer(1000);
            j1InquireFilmsButton_ClickTimer.Elapsed += J1InquireFilmsButton_ClickTimer_Elapsed; ;//到达时间的时候执行事件；
            j1InquireFilmsButton_ClickTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            j1InquireFilmsButton_ClickTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }

        private void J7ComboBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            serialPortNames = serialPortCom.getSerialPortNames();
            j7ComboBox.ItemsSource = serialPortNames;
        }

        private void J1ComboBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            serialPortNames = serialPortCom.getSerialPortNames();
            j1ComboBox.ItemsSource = serialPortNames;
        }

        private void J3ComboBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            serialPortNames = serialPortCom.getSerialPortNames();
            j3ComboBox.ItemsSource = serialPortNames;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Q))
            {
                if (j7HeartButton.IsEnabled) {
                    byte[] data = sY_760.SYCNTo760Heart(0);
                    serialPortCom.addTxDataToBuffer(data);
                    j7TxTextBlock.Text = sY_CN.ToHexString(data);
                    j7RxTextBlock.Text = "";

                    j7HeartButton.IsEnabled = false;

                    j7HeartButton_ClickTimer = new Timer(8000);
                    j7HeartButton_ClickTimer.Elapsed += J7HeartButton_ClickTimer_Elapsed;//到达时间的时候执行事件；
                    j7HeartButton_ClickTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
                    j7HeartButton_ClickTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                }
                
            }
            else if (Keyboard.IsKeyDown(Key.W)) {
                if (j1AccreditFilmsButton.IsEnabled) {
                    byte[] data = sY_CN.txGetRandomData();
                    serialPortCom.addTxDataToBuffer(data);
                    j1TxAccreditTextBlock.Text = sY_CN.ToHexString(data);
                    j1RxAccreditTextBlock.Text = "";
                    j1AccreditFilmsButton.IsEnabled = false;

                    j1AccreditFilmsButton_ClickTimer = new Timer(1000);
                    j1AccreditFilmsButton_ClickTimer.Elapsed += J1AccreditFilmsButton_ClickTimer_Elapsed;//到达时间的时候执行事件；
                    j1AccreditFilmsButton_ClickTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
                    j1AccreditFilmsButton_ClickTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                }
                
            }
            else if (Keyboard.IsKeyDown(Key.E))
            {
                if (j1InquireFilmsButton.IsEnabled) {
                    byte[] data = sY_CN.txGetFilmsAmount();
                    serialPortCom.addTxDataToBuffer(data);
                    j1TxInquireTextBlock.Text = sY_CN.ToHexString(data);
                    j1RxInquireTextBlock.Text = "";
                }
               
            }
            else if (Keyboard.IsKeyDown(Key.R))
            {
                if (j1clearFilmsButton.IsEnabled) {
                    byte[] data = sY_CN.txClearFilms();
                    serialPortCom.addTxDataToBuffer(data);
                    j1TxClearTextBlock.Text = sY_CN.ToHexString(data);
                    j1RxClearTextBlock.Text = "";
                    j1clearFilmsButton.IsEnabled = false;

                    j1InquireFilmsButton_ClickTimer = new Timer(1000);
                    j1InquireFilmsButton_ClickTimer.Elapsed += J1InquireFilmsButton_ClickTimer_Elapsed; ;//到达时间的时候执行事件；
                    j1InquireFilmsButton_ClickTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
                    j1InquireFilmsButton_ClickTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                }
               
            }
            else if (Keyboard.IsKeyDown(Key.T))
            {
                if (j3MinusButton.IsEnabled) {
                    byte[] data = sY_760.SY760ToCNPrint();
                    J3MinusRandomData = data[3];
                    serialPortCom.addTxDataToBuffer(data);
                    j3TxMinusTextBlock.Text = sY_CN.ToHexString(data);
                    j3RxMinusTextBlock.Text = "";
                    j3MinusButton.IsEnabled = false;

                    j3MinusButton_ClickTimer = new Timer(1000);
                    j3MinusButton_ClickTimer.Elapsed += J3MinusButton_ClickTimer_Elapsed; ; ;//到达时间的时候执行事件；
                    j3MinusButton_ClickTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
                    j3MinusButton_ClickTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                }
                
            }
        }

        private void J1InquireFilmsButton_ClickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            j1clearFilmsButton.Dispatcher.Invoke(new Action(delegate
            {
                if (String.Equals(j1SerialPortButton.Content, "关闭串口"))
                {
                    j1clearFilmsButton.IsEnabled = true;
                }
                
            }));
        }

        private void J3MinusButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = sY_760.SY760ToCNPrint();
            J3MinusRandomData = data[3];
            serialPortCom.addTxDataToBuffer(data);
            j3TxMinusTextBlock.Text = sY_CN.ToHexString(data);
            j3RxMinusTextBlock.Text = "";
            j3MinusButton.IsEnabled = false;

            j3MinusButton_ClickTimer = new Timer(1000);
            j3MinusButton_ClickTimer.Elapsed += J3MinusButton_ClickTimer_Elapsed; ; ;//到达时间的时候执行事件；
            j3MinusButton_ClickTimer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            j3MinusButton_ClickTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

        }

        private void J3MinusButton_ClickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (String.Equals(j3SerialPortButton.Content,"关闭串口")) {
                    j3MinusButton.IsEnabled = true;
                }
                
            }));
        }
    }
}
