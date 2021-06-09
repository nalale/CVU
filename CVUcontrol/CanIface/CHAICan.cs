using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CHAI;

namespace FTDI_CAN
{
    public class ErrorDescription
    {
        static public short Code;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        static public char[] errorDesc = new char[16];
    }
    class CHAICan : ICanDriver
    {
        CancellationTokenSource cancelSource = new CancellationTokenSource();
        
        public CHAICan()
        {
            Task.Factory.StartNew(check_thread);
        }

        public int BitRate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int CanLoadPercent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int CanLoadTx { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int CanLoadRx { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Model { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort HW_Version { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort SW_Version { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsConnected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint TxCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint RxCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event dCANDataRead OnReadMessage;

        public bool SendMessage(ICanMessage msg)
        {
            throw new NotImplementedException();
        }

        public void SetDiagnosticMode(bool val)
        {
            throw new NotImplementedException();
        }

        public void SetR120(bool val)
        {
            throw new NotImplementedException();
        }

        public bool SetSpeed(int bitRate)
        {
            throw new NotImplementedException();
        }

        public bool Start(int channel, int bitRate)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        #region Private

        List<CiCANboard> ciBoards = new List<CiCANboard>(2);        

        private void check_thread()
        {
            bool div = false;

            while (!cancelSource.Token.IsCancellationRequested)
            {
                div = !div;
                string s = "";

                if (div)    // 1 раз в секунду
                {
                    
                    CiCANboard ciBoards = new CiCANboard();
                    ciBoards.brdnum = 0;
                    // Initialize unmanged memory to hold the struct.
//                    IntPtr param = Marshal.AllocHGlobal(Marshal.SizeOf(ciBoards));                                  
//                    Marshal.StructureToPtr<CiCANboard>(ciBoards, param, false);
                    ErrorDescription.Code = CHAIDriver.CiBoardInfo(ref ciBoards);
                    ErrorDescription.errorDesc[0] = 'e';
                    if (ErrorDescription.Code < 0)
                    {
                        CHAIDriver.CiStrError(ErrorDescription.Code, ErrorDescription.errorDesc, 16);
                    }
                    //ciBoards = Marshal.PtrToStructure<CiCANboard>(param);
                    //Marshal.FreeHGlobal(param);
                }

                Task.Delay(500).Wait();
//                Thread.Sleep(500);
            }
        }

        #endregion
    }
}
