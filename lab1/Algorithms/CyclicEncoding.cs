namespace lab1.Algorithms
{
    public class CyclicEncoding
    {
        private readonly uint polynomial = 0x31;

        public byte GetCrc8(byte[] data)
        {
            byte crc = 0xFF;

            for(int i = 0; i < data.Length; i++)
            {
                crc ^= data[i];

                for (int j = 0; j < 8; j++)
                    crc = (byte)((crc & 0x80) != 0 ? (crc << 1) ^ polynomial : crc << 1);
            }

            return crc;
        }

        public bool TryFindErrorBitNumber(byte[] data, byte receivedCrc, out (int, int) errorIndex)
        {
            errorIndex = (-1, -1);

            for (int byteIndex = 0; byteIndex < data.Length; byteIndex++)
            {
                for (int bitIndex = 7; bitIndex >= 0; bitIndex--)
                {
                    data[byteIndex] ^= (byte)(1 << bitIndex);

                    var crc = GetCrc8(data);

                    data[byteIndex] ^= (byte)(1 << bitIndex);

                    if (crc.Equals(receivedCrc))
                    {
                        errorIndex = (byteIndex, bitIndex);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
