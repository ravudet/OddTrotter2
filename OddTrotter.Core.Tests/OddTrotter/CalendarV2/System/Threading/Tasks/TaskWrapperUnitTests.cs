﻿namespace System.Threading.Tasks
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [TestClass]
    public sealed class TaskWrapperUnitTests
    {
        [TestMethod]
        public async Task Test()
        {
            var value = await new AwaitedType().GetValue();
            Assert.AreEqual("asdf", value);

            //// TODO use this to create a test which runs the safe oncompleted method: https://sharplab.io/#v2:CYLg1APgAgTAjAWAFBQAwAIpwKwG5lqZwB0ASgK4B2ALgJYC2ApsQMID29ADrQDaMBOAZQEA3WgGNGAZ3woAzJhjoW6AN4EFUAByYAbOgAq06gAoAlMnVJ0N9CICG/O/Z7lG6ALyYAnOkqMAd3QAQQD7WmpGYAMAT05Gc2IAcUZqADUXN3NZAF9kZE5+WgdI9ClGFyjFELCIqNj4y2RbTAUASShdAB4sVAA+dBT0zISLa1srFpaoAHY/QL0AdX57Tnj+Hrh+kwAxNjZzM1kWvKRm2yhNHU7N/vQ9g7GWyambKF9O4gARRh57GJMW1QZlYbEoADNaABzcj8RihcKmcEucpHc6vWboABE9ikwHBWOOtlOpxa6Na6FoNAEyMk6A63TY5Gohj6TXGNgZCLqGwMAyG3Mi/GyyFJtnJlzKFT4wGqnWWq3WXT56BA9JufPZLUKxXspTh9mAYJ4MT0yoG1FxAGtZBLNLoFWsBCYNRbrU8JuSWtQABa0KTES1SK2edBBm3ksU2O3q3SCgTmwapePCj02F4Yub+IKdFOOpV8ky+/2B63JZO1IWHIk2EnkmNUoW09xcysJpkslVqtosIp0cQuABybDo4Ji7C4fDoYK1tgARvsePSpBPOFOqqp0FDUrh0KdyQYk9RSNJyDxTGikFH0DHypVZbA9HmVk7eQNu7m22/ZzYdSV3AaRqUCaT5fom4YpraHIUp+iICPmzqwTy4HWimaZqF6tjFgGEFfqGuFwfwNZ7vk0GSgubBLm0K4cGuqRROSGZTNu1CYembFTJi2GlsGKbENRq7rsAxEnJGpHTAoh5DCeUhnhejEcVxfo4ahX7lsep7niK0H7mRmgACzoAA8pQgn0cALpwEo4hgnQlDkHqtBguhTHespPFWnxJlmZEFk2TQVIOdOlCXqJZx6ZghkAKqUFI9jgow3m0UJlnWbZgWOc5CnQW5JYETyxAxXFCVJZO5kmP5dlBU5IXEXWV5AA=
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

        [TestMethod]
        public Task Test2()
        {
            StateMachine stateMachine = new StateMachine();
            stateMachine.builder = AsyncTaskMethodBuilder.Create();
            stateMachine.self = this;
            stateMachine.state = -1;
            stateMachine.builder.Start(ref stateMachine);
            return stateMachine.builder.Task;
        }

        private sealed class StateMachine : IAsyncStateMachine
        {
            public int state;

            public AsyncTaskMethodBuilder builder;

            public TaskWrapperUnitTests? self;

            private string? value;

            private string? s;

            private object? u;

            private void MoveNext()
            {
                int num = state;
                try
                {
                    ITaskAwaiter<string> awaiter;
                    if (num != 0)
                    {
                        awaiter = new AwaitedType().GetValue().GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            num = (state = 0);
                            u = awaiter;
                            StateMachine stateMachine = this;
                            builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
                            return;
                        }
                    }
                    else
                    {
                        awaiter = ((ITaskAwaiter<string>)u!); //// TODO you addeed the bang, is that ok?
                        u = null;
                        num = (state = -1);
                    }
                    s = awaiter.GetResult();
                    value = s;
                    s = null;
                    Assert.AreEqual(value, "asdf");
                }
                catch (Exception exception)
                {
                    state = -2;
                    value = null;
                    builder.SetException(exception);
                    return;
                }
                state = -2;
                value = null;
                builder.SetResult();
            }

            void IAsyncStateMachine.MoveNext()
            {
                //ILSpy generated this explicit interface implementation from .override directive in MoveNext
                this.MoveNext();
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
            }

            void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
            {
                //ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
                this.SetStateMachine(stateMachine);
            }
        }
    }
}
