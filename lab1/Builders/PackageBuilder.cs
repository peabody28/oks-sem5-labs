using lab1.Algorithms;
using lab1.Data;
using System.Text;

namespace lab1.Builders
{
    internal class PackageBuilder
    {
        private const short dataSize = 7;
        private const byte flag = (short)'z' - dataSize;

        public static IEnumerable<Package> Build(string data, int senderPortNumber)
        {
            var dataBytes = Encoding.ASCII.GetBytes(data);
            var stuffedData = GetStuffedData(dataBytes);

            var packagesCount = (stuffedData.Length - 1) / dataSize + 1;

            var packages = new List<Package>();

            for(int i = 0; i < packagesCount; i++)
            {
                var stuffedDataBytesPart = new byte[7];
                stuffedData.Skip(i * dataSize).Take(dataSize).ToArray().CopyTo(stuffedDataBytesPart, 0);

                var package = new Package();

                package.flag = flag;
                package.sourceAddress = (byte)senderPortNumber;
                package.data = stuffedDataBytesPart;
                package.fcs = new CyclicEncoding().GetCrc8(stuffedDataBytesPart);

                packages.Add(package);
            }

            return packages;
        }

        private static byte[] GetStuffedData(byte[] data)
        {
            return ByteStuffing.Encode(data, flag);
        }

        public static Package Parse(byte[] packageData)
        {
            var stuffedData = packageData.Skip(3).Take(dataSize).ToArray();
            var fcs = packageData.Skip(3 + dataSize).Take(1).FirstOrDefault();
            var package = new Package();

            package.flag = packageData[0];
            package.data = stuffedData;
            package.fcs = fcs;

            return package;
        }
    }
}
