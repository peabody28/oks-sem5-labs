namespace lab1.Data
{
    internal struct Package
    {
        public byte flag;

        public byte destinationAddress;
        public byte sourceAddress;

        public byte[] data;

        public byte fcs;

        public byte[] ToByteArray()
        {
            List<byte> bytes = new() { flag, destinationAddress, sourceAddress };
            
            bytes.AddRange(data);

            bytes.Add(fcs);

            return bytes.ToArray(); 
        }

        public Package Copy()
        {
            byte[] copiedData = new byte[7];
            data.CopyTo(copiedData, 0);

            return new Package
            {
                flag = flag,
                destinationAddress = destinationAddress,
                sourceAddress = sourceAddress,
                data = copiedData,
                fcs = fcs
            };
        }
    }
}
