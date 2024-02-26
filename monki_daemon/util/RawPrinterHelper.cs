using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MonkiDaemon.util
{
    public class RawPrinterHelper
    {
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool StartDocPrinter(
          IntPtr hPrinter,
          int level,
          [MarshalAs(UnmanagedType.LPStruct), In] RawPrinterHelper.DOCINFOA di);

        [DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern bool WritePrinter(
          IntPtr hPrinter,
          IntPtr pBytes,
          int dwCount,
          out int dwWritten);

        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, int dwCount)
        {
            int num = 0;
            int dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            RawPrinterHelper.DOCINFOA di = new RawPrinterHelper.DOCINFOA();
            bool printer = false;
            di.pDocName = "RAW Document";
            di.pDataType = "RAW";
            if (RawPrinterHelper.OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                if (RawPrinterHelper.StartDocPrinter(hPrinter, 1, di))
                {
                    if (RawPrinterHelper.StartPagePrinter(hPrinter))
                    {
                        printer = RawPrinterHelper.WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        RawPrinterHelper.EndPagePrinter(hPrinter);
                    }
                    RawPrinterHelper.EndDocPrinter(hPrinter);
                }
                RawPrinterHelper.ClosePrinter(hPrinter);
            }
            if (!printer)
                num = Marshal.GetLastWin32Error();
            return printer;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            FileStream input = new FileStream(szFileName, FileMode.Open);
            BinaryReader binaryReader = new BinaryReader((Stream)input);
            byte[] numArray = new byte[input.Length];
            IntPtr num1 = new IntPtr(0);
            int int32 = Convert.ToInt32(input.Length);
            byte[] source = binaryReader.ReadBytes(int32);
            IntPtr num2 = Marshal.AllocCoTaskMem(int32);
            Marshal.Copy(source, 0, num2, int32);
            bool printer = RawPrinterHelper.SendBytesToPrinter(szPrinterName, num2, int32);
            Marshal.FreeCoTaskMem(num2);
            input.Close();
            input.Dispose();
            return printer;
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            int length = szString.Length;
            IntPtr coTaskMemAnsi = Marshal.StringToCoTaskMemAnsi(szString);
            RawPrinterHelper.SendBytesToPrinter(szPrinterName, coTaskMemAnsi, length);
            Marshal.FreeCoTaskMem(coTaskMemAnsi);
            return true;
        }

        public static List<string> GetSerialPortNames()
        {
            List<string> source = new List<string>();
            string[] portNames = SerialPort.GetPortNames();
            if (portNames.Length != 0)
            {
                foreach (string str in portNames)
                {
                    int num = RegexUtil.LastIndexOf(str, "[0-9]");
                    if (num < 0)
                        source.Add(str);
                    else
                        source.Add(str.Substring(0, num + 1));
                }
                int portNumber = 0;
                source = source.OrderBy<string, int>((Func<string, int>)(p =>
                {
                    int.TryParse(RegexUtil.GetOnlyNumber(p), out portNumber);
                    return portNumber;
                })).ToList<string>();
            }
            return source;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
    }
}
