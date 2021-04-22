using System;
using UnityEngine.Networking;

namespace EPloy
{
    public partial class DownloadAgent : IDisposable
    {
        private sealed class DownloadHandler : DownloadHandlerScript
        {
            private readonly DownloadAgent Owner;
            public DownloadHandler(DownloadAgent owner)
                : base(owner.CachedBytes)
            {
                Owner = owner;
            }

            protected override bool ReceiveData(byte[] data, int dataLength)
            {
                if (Owner != null && Owner.UnityWebRequest != null && dataLength > 0)
                {
                    Owner.OnDownloadUpdateBytes(0, dataLength, data);
                    Owner.OnDownloadUpdateLength(dataLength);
                }
                return base.ReceiveData(data, dataLength);
            }
        }
    }
}
