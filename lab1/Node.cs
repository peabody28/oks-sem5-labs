using lab1.Algorithms;
using lab1.Builders;
using lab1.Constants;
using lab1.Data;
using lab1.Extensions;
using System.IO.Ports;
using System.Text;

namespace lab1
{
    public class Node : IDisposable
    {
        private readonly SerialPort writePort;
        private readonly SerialPort readPort;

        private readonly List<TokenRingPackage> readedPackages;
        private readonly Queue<TokenRingPackage> packagesQueue;

        private readonly TokenRingPackageBuilder tokenRingPackageBuilder;

        private readonly bool isMonitor;

        private byte priority = 1;

        public Node(string writePortName, string readPortName, bool _isMonitor, Parity parity = Parity.None) 
        {
            tokenRingPackageBuilder = new TokenRingPackageBuilder();
            readedPackages = new List<TokenRingPackage>();
            packagesQueue = new Queue<TokenRingPackage>();
            isMonitor = _isMonitor;

            writePort = new SerialPort(writePortName, 19200, parity, 8, StopBits.One);
            writePort.Open();

            readPort = new SerialPort(readPortName, 19200, parity, 8, StopBits.One);
            readPort.DataReceived += new SerialDataReceivedEventHandler(DataReaded);
            readPort.Open();

            Console.WriteLine($"Node starting writer: {writePortName}, reader: {readPortName}");
        }

        #region [ Reader ]

        public void DataReaded(object sender, SerialDataReceivedEventArgs e)
        {
            var buffer = new byte[TransferConstants.PackageLen];

            readPort.Read(buffer, 0, TransferConstants.PackageLen);
            
            var recievedPackage = tokenRingPackageBuilder.Parse(buffer);
            TokenRingPackage packageToSend = recievedPackage;

            var isToken = recievedPackage.accessControl.isToken;
            var imInReservation = priority >= recievedPackage.accessControl.reservation;
            var imInPriority = priority >= recievedPackage.accessControl.priority;

            if (!isToken)
            {
                // package for me
                if (recievedPackage.destinationAddress.Equals((byte)readPort.GetPortNumber()))
                {
                    readedPackages.Add(recievedPackage);

                    packageToSend.frameStatus.isRecieved = true;
                    packageToSend.frameStatus.isCopied = true;

                    if (recievedPackage.flag.Equals(TransferConstants.LastPackageFlag)) // end of data packages
                    {
                        var data = GetData(readedPackages);
                        Console.WriteLine(data);
                        readedPackages.Clear();
                    }

                    WritePackage(packageToSend);
                    return;
                }

                // im sender
                if (recievedPackage.sourceAddress.Equals((byte)writePort.GetPortNumber()))
                {
                    // remake token
                    packageToSend.accessControl.isToken = true;
                    packageToSend.accessControl.priority = recievedPackage.accessControl.reservation;
                    packageToSend.accessControl.reservation = 0;

                    WritePackage(packageToSend);
                    return;
                }

                // проходной for frame
                if (packagesQueue.Any() && imInReservation)
                {
                    packageToSend.accessControl.reservation = priority;
                }

                WritePackage(packageToSend);
                return;
            }
            else if(packagesQueue.Any())
            {
                // token
                if (imInPriority)
                {
                    var newPackage = packagesQueue.Dequeue();
                    newPackage.accessControl = new AccessControl()
                    {
                        isToken = false,
                        priority = priority,
                        reservation = 0
                    };
                    newPackage.frameStatus = new FrameStatus();

                    packageToSend = newPackage;
                }
                else if (imInReservation)
                    recievedPackage.accessControl.reservation = priority;
            }
                
            WritePackage(packageToSend);
            return;
        }

        private static string GetData(IEnumerable<Package> packages)
        {
            Console.WriteLine("Packages count:" + packages.Count());
            var dataBytes = new List<byte>();

            foreach (var package in packages)
            {
                //RepairPackageIfRequired(package);

                dataBytes.AddRange(package.data);
            }

            //Log(dataBytes.ToArray());

            var unstaffedData = ByteStuffing.Decode(dataBytes.ToArray());

            return Encoding.ASCII.GetString(unstaffedData);
        }

        private static void RepairPackageIfRequired(Package package)
        {
            var alg = new CyclicEncoding();
            var expectedFcs = alg.GetCrc8(package.data);

            Console.WriteLine($"Expected fcs = {expectedFcs}, package.fcs = {package.fcs}");

            if (!expectedFcs.Equals(package.fcs) && alg.TryFindErrorBitNumber(package.data, package.fcs, out var errorIndex))
            {
                package.data[errorIndex.Item1] ^= (byte)(1 << errorIndex.Item2);

                Console.WriteLine($"Error on ({errorIndex.Item1}, {errorIndex.Item2}) fixed");
            }
        }

        private static void Log(byte[] data)
        {
            Console.Write("Staffed data is: ");

            foreach (var b in data)
            {
                Console.Write(b);
                Console.Write(" ");
            }

            Console.WriteLine();
        }

        #endregion


        #region [ Writer ]

        public void Produce()
        {
            Task.Run(() => ProcessQueue());

            if (isMonitor)
                Task.Run(() => Monitoring());

            while (true)
            {
                Console.Write("input destination: ");
                var destination = Convert.ToInt32(Console.ReadLine());
                var data = Console.ReadLine();

                var frameStatus = new FrameStatus();
                var accessControl = new AccessControl();
                var packages = tokenRingPackageBuilder.Build(destination, data, writePort.GetPortNumber(), accessControl, frameStatus);

                foreach (var package in packages)
                {
                    packagesQueue.Enqueue(package);
                }
            }
        }

        private void ProcessQueue()
        {
            while (true)
            {
                if (packagesQueue.Any())
                {
                    var package = packagesQueue.Dequeue();

                    //SimulateError(package);

                    var packageBytes = package.ToByteArray();

                    writePort.Write(packageBytes, 0, packageBytes.Length);

                    if (!package.accessControl?.isToken ?? true)
                        Console.WriteLine($"{packageBytes.Length} bytes sended");
                }
            }
        }

        private void Monitoring()
        {
            while (true)
            {
                var accessControl = new AccessControl();
                accessControl.isMonitor = isMonitor;
                accessControl.isToken = true;

                var frameStatus = new FrameStatus();
                var tokenPackage = tokenRingPackageBuilder.Build(0, string.Empty, writePort.GetPortNumber(), accessControl, frameStatus).First();

                WritePackage(tokenPackage);

                Thread.Sleep(5000);
            }
        }

        private static void SimulateError(Package package)
        {
            var randomByteIndex = new Random().Next(0, package.data.Length - 1);
            var randomBitIndex = new Random().Next(0, 7);

            var randomByte = package.data[randomByteIndex];
            package.data[randomByteIndex] |= (byte)(1 << randomBitIndex);

            if ((randomByte ^ package.data[randomByteIndex]) != 0)
                Console.WriteLine($"{randomByteIndex} data byte, {randomBitIndex} bit setted to 1");
        }

        #endregion

        private void WritePackage(TokenRingPackage package)
        {
            var packageBytes = package.ToByteArray();

            writePort.Write(packageBytes, 0, packageBytes.Length);

            if(!package.accessControl.isToken)
                Console.WriteLine($"{packageBytes.Length} bytes sended");
        }

        public void Dispose()
        {
            writePort.Dispose();
            readPort.Dispose();
        }
    }
}
