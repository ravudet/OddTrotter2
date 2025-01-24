namespace System
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OddTrotter;
    using System.Threading;

    /// <summary>
    /// Unit tests for <see cref="RelativeUri"/>
    /// </summary>
    [TestCategory(TestCategories.Unit)]
    [TestClass]
    public sealed class RelativeUriUnitTests
    {
        /// <summary>
        /// Creates a <see cref="RelativeUri"/> for a <see langword="null"/> <see cref="Uri"/>
        /// </summary>
        [TestMethod]
        public void NullUri()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsException<ArgumentNullException>(() => new RelativeUri(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        /// <summary>
        /// Creates a <see cref="RelativeUri"/> from a <see cref="Uri"/> with a absolute URI
        /// </summary>
        [TestMethod]
        public void AbsoluteUri()
        {
            var uri = new Uri("https://www.test.com/index.html", UriKind.Absolute);
            Assert.ThrowsException<ArgumentException>(() => new RelativeUri(uri));

            uri = new Uri("https://www.test.com/index.html", UriKind.RelativeOrAbsolute);
            Assert.ThrowsException<ArgumentException>(() => new RelativeUri(uri));
        }

        /// <summary>
        /// Creates a <see cref="AbsoluteUri"/> from a <see cref="Uri"/> with a relative URI
        /// </summary>
        [TestMethod]
        public void RelativeUri()
        {
            var uri = new Uri("/index.html", UriKind.Relative);
            var absoluteUri = new RelativeUri(uri);
            Assert.IsFalse(absoluteUri.IsAbsoluteUri);

            uri = new Uri("/index.html", UriKind.RelativeOrAbsolute);
            absoluteUri = new RelativeUri(uri);
            Assert.IsFalse(absoluteUri.IsAbsoluteUri);
        }

        [TestMethod]
        public void Asdf()
        {
            ulong foo = ulong.MaxValue;
            Interlocked.Increment(ref foo);
        }
    }

    public ref struct BigCounter
    {
        private ulong smallEnd;

        private volatile uint bigEnd;

        public BigCounter()
        {
            this.smallEnd = 0;
            this.bigEnd = 0;
        }

        /// <summary>
        /// WARNING: this method makes the assumption that `ulong` won't be overflowed *twice* in the time it takes for this method to return
        /// </summary>
        public void Increment()
        {
            var originalBigEnd = this.bigEnd;

            // increment the small end of the counter
            var result = Interlocked.Increment(ref this.smallEnd);
            if (result == 0)
            {
                // if `result` is `0`, then we have overflowed; let's increment the big end, but only if someone else hasn't already incremented it
                Interlocked.CompareExchange(ref this.bigEnd, originalBigEnd + 1, originalBigEnd);
            }
        }
    }
}