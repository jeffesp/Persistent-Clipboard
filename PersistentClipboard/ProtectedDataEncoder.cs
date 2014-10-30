using System.Security.Cryptography;

namespace PersistentClipboard
{
    public class ProtectedDataEncoder : IContentEncoder
    {
        private static readonly byte[] entropy = {127, 133, 211, 54, 65, 125, 183, 19, 157, 13, 70, 171, 176, 7, 251, 68};

        public byte[] Decode(byte[] contentBytes)
        {
            return ProtectedData.Unprotect(contentBytes, entropy, DataProtectionScope.CurrentUser);
        }

        public byte[] Encode(byte[] contentBytes)
        {
            return  ProtectedData.Protect(contentBytes, entropy, DataProtectionScope.CurrentUser);
        }
    }
}