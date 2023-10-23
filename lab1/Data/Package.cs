namespace lab1.Data
{
    public class Package
    {
        public byte flag;

        public byte destinationAddress;
        public byte sourceAddress;

        public byte[] data;

        public byte fcs;

        public virtual byte[] ToByteArray()
        {
            List<byte> bytes = new() { flag, destinationAddress, sourceAddress };
            
            bytes.AddRange(data);

            bytes.Add(fcs);

            return bytes.ToArray(); 
        }
    }
}
