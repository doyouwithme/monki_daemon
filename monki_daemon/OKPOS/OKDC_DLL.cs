using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MonkiDaemon.OKPOS
{
    class OKDC_DLL
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackProcServer(int length, string data);

        [DllImport("OKDC.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CheckConnect();

        [DllImport("OKDC.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RequestPos(byte[] input_msg, byte[] output_msg, int max_output_msg_size);

        [DllImport("OKDC.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RequestPosTimeOut(byte[] input_msg, byte[] output_msg, int max_output_msg_size, int timeout_sec);

        [DllImport("OKDC.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RegServerCallback(byte[] User_id, IntPtr lpCallbackFunc);

        [DllImport("OKDC.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int UNRegServerCallback();


        public static int Dll_CheckConnect()
        {
            return CheckConnect();
        }


        public static int Dll_RequestPos(string inData, ref string pOutput)
        {
            byte[] recvArray = new byte[OKDC_INFO.BUFFER_SIZE];
            Array.Clear(recvArray, 0, OKDC_INFO.BUFFER_SIZE);

            int rc = RequestPos(Encoding.Default.GetBytes(inData), recvArray, recvArray.Length);
            if (rc > 0)
            {
                pOutput = Encoding.Default.GetString(recvArray);
            }

            GC.Collect();

            return rc;
        }


        public static int Dll_RequestPosTimeout(string inData, ref string pOutput, int timeout)
        {
            byte[] recvArray = new byte[OKDC_INFO.BUFFER_SIZE];
            Array.Clear(recvArray, 0, OKDC_INFO.BUFFER_SIZE);

            int rc = RequestPosTimeOut(Encoding.Default.GetBytes(inData), recvArray, recvArray.Length, timeout);
            if (rc > 0)
            {
                pOutput = Encoding.Default.GetString(recvArray);
            }

            GC.Collect();

            return rc;
        }


        public static int Dll_RegServerCallback(IntPtr callback)
        {
            return RegServerCallback(Encoding.Default.GetBytes(OKDC_INFO.EXTERNAL_CODE), callback);
        }


        public static int Dll_UNRegServerCallback()
        {
            return UNRegServerCallback();
        }
    }
}
