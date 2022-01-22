using System.Threading;

namespace DbLocalizationProvider.Sync
{
    public class ThreadSafeSingleShotFlag
    {
        private static readonly int INIT = 0;
        private static readonly int CALLED = 1;
        private int _state = INIT;

        /// <summary>Explicit call to check and set if this is the first call</summary>
        public bool CheckAndSetFirstCall => Interlocked.Exchange(ref _state, CALLED) == INIT;

        /// <summary>usually init by false</summary>
        public static implicit operator ThreadSafeSingleShotFlag(bool called)
        {
            return new ThreadSafeSingleShotFlag { _state = called ? CALLED : INIT };
        }

        /// <summary>
        /// if init to false, returns true with the first call, then always false -
        /// if init to true, always returns false.
        /// </summary>
        public static bool operator !(ThreadSafeSingleShotFlag obj)
        {
            return obj.CheckAndSetFirstCall;
        }
    }
}
