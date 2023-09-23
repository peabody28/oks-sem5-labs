using lab1.Builders;
using lab1.Data;
using lab1.Extensions;
using System.IO.Ports;

namespace lab1
{
    public class Producer : Node
    {
        public Producer(string serialPortName, Parity parity = Parity.None) : base(serialPortName, parity)
        {
        }

        public override void Do()
        {
            base.Do();
            while(true)
            {
                var data = Console.ReadLine();

                var packages = PackageBuilder.Build(data, serialPort.GetPortNumber());

                foreach(var package in packages)
                {
                    SimulateError(package);

                    var packageBytes = package.ToByteArray();

                    serialPort.Write(packageBytes, 0, packageBytes.Length);

                    Console.WriteLine($"{packageBytes.Length} bytes sended");
                }   
            }
        }

        private static void SimulateError(Package package)
        {
            var randomByteIndex = new Random().Next(0, package.data.Length - 1);
            var randomBitIndex = new Random().Next(0, 7);

            var randomByte = package.data[randomByteIndex];
            package.data[randomByteIndex] |= (byte)(1 << randomBitIndex);

            if((randomByte ^ package.data[randomByteIndex]) != 0)
                Console.WriteLine($"{randomByteIndex} data byte, {randomBitIndex} bit setted to 1");
        }
    }
}
