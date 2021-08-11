using System.Threading;

namespace EPloy
{
    public class ETCancellationTokenSource : IGameModule
    {
        public CancellationTokenSource CancellationTokenSource;

        public  void Awake()
        {
            CancellationTokenSource = new CancellationTokenSource();
        }

        public  void Update()
        {
            //
        }

        public  void OnDestroy()
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
            await GameModule.Timer.WaitAsync(afterTimeCancel);
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