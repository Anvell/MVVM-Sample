using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MVVMSample.Data
{

    public class FileAccessLock {
        private string lockFileName = string.Empty;
        private CancellationTokenSource CancelSource;
        public bool IsCancelled => CancelSource == null ? false : CancelSource.IsCancellationRequested;

        public async Task SetLock(string fileName) {
            lockFileName = fileName + ".lock";

            if (File.Exists(lockFileName)) {
                try {
                    CancelSource = new CancellationTokenSource();
                    await AwaitFileUnlock(lockFileName, CancelSource.Token);
                }
                catch (OperationCanceledException) {
                    throw;
                }
                finally {
                    CancelSource.Dispose();
                    CancelSource = null;
                }
            }

            using (File.Create(lockFileName)) { };
        }

        public void Cancel() => CancelSource?.Cancel();

        public void ReleaseLock() {
            if (File.Exists(lockFileName)) File.Delete(lockFileName);
        }

        private async Task AwaitFileUnlock(string lockFileName, CancellationToken cancelToken) {
            while (File.Exists(lockFileName)) {
                await Task.Delay(200, cancelToken).ConfigureAwait(false);
            }
        }
    }
}
