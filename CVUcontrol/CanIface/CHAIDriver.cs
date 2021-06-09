using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CHAI
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class CiCANMsg
    {
        public UInt32 id;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] data = new byte[8];
        public Byte len;
        public UInt16 flags;
        public UInt32 ts;
    };
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class CiCANboard
    {
        public Byte brdnum; /* номер платы (от 0 до CI_BRD_NUMS-1)*/
        public UInt32 hwver; /* номер версии железа (аналогичен по структуре номеру версии библиотеки */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Int16[] chip = new Int16[4]; /* массив номеров каналов (например chip[0] содержит номер канала к которому привязан первый чип платы, если номер <0 - чип отсутствует)*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] name = new byte[64]; /* текстовая строка названия платы 64*/
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public Byte[] manufact = new byte[64]; /* текстовая строка - имя производителя 64*/
    };

class CHAIDriver
    {
        /*typedef struct 
        {
            _u32 id; 
            _u8 data[8]; 
            _u8 len; 
            _u16 flags; // bit 0 - RTR, bit 2 – EFF    
            _u32 ts; 
        }
        canmsg_t;*/

        [DllImport("chai.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern Int16 CiInit();
        
        [DllImport("chai.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern Int16 CiOpen(Byte Channel, Byte flags);

        [DllImport("chai.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern Int16 CiClose(Byte Channel);

        [DllImport("chai.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern Int16 CiStart(Byte Channel);

        [DllImport("chai.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern Int16 CiSetBaud(Byte Channel, Byte bto0, Byte bto1);

        [DllImport("chai.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern Int16 CiTransmit(Byte Channel, IntPtr msgBuf);

        [DllImport("chai.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern Int16 CiRead(Byte Channel, IntPtr msgBuf, Int16 cnt);

        [DllImport("chai.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern Int16 CiBoardInfo(ref CiCANboard boardInfo);

        [DllImport("chai.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern void CiStrError(Int16 cierrno,
            [In, MarshalAs(UnmanagedType.LPArray, SizeConst = 16)]
            char[] buf, 
            Int16 n);

    }
}
