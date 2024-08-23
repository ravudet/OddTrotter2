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
    public sealed class ShareableLock
    {
        private static class CategoryLockValues
        {
            public const int ExclusiveLockTaken = -1;
            public const int NoLockTaken = 0;
            public const int SharedLockTaken = 1;
        }

        private int categoryLock; //// TODO readWriteLock

        private int sharedLockCount;

        public ShareableLock()
        {
            this.categoryLock = CategoryLockValues.NoLockTaken;
            this.sharedLockCount = 1; //// TODO starting at 1 is *very* important
        }

        public void EnterReadLock() //// TODO entershared
        {
            Interlocked.Increment(ref this.sharedLockCount);
            while (true)
            {
                if (Interlocked.CompareExchange(ref this.categoryLock, CategoryLockValues.SharedLockTaken, 0) == 0)
                {
                    // no one has the lock yet, be the first aligned test to take it
                    break;
                }
                else if (Interlocked.CompareExchange(ref this.categoryLock, CategoryLockValues.SharedLockTaken, 1) == 1)
                {
                    // another aligned test has the lock, increment the count of aligned tests
                    break;
                }

                // the lock is acquired by non-aligned tests, wait until they are done
                //// TODO thread.sleep or spinlock
                //// TODO you should probably have a separate class if async is needed
            }
        }

        public void ExitReadLock()
        {
/*
current 2 shareds
both exit "At the same time"
thread a: decrement to 2
thread b: decrement to 1
thread a: if 1, give up shared lock, so it's given up
thread b: if 1, give up shared lock; this needs to not break anything that's happened since thread a gave up the lock
*/

            Interlocked.Decrement(ref this.sharedLockCount);
            //// TODO sharedLockCount might need to be volatile because of this non-reference read...

            // if the lock is 0, then another aligned test already released the lock, so we don't need to release it
            // if the lock is -1, then another aligned test already released the lock and allowed a non-aligned test to acquire it, meaning we don't need to release the lock
            // if the lock is 1, then we either succeed in releasing because alignedcount has remained 1 (meaning we don't need to re-release), or alignedcount is more than 1 because another aligned test has acquired the lock (meaning that we *shouldn't* release the lock)
            Interlocked.CompareExchange(ref this.categoryLock, CategoryLockValues.NoLockTaken, this.sharedLockCount); // if sharedlock and categorylock are 1, make category lock 0
        }

        public void EnterWriteLock() //// TODO enterexclusive
        {
            while (true)
            {
                if (Interlocked.CompareExchange(ref this.categoryLock, CategoryLockValues.ExclusiveLockTaken, 0) == 0)
                {
                    break;
                }
            }
        }

        public void ExitWriteLock()
        {
            Interlocked.Exchange(ref this.categoryLock, CategoryLockValues.NoLockTaken);
            //// TODO debug.asserts?
        }
    }
}
