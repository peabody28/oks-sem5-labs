using lab1.Constants;
using lab1.Data;

namespace lab1.Builders
{
    public class TokenRingPackageBuilder : PackageBuilder
    {
        public IEnumerable<Package> Build(int destination, string data, int senderPortNumber, AccessControl accessControl, FrameStatus frameStatus)
        {
            var packages = new List<TokenRingPackage>();

            var innerPackages = base.Build(destination, data, senderPortNumber);

            foreach(var innerPackage in innerPackages)
            {
                var package = FromInnerPackage(innerPackage);
                package.accessControl = accessControl;
                package.frameStatus = frameStatus;
                packages.Add(package);
            }

            packages.Last().flag = TransferConstants.LastPackageFlag;

            return packages;
        }


        public override TokenRingPackage Parse(byte[] packageData)
        {
            var innerPackage = base.Parse(packageData);
            var package = FromInnerPackage(innerPackage);

            package.accessControl = AccessControl.FromByte(packageData[0]);
            package.frameStatus = FrameStatus.FromByte(packageData[1]);

            return package;
        }

        private static TokenRingPackage FromInnerPackage(Package package)
        {
            var tokenRingPackage = new TokenRingPackage();

            tokenRingPackage.flag = package.flag;
            tokenRingPackage.destinationAddress = package.destinationAddress;
            tokenRingPackage.sourceAddress = package.sourceAddress;
            tokenRingPackage.data = package.data;
            tokenRingPackage.fcs = package.fcs;

            return tokenRingPackage;
        }
    }
}
