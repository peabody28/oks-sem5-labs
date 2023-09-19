using lab1.Builders;
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
                    var packageBytes = package.ToByteArray();

                    serialPort.Write(packageBytes, 0, packageBytes.Length);

                    Console.WriteLine($"{packageBytes.Length} bytes sended");
                }   
            }
        }
    }
}
