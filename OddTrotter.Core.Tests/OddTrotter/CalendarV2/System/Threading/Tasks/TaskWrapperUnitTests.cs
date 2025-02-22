namespace System.Threading.Tasks
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Runtime.CompilerServices;

    [TestClass]
    public sealed class TaskWrapperUnitTests
    {
        [TestMethod]
        public async Task Test()
        {
            var value = await new AwaitedType().GetValue();
            Assert.AreEqual("asdf", value);
        }

        private sealed class AwaitedType
        {
            public ITask<string> GetValue()
            {
                return new TaskWrapper<string>(Foo());
            }

            public async Task<string> Foo()
            {
                await Task.Delay(100).ConfigureAwait(false);
                return "asdf";
            }
        }
    }
}
