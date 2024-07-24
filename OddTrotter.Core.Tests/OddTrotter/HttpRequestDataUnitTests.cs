namespace OddTrotter
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="HttpRequestData"/>
    /// </summary>
    [TestCategory(TestCategories.Unit)]
    [TestClass]
    public sealed class HttpRequestDataUnitTests
    {
        /// <summary>
        /// Creates <see cref="HttpRequestData"/> with <see langword="null"/> form data
        /// </summary>
        [TestMethod]
        public void NullForm()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new HttpRequestData(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                null
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                ));
        }

        /// <summary>
        /// Retrieves the form data after the original has been modified
        /// </summary>
        [TestMethod]
        public void ModifiedFormData()
        {
            var form = new Dictionary<string, IReadOnlyList<string>>()
            {
                { "key", new List<string>() { "a value" } },
            };
            var httpRequestData = new HttpRequestData(form);
            form.Add("key2", new List<string>());
            Assert.IsFalse(httpRequestData.Form.TryGetValue("key2", out _));
        }

        /// <summary>
        /// Compares the ID of two <see cref="HttpRequestData"/>s
        /// </summary>
        [TestMethod]
        public void UniqueId()
        {
            var first = new HttpRequestData(new Dictionary<string, IReadOnlyList<string>>());
            var second = new HttpRequestData(new Dictionary<string, IReadOnlyList<string>>());
            Assert.AreNotEqual(first.Id, second.Id);
        }
    }
}
