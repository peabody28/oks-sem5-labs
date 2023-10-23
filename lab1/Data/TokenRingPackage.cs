namespace lab1.Data
{

    public class AccessControl
    {
        public bool isToken;
        public byte priority; // length = 3
        public bool isMonitor;
        public byte reservation;

        public byte ToByte()
        {
            byte acByte = 0;

            acByte = (byte)(acByte | reservation);

            if (isToken) acByte = (byte)(acByte | 0b00010000);

            if (isMonitor) acByte = (byte)(acByte | 0b00001000);

            acByte = (byte)(acByte | (priority << 5));

            return acByte;
        }

        public static AccessControl FromByte(byte accessControlByte)
        {
            var accessControl = new AccessControl();

            accessControl.priority = (byte)((accessControlByte & 0b11100000) >> 5);
            accessControl.isToken = (accessControlByte & 0b00010000) > 0;
            accessControl.isMonitor = (accessControlByte & 0b00001000) > 0;
            accessControl.reservation = (byte)(accessControlByte & 0b00000111);

            return accessControl;
        }
    }

    public class FrameStatus
    {
        public bool isRecieved;
        public bool isCopied;
        public bool reserved;

        public byte ToByte()
        {
            byte acByte = 0;

            if (isRecieved) acByte = (byte)(acByte | 0b10001000);
            if (isCopied) acByte = (byte)(acByte | 0b01000100);

            return acByte;
        }

        public static FrameStatus FromByte(byte frameStatusByte)
        {
            var frameControl = new FrameStatus();

            frameControl.isRecieved = (frameStatusByte & 0b10001000) > 0;
            frameControl.isCopied = (frameStatusByte & 0b01001000) > 0;

            return frameControl;
        }
    }

    public class TokenRingPackage : Package
    {
        public AccessControl accessControl;

        public FrameStatus frameStatus;

        public override byte[] ToByteArray()
        {
            var bytes = new List<byte>();
            var innerPackageBytes = base.ToByteArray();

            bytes.Add(accessControl?.ToByte() ?? 0);
            bytes.Add(frameStatus?.ToByte() ?? 0);

            bytes.AddRange(innerPackageBytes);

            return bytes.ToArray();
        }
    }
}
