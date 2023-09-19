namespace lab1.Algorithms
{
    internal static class ByteStuffing
    {
        private const byte endByte = 0;
        private const byte escapeByte = 0x20;
        private const byte xorMask = 0x30;

        public static byte[] Encode(byte[] data, byte staffedByte)
        {
            var stuffedData = new List<byte>();

            foreach (var b in data)
            {
                byte byteToInsert = b;

                if(b.Equals(staffedByte))
                {
                    stuffedData.Add(escapeByte);
                    byteToInsert ^= xorMask;
                }

                stuffedData.Add(byteToInsert);
            }

            stuffedData.Add(endByte);

            return stuffedData.ToArray();
        }

        public static byte[] Decode(byte[] data)
        {
            var destuffedData = new List<byte>();

            for(int i = 0; i < data.Length && data[i] != endByte; i++)
            {
                byte byteToInsert = data[i];

                if (byteToInsert.Equals(escapeByte))
                    byteToInsert = (byte)(data[++i] ^ xorMask);
                
                destuffedData.Add(byteToInsert);
            }

            return destuffedData.ToArray();
        }
    }
}
