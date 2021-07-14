using System.Threading;

namespace EPloy
{
    public class ETCancellationTokenSource : EPloyModule
    {
        public CancellationTokenSource CancellationTokenSource;

        public override void Awake()
        {
            CancellationTokenSource = new CancellationTokenSource();
        }

        public override void Update()
        {
            //
        }

        public override void OnDestroy()
        {
            this.CancellationTokenSource?.Dispose();
            this.CancellationTokenSource = null;
        }

        public void Cancel()
        {
            this.CancellationTokenSource.Cancel();
            OnDestroy();
        }

        public async ETVoid CancelAfter(long afterTimeCancel)
        {
            await Game.Timer.WaitAsync(afterTimeCancel);
            this.CancellationTokenSource.Cancel();
            OnDestroy();
        }

        public CancellationToken Token
        {
            get
            {
                return this.CancellationTokenSource.Token;
            }
        }

    }
}