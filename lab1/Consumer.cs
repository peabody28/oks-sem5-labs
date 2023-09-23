using lab1.Algorithms;
using lab1.Builders;
using lab1.Data;
using System.IO.Ports;
using System.Text;

namespace lab1
{
    public class Consumer : Node
    {
        public Consumer(string serialPortName) : base(serialPortName)
        {
            serialPort.DataReceived += new SerialDataReceivedEventHandler(OutputData);
        }

        public override void Do()
        {
            base.Do();
            while(true)
                ;
        }

        private void OutputData(object sender, SerialDataReceivedEventArgs e)
        {
            var buffer = new byte[11];

            var packages = new List<Package>();
            try
            {
                while (serialPort.Read(buffer, 0, 11) != 0)
                {
                    var package = PackageBuilder.Parse(buffer);
                    packages.Add(package);
                }
            }
            catch(TimeoutException)
            {
                var data = GetData(packages);

                Console.WriteLine(data);
            }
        }

        private string GetData(IEnumerable<Package> packages)
        {
            Console.WriteLine("Packages count:" + packages.Count());
            var dataBytes = new List<byte>();

            foreach (var package in packages)
            {
                RepairPackageIfRequired(package);

                dataBytes.AddRange(package.data);
            }

            Log(dataBytes.ToArray());

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
    }
}
