using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

using FTDI_CAN;
using DC_Debug.Devices;

namespace DC_Debug
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer sendMsg;
        DispatcherTimer updateForm;
        ICanDriver canDEDriver;
        ICanMessage txMsg;
        CanMessages MessagesSet;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;

        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            updateForm.Stop();
            sendMsg.Dispose();
            canDEDriver.Stop();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txMsg = new CAN_Message();
            MessagesSet = new CanMessages();

            canDEDriver = new UCan();
            canDEDriver.OnReadMessage += CanDEDriver_OnReadMessage;            
            
            canDEDriver.Start(0, 500);
            canDEDriver.SetDiagnosticMode(false);

            StackPanel_KeyDown((object)spControlSource, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Enter));
            control_Checked((object)spControlSource, null);

            sendMsg = new Timer(SendMsg, MessagesSet, 0, 100);
            updateForm = new DispatcherTimer(DispatcherPriority.Background);
            updateForm.Interval = TimeSpan.FromMilliseconds(50);
            updateForm.Tick += UpdateForm;  //new Timer(UpdateForm, MessagesSet, 0, 50);
            updateForm.Start();
            
        }

        private void CanDEDriver_OnReadMessage(CAN_Message msg)
        {
            uint node_id = msg.id & 0xFF;
            if ((msg.id & 0xFFFFFF00) == 0x200)
            {
                if(node_id == 1)
                    MessagesSet.rxMsg_1.SetMsgData(msg.data);
            }
            else if((msg.id & 0xFFFFFF00) == 0x300)
            {
                if (node_id == 1)
                    MessagesSet.rxMsg_2.SetMsgData(msg.data);
            }
            else if ((msg.id <= 15) && (msg.id >= 1))
            {
                if (node_id == 1)
                    MessagesSet.rxMsg_3.SetMsgData(msg.data);
            }
        }

        void SendMsg(object state)
        {
            txMsg.id = 0x100;
            txMsg.Ext = false;
            txMsg.dlc = 8;
            txMsg.data = MessagesSet.txMsg.GetMsgData();

            if(canDEDriver.IsConnected)
                canDEDriver.SendMessage(txMsg);
        }
        void UpdateForm(object state, EventArgs e)
        {
            if(canDEDriver.IsConnected)
            {
                rectAdapterStatus.Fill = Brushes.Green;
                tbAdapterStatus.Text = "Connected";
            }
            else 
            {
                rectAdapterStatus.Fill = Brushes.Red;
                tbAdapterStatus.Text = "Not Connected";
            }

            // Set Values
            tbUin.Text = MessagesSet.rxMsg_1.InputVoltage.ToString("F", CultureInfo.InvariantCulture);
            tbIin.Text = MessagesSet.rxMsg_1.PrimaryCurrent.ToString("F", CultureInfo.InvariantCulture);
            tbUout.Text = MessagesSet.rxMsg_1.OutputVoltage.ToString("F", CultureInfo.InvariantCulture);
            tbIout.Text = MessagesSet.rxMsg_1.OutputCurrent.ToString("F", CultureInfo.InvariantCulture);
            tbPin.Text = MessagesSet.rxMsg_2.InputPower.ToString("F", CultureInfo.InvariantCulture);
            tbPout.Text = MessagesSet.rxMsg_2.OutputPower.ToString("F", CultureInfo.InvariantCulture);
            tbInvTemp.Text = MessagesSet.rxMsg_2.InvTemp.ToString();
            tbRectTemp.Text = MessagesSet.rxMsg_2.RectTemp.ToString();
            tbEfficiency.Text = MessagesSet.rxMsg_2.Efficiency.ToString("F", CultureInfo.InvariantCulture);

            // Set Status
            elCCM.Fill = (MessagesSet.rxMsg_3.CCMIsActive) ? Brushes.Green : Brushes.Gray;
            elVCM.Fill = (MessagesSet.rxMsg_3.VCMIsActive) ? Brushes.Green : Brushes.Gray;
            elOperate.Fill = (MessagesSet.rxMsg_3.IsOperate) ? Brushes.Green : Brushes.Gray;
            elDebug.Fill = (MessagesSet.rxMsg_3.IsDebug) ? Brushes.Yellow : Brushes.Gray;
            elClear.Fill = (MessagesSet.rxMsg_3.ClearIsActive) ? Brushes.Green : Brushes.Gray;
            elError.Fill = (MessagesSet.rxMsg_3.IsFault) ? Brushes.Red : Brushes.Gray;
            elCalib.Fill = (MessagesSet.rxMsg_3.CalibIsActive) ? Brushes.Green : Brushes.Gray;
            elNumber.Fill = (MessagesSet.rxMsg_3.NumIsActive) ? Brushes.Green : Brushes.Gray;

            // Set Faults
            el_F_Calib.Fill = (MessagesSet.rxMsg_3.F_Calib) ? Brushes.Red : Brushes.Green;
            el_F_InvOverheat.Fill = (MessagesSet.rxMsg_3.F_InvOverheat) ? Brushes.Red : Brushes.Green;
            el_F_RectOverheat.Fill = (MessagesSet.rxMsg_3.F_RectOverheat) ? Brushes.Red : Brushes.Green;
            el_F_InvTS.Fill = (MessagesSet.rxMsg_3.F_InvTS) ? Brushes.Red : Brushes.Green;
            el_F_RectTS.Fill = (MessagesSet.rxMsg_3.F_RectTS) ? Brushes.Red : Brushes.Green;
            el_F_Timeout.Fill = (MessagesSet.rxMsg_3.F_Timeout) ? Brushes.Red : Brushes.Green;
            el_F_HSDriverNotReady.Fill = (MessagesSet.rxMsg_3.F_HSNotReady) ? Brushes.Red : Brushes.Green;
            el_F_HSDriver.Fill = (MessagesSet.rxMsg_3.F_HSFault) ? Brushes.Red : Brushes.Green;
            el_F_OverVoltageIn.Fill = (MessagesSet.rxMsg_3.F_InOverVoltage) ? Brushes.Red : Brushes.Green;
            el_F_UnderVoltageIn.Fill = (MessagesSet.rxMsg_3.F_InUnderVoltage) ? Brushes.Red : Brushes.Green;
            el_F_OverCurrentIn.Fill = (MessagesSet.rxMsg_3.F_InOverCurrent) ? Brushes.Red : Brushes.Green;
            el_F_OverVoltageOut.Fill = (MessagesSet.rxMsg_3.F_OutOverVoltage) ? Brushes.Red : Brushes.Green;
            el_F_OverCurrentOut.Fill = (MessagesSet.rxMsg_3.F_OutOverCurrent) ? Brushes.Red : Brushes.Green;
            el_F_CurrentCutoff.Fill = (MessagesSet.rxMsg_3.F_CurrentCutoff) ? Brushes.Red : Brushes.Green;
            el_F_LSDriverNotReady.Fill = (MessagesSet.rxMsg_3.F_LSNotReady) ? Brushes.Red : Brushes.Green;
            el_F_LSDriver.Fill = (MessagesSet.rxMsg_3.F_LSFault) ? Brushes.Red : Brushes.Green;

            el_F_Calib_VIN_Point1.Fill = (MessagesSet.rxMsg_3.F_CalibVIN_1) ? Brushes.Red : Brushes.Green;
            el_F_Calib_VIN_Point2.Fill = (MessagesSet.rxMsg_3.F_CalibVIN_2) ? Brushes.Red : Brushes.Green;
            el_F_Calib_CIN_Point1.Fill = (MessagesSet.rxMsg_3.F_CalibCPR_1) ? Brushes.Red : Brushes.Green;
            el_F_Calib_CIN_Point2.Fill = (MessagesSet.rxMsg_3.F_CalibCPR_2) ? Brushes.Red : Brushes.Green;
            el_F_Calib_VOUT_Point1.Fill = (MessagesSet.rxMsg_3.F_CalibVOUT_1) ? Brushes.Red : Brushes.Green;
            el_F_Calib_VOUT_Point2.Fill = (MessagesSet.rxMsg_3.F_CalibVOUT_2) ? Brushes.Red : Brushes.Green;
            el_F_Calib_COUT_Point1.Fill = (MessagesSet.rxMsg_3.F_CalibCOUT_1) ? Brushes.Red : Brushes.Green;
            el_F_Calib_COUT_Point2.Fill = (MessagesSet.rxMsg_3.F_CalibCOUT_2) ? Brushes.Red : Brushes.Green;
            el_F_Calib_TINV_Point1.Fill = (MessagesSet.rxMsg_3.F_CalibTINV_1) ? Brushes.Red : Brushes.Green;
            el_F_Calib_TINV_Point2.Fill = (MessagesSet.rxMsg_3.F_CalibTINV_2) ? Brushes.Red : Brushes.Green;
            el_F_Calib_TRECT_Point1.Fill = (MessagesSet.rxMsg_3.F_CalibTRECT_1) ? Brushes.Red : Brushes.Green;
            el_F_Calib_TRECT_Point2.Fill = (MessagesSet.rxMsg_3.F_CalibTRECT_2) ? Brushes.Red : Brushes.Green;
        }

        private void StackPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                double val;
                if(Double.TryParse(tbTargetVoltage.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out val))
                    MessagesSet.txMsg.TargetVoltage = val;

                if (Double.TryParse(tbTargetCurrent.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out val))
                    MessagesSet.txMsg.MaxCurrentLimit = val;

                slFreq_ValueChanged(null, new RoutedPropertyChangedEventArgs<double>(0, 0));
            }
            else if ((e.Key < Key.D0) || (e.Key > Key.D9) && (e.Key != Key.OemPeriod))
                e.Handled = true;
        }

        private void control_Checked(object sender, RoutedEventArgs e)
        {
            byte val;
            Int16 val16;

            if (Byte.TryParse(tbNumber.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out val))
                MessagesSet.txMsg.Number = val;

            if (Int16.TryParse(tbFreq.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out val16))
                MessagesSet.txMsg.FreqDebug = val16;

            MessagesSet.txMsg.SetNumber = cbNumber.IsChecked.Value;
            MessagesSet.txMsg.OperateEnable = cbOperate.IsChecked.Value;
            MessagesSet.txMsg.ClearSettings = cbClear.IsChecked.Value;
            MessagesSet.txMsg.DebugEnable = cbDebug.IsChecked.Value;
        }

        private void slFreq_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MessagesSet != null)
            {                
                int val = Convert.ToInt16(slFreq.Maximum + slFreq.Minimum) - Convert.ToInt16(slFreq.Value);
                MessagesSet.txMsg.FreqDebug = (short)val;// tbFreq.Text);
                tbFreq.Text = MessagesSet.txMsg.FreqDebug.ToString();
            }
        }
    }
}
