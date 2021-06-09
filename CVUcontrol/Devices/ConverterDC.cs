using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace DC_Debug.Devices
{    
    class ConverterDC : IPowerConverter
    {
        private CanMessages messages;

        double IPowerConverter.GetInVoltage { get => messages.rxMsg_1.InputVoltage;  }
        double IPowerConverter.GetInCurrent { get => messages.rxMsg_1.PrimaryCurrent; }
        double IPowerConverter.GetOutVoltage { get => messages.rxMsg_1.OutputVoltage;  }
        double IPowerConverter.GetOutCurrent { get => messages.rxMsg_1.OutputCurrent; }
        int IPowerConverter.GetTemperature { get => messages.rxMsg_2.InvTemp; }
        int IPowerConverter.GetTemperature2 { get => messages.rxMsg_2.RectTemp; }

        int IPowerConverter.ParseIncomeMessage(int MessageID, byte[] Data)
        {
            switch(MessageID)
            {
                case 0:
                    messages.rxMsg_3.SetMsgData(Data);
                    break;

                case 0x200:
                    messages.rxMsg_1.SetMsgData(Data);
                    break;

                case 0x300:
                    messages.rxMsg_2.SetMsgData(Data);
                    break;

                default:
                    return 1;
            }

            return 0;
        }
    }

    class CanMessages
    {
        public TxMsg txMsg = new TxMsg();
        public RxMsg1 rxMsg_1 = new RxMsg1();
        public RxMsg2 rxMsg_2 = new RxMsg2();
        public RxMsg3 rxMsg_3 = new RxMsg3();

        public class TxMsg
        {
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            struct Data
            {
                public short TargetVoltage;
                public byte MaxCurrent;
                public byte State;
                public short Freq;
                public byte Cmd;
                public byte Number;
            }

            Data msgData;
            public Double TargetVoltage
            {
                get
                {
                    byte[] data = BitConverter.GetBytes(msgData.TargetVoltage);
                    short val = BitConverter.ToInt16(new byte[] { data[1], data[0] }, 0);
                    return val / 10;
                }
                set
                {
                    short val = Convert.ToInt16(value * 10);
                    byte[] data = BitConverter.GetBytes(val);
                    msgData.TargetVoltage = BitConverter.ToInt16(new byte[] { data[1], data[0] }, 0);
                }
            }
            public Double MaxCurrentLimit
            {
                get => (double)msgData.MaxCurrent;
                set
                {
                    if (value > 160)
                        msgData.MaxCurrent = 160;
                    else if (value < 25)
                        msgData.MaxCurrent = 25;
                    else
                        msgData.MaxCurrent = (byte)value;
                }
            }
            public bool OperateEnable
            {
                get => Convert.ToBoolean(msgData.State & 0x01);
                set
                {
                    if (value == true)
                        msgData.State |= 1;
                    else
                        msgData.State &= 0xfe;
                }
            }
            public bool DebugEnable
            {
                get => Convert.ToBoolean(msgData.State & 0x02);
                set
                {
                    if (value == true)
                        msgData.State |= 2;
                    else
                        msgData.State &= 0xfd;
                }
            }
            public short FreqDebug
            {
                get
                {
                    byte[] data = BitConverter.GetBytes(msgData.Freq);
                    return BitConverter.ToInt16(new byte[] { data[1], data[0] }, 0);

                }
                set
                {
                    int freq = value;
                    if (value > 500)
                        freq = 500;
                    else if (value < 80)
                        freq = 80;

                    byte[] data = BitConverter.GetBytes(freq);
                    msgData.Freq = BitConverter.ToInt16(new byte[] { data[1], data[0] }, 0);
                }
            }
            public bool SetNumber
            {
                get => Convert.ToBoolean(msgData.Cmd & 0x01);
                set
                {
                    if (value == true)
                        msgData.Cmd |= 1;
                    else
                        msgData.Cmd &= 0xfe;
                }
            }
            public bool ClearSettings
            {
                get => Convert.ToBoolean(msgData.Cmd & 0x02);
                set
                {
                    if (value == true)
                        msgData.Cmd |= 2;
                    else
                        msgData.Cmd &= 0xfd;
                }
            }
            public byte Number
            {
                get => msgData.Number;
                set
                {
                    if (value > 15)
                        msgData.Number = 15;
                    else if (value < 1)
                        msgData.Number = 1;
                    else
                        msgData.Number = value;
                }
            }

            public byte[] GetMsgData()
            {
                int size = Marshal.SizeOf(msgData);
                byte[] arr = new byte[size];

                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(msgData, ptr, true);
                Marshal.Copy(ptr, arr, 0, size);
                Marshal.FreeHGlobal(ptr);
                return arr;
            }

            public int GetMsgId()
            {
                return 0x100;
            }
        }

        public class RxMsg1
        {
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            struct Data
            {
                public short InputVoltage;
                public short PrimaryCurrent;
                public short OutputVoltage;
                public short OutputCurrent;
            }

            Data msgData;
            public float InputVoltage
            {
                get
                {
                    byte[] data = BitConverter.GetBytes((short)msgData.InputVoltage);
                    short val = BitConverter.ToInt16(new byte[] { data[1], data[0] }, 0);
                    return (float)val / 10;
                }
                set
                {
                    msgData.InputVoltage = (short)value;
                }
            }
            public float PrimaryCurrent
            {
                get
                {
                    byte[] data = BitConverter.GetBytes((short)msgData.PrimaryCurrent);
                    short val = BitConverter.ToInt16(new byte[] { data[1], data[0] }, 0);
                    return (float)val / 10;
                }
                set
                {
                    msgData.PrimaryCurrent = (short)value;
                }
            }
            public float OutputVoltage
            {
                get
                {
                    byte[] data = BitConverter.GetBytes((short)msgData.OutputVoltage);
                    short val = BitConverter.ToInt16(new byte[] { data[1], data[0] }, 0);
                    return (float)val / 10;
                }
                set
                {
                    msgData.OutputVoltage = (short)value;
                }
            }
            public float OutputCurrent
            {
                get
                {
                    byte[] data = BitConverter.GetBytes((short)msgData.OutputCurrent);
                    short val = BitConverter.ToInt16(new byte[] { data[1], data[0] }, 0);
                    return (float)val / 10;
                }
                set
                {
                    msgData.OutputCurrent = (short)value;
                }
            }

            public void SetMsgData(byte[] data)
            {
                int size = Marshal.SizeOf(msgData);
                IntPtr ptr = Marshal.AllocHGlobal(size);

                Marshal.Copy(data, 0, ptr, size);

                msgData = (Data)Marshal.PtrToStructure(ptr, msgData.GetType());
                Marshal.FreeHGlobal(ptr);
            }
        }
        public class RxMsg2
        {
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            struct Data
            {
                public short InputPower;
                public short OutputPower;
                public short Efficiency;
                public sbyte InvTemp;
                public sbyte RectTemp;
            }

            Data msgData;
            public float InputPower
            {
                get
                {
                    byte[] data = BitConverter.GetBytes((short)msgData.InputPower);
                    short val = BitConverter.ToInt16(new byte[] { data[1], data[0] }, 0);
                    return (float)val / 10;
                }
                set
                {
                    msgData.InputPower = (short)value;
                }
            }
            public float OutputPower
            {
                get
                {
                    byte[] data = BitConverter.GetBytes((short)msgData.OutputPower);
                    short val = BitConverter.ToInt16(new byte[] { data[1], data[0] }, 0);
                    return (float)val / 10;
                }
                set
                {
                    msgData.OutputPower = (short)value;
                }
            }
            public float Efficiency
            {
                get
                {
                    byte[] data = BitConverter.GetBytes((short)msgData.Efficiency);
                    short val = BitConverter.ToInt16(new byte[] { data[1], data[0] }, 0);
                    return (float)val / 10;
                }
                set
                {
                    msgData.Efficiency = (short)value;
                }
            }
            public sbyte InvTemp
            {
                get => (sbyte)(msgData.InvTemp - 80);
                set => msgData.InvTemp = value;
            }
            public sbyte RectTemp
            {
                get => (sbyte)(msgData.RectTemp - 80);
                set => msgData.RectTemp = value;
            }

            public void SetMsgData(byte[] data)
            {
                int size = Marshal.SizeOf(msgData);
                IntPtr ptr = Marshal.AllocHGlobal(size);

                Marshal.Copy(data, 0, ptr, size);

                msgData = (Data)Marshal.PtrToStructure(ptr, msgData.GetType());
                Marshal.FreeHGlobal(ptr);
            }
        }
        public class RxMsg3
        {
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            struct Data
            {
                public short FaultState1;
                public short FaultState2;
                public short FaultState3;
                public byte Mode;
                public byte Status;
            }

            Data msgData;

            public bool F_Calib => Convert.ToBoolean(msgData.FaultState1 & 0x01);
            public bool F_InvOverheat => Convert.ToBoolean(msgData.FaultState1 & 0x02);
            public bool F_RectOverheat => Convert.ToBoolean(msgData.FaultState1 & 0x04);
            public bool F_InvTS => Convert.ToBoolean(msgData.FaultState1 & 0x08);
            public bool F_RectTS => Convert.ToBoolean(msgData.FaultState1 & 0x10);
            public bool F_Timeout => Convert.ToBoolean(msgData.FaultState1 & 0x20);
            public bool F_InOverVoltage => Convert.ToBoolean(msgData.FaultState1 & 0x100);
            public bool F_InUnderVoltage => Convert.ToBoolean(msgData.FaultState1 & 0x200);
            public bool F_InOverCurrent => Convert.ToBoolean(msgData.FaultState1 & 0x400);
            public bool F_OutOverVoltage => Convert.ToBoolean(msgData.FaultState1 & 0x800);
            public bool F_OutOverCurrent => Convert.ToBoolean(msgData.FaultState1 & 0x1000);
            public bool F_HSNotReady => Convert.ToBoolean(msgData.FaultState2 & 0x01);
            public bool F_HSFault => Convert.ToBoolean(msgData.FaultState2 & 0x02);
            public bool F_LSNotReady => Convert.ToBoolean(msgData.FaultState2 & 0x04);
            public bool F_LSFault => Convert.ToBoolean(msgData.FaultState2 & 0x08);
            public bool F_CurrentCutoff => Convert.ToBoolean(msgData.FaultState2 & 0x10);
            public bool F_CalibVIN_1 => Convert.ToBoolean(msgData.FaultState3 & 0x01);
            public bool F_CalibCPR_1 => Convert.ToBoolean(msgData.FaultState3 & 0x02);
            public bool F_CalibVOUT_1 => Convert.ToBoolean(msgData.FaultState3 & 0x04);
            public bool F_CalibCOUT_1 => Convert.ToBoolean(msgData.FaultState3 & 0x08);
            public bool F_CalibTINV_1 => Convert.ToBoolean(msgData.FaultState3 & 0x10);
            public bool F_CalibTRECT_1 => Convert.ToBoolean(msgData.FaultState3 & 0x20);
            public bool F_CalibVIN_2 => Convert.ToBoolean(msgData.FaultState3 & 0x100);
            public bool F_CalibCPR_2 => Convert.ToBoolean(msgData.FaultState3 & 0x200);
            public bool F_CalibVOUT_2 => Convert.ToBoolean(msgData.FaultState3 & 0x400);
            public bool F_CalibCOUT_2 => Convert.ToBoolean(msgData.FaultState3 & 0x800);
            public bool F_CalibTINV_2 => Convert.ToBoolean(msgData.FaultState3 & 0x1000);
            public bool F_CalibTRECT_2 => Convert.ToBoolean(msgData.FaultState3 & 0x2000);

            public bool VCMIsActive => Convert.ToBoolean(msgData.Mode & 0x1);
            public bool CCMIsActive => Convert.ToBoolean(msgData.Mode & 0x2);
            public bool NumIsActive => Convert.ToBoolean(msgData.Mode & 0x4);
            public bool CalibIsActive => Convert.ToBoolean(msgData.Mode & 0x8);
            public bool ClearIsActive => Convert.ToBoolean(msgData.Mode & 0x10);

            public bool IsOperate => Convert.ToBoolean(msgData.Status & 0x1);
            public bool IsFault => Convert.ToBoolean(msgData.Status & 0x2);
            public bool IsDebug => Convert.ToBoolean(msgData.Status & 0x4);

            public void SetMsgData(byte[] data)
            {
                int size = Marshal.SizeOf(msgData);
                IntPtr ptr = Marshal.AllocHGlobal(size);

                Marshal.Copy(data, 0, ptr, size);

                msgData = (Data)Marshal.PtrToStructure(ptr, msgData.GetType());
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
