using System.IO.Ports;

namespace lab1.Extensions
{
    internal static class SerialPortExtension
    {
        public static int GetPortNumber(this SerialPort serialPort)
        {
            var lastCharacter = new string(serialPort.PortName.SkipWhile(c => c <= '0' || c >= '9').ToArray());
            return int.Parse(lastCharacter);
        }
    }
}
