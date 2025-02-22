namespace System.Threading.Tasks
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class TaskWrapperUnitTests
    {
        [TestMethod]
        public async Task Await()
        {
            var value = await new AwaitedType().GetValue2();
            Assert.AreEqual("asdfasdf3asdf2", value);
        }

        private sealed class AwaitedType
        {
            public ITask<string> GetValue()
            {
                return new TaskWrapper<string>(Task.Delay(100).ContinueWith(_ => "asdf"));
            }

            public ITask<string> GetAnotherValue()
            {
                return new TaskWrapper<string>(Task.FromResult("asdf3"));
            }

            public async Task<string> GetValue2()
            {
                var value = await GetValue();
                var value2 = await GetAnotherValue();
                return value + value2 + "asdf2";
            }
        }
    }
}
