﻿using lab1.Constants;
using System.IO.Ports;

namespace lab1
{
    public class Node : IDisposable
    {
        protected SerialPort serialPort;

        public Node(string serialPortName, Parity parity = Parity.None) 
        {
            serialPort = new SerialPort(serialPortName, 19200, parity, 8, StopBits.One);
            serialPort.Open();

            serialPort.ReadTimeout = TransferConstants.ReadTimeout;
        }

        public virtual void Do()
        {
            Console.WriteLine($"Node starting on port {serialPort.PortName}");
        }

        public void Dispose()
        {
            serialPort.Dispose();
        }
    }
}
