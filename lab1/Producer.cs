using lab1.Builders;
using lab1.Constants;
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

                var packages = PackageBuilder.Build(data, serialPort.GetPortNumber()).ToArray();

                for (int i = 0; i < packages.Count(); i++)
                {
                    var targetPackage = packages[i];

                    var package = SimulateError(targetPackage);

                    var packageBytes = package.ToByteArray();

                    var emulateBusy = new Random().Next(0, 1).Equals(1);

                    if (emulateBusy)
                        Thread.Sleep(CollisionConstants._busyDelay);

                    serialPort.Write(packageBytes, 0, packageBytes.Length);

                    Console.WriteLine($"{packageBytes.Length} bytes sended");

                    var emulateСollision = new Random().Next(0, 4).Equals(1);
                    if (emulateСollision)
                    {
                        Console.WriteLine($"Collision! Sending JAM-package...");

                        var jamPackageBytes = PackageBuilder.BuildJam().ToByteArray();

                        serialPort.Write(jamPackageBytes, 0, jamPackageBytes.Length);

                        Thread.Sleep(CollisionConstants._collisionDelay);
                        i--;
                    }
                }
            }
        }

        private static Package SimulateError(Package package)
        {
            var brokenPackage = package.Copy();

            var randomByteIndex = new Random().Next(0, brokenPackage.data.Length - 1);
            var randomBitIndex = new Random().Next(0, 7);

            var randomByte = brokenPackage.data[randomByteIndex];

            brokenPackage.data[randomByteIndex] |= (byte)(1 << randomBitIndex);

            if ((randomByte ^ brokenPackage.data[randomByteIndex]) != 0)
                Console.WriteLine($"{randomByteIndex} data byte, {randomBitIndex} bit setted to 1");

            return brokenPackage;
        }
    }
}
