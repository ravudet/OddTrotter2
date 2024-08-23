using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading
{
    /// <summary>
    /// TODO check chat from 08/19 to add details
    /// </summary>
    public sealed class ShareableLock //// TODO better name
    {
        private int categoryLock; //// TODO readWriteLock

        private int alignedCount; //// TODO readCount

        public ShareableLock()
        {
            this.categoryLock = 0;
            this.alignedCount = 1; //// TODO starting at 1 is *very* important
        }

        public void EnterReadLock()
        {
            Interlocked.Increment(ref this.alignedCount);
            while (true)
            {
                if (Interlocked.CompareExchange(ref this.categoryLock, 1, 0) == 0)
                {
                    // no one has the lock yet, be the first aligned test to take it
                    break;
                }
                else if (Interlocked.CompareExchange(ref this.categoryLock, 1, 1) == 1)
                {
                    // another aligned test has the lock, increment the count of aligned tests
                    break;
                }

                // the lock is acquired by non-aligned tests, wait until they are done
                //// TODO spinwait?
            }
        }

        public void ExitReadLock()
        {
            Interlocked.Decrement(ref this.alignedCount);
            //// TODO alignedcount might need to be volatile because of this non-reference read...
            if (this.alignedCount == 1) //// TODO do you need this if statement? don't the below cases still work even if you don't do this check?
            {
                // if the lock is 0, then another aligned test already released the lock, so we don't need to release it
                // if the lock is -1, then another aligned test already released the lock and allowed a non-aligned test to acquire it, meaning we don't need to release the lock
                // if the lock is 1, then we either succeed in releasing because alignedcount has remained 1 (meaning we don't need to re-release), or alignedcount is more than 1 because another aligned test has acquired the lock (meaning that we *shouldn't* release the lock)
                Interlocked.CompareExchange(ref this.categoryLock, 0, this.alignedCount);
            }
        }

        public void EnterWriteLock()
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref this.categoryLock, -1, 0) == 0)
                {
                    break;
                }
            }
        }

        public void ExitWriteLock()
        {
            Interlocked.Exchange(ref this.categoryLock, 0);
        }
    }
}
