using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using FTDI_CAN;
using DC_Debug.Devices;

namespace DC_Debug
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        DispatcherTimer updateForm;
        ICanDriver canDEDriver;
        ICanMessage txMsg;
        CanMessages MessagesSet;

        List<uint> ModulesNumbers = new List<uint>(8);

        public StartWindow()
        {
            InitializeComponent();

            this.Loaded += StartWindow_Loaded;
        }

        private void StartWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MessagesSet = new CanMessages();

            canDEDriver = new UCan();
            canDEDriver.OnReadMessage += CanDEDriver_OnReadMessage;

            canDEDriver.Start(0, 500);
            canDEDriver.SetDiagnosticMode(false);
        }

        private void CanDEDriver_OnReadMessage(CAN_Message msg)
        {
            if (!msg.Ext && msg.id != 0)
            {
                uint module_id = msg.id & 0xFF;

                if (!ModulesNumbers.Contains(module_id))
                    ModulesNumbers.Add(module_id);
            }
        }
    }
}
