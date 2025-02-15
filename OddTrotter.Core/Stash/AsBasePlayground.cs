using System;

namespace Stash
{
    public static class AsBasePlayground
    {
        public static void DoWork()
        {
            As<Exception>().Base(new InvalidOperationException());

            ////new InvalidOperationException().As().Base2<Exception>();


            new InvalidOperationException().As(Type2.Of<Exception>());
        }


        public sealed class Base<TBase>
        {
        }


        public static TBase As<TBase, TDerived>(this TDerived derived, Type2<TBase> type) where TDerived : TBase
        {
            return derived;
        }



        public static Factory2<TBase> As<TBase>()
        {
            return new Factory2<TBase>();
        }

        public sealed class Factory2<TBase>
        {
            public TBase Base<TDerived>(TDerived value) where TDerived : TBase
            {
                return value;
            }
        }

        public static Factory<TDerived> As<TDerived>(this TDerived value)
        {
            return new Factory<TDerived>(value);
        }

        public static TBase Base2<TDerived, TBase>(this Factory<TDerived> factory) where TDerived : TBase
        {
            return factory.Value;
        }

        public sealed class Factory<TDerived>
        {
            public Factory(TDerived value)
            {
                Value = value;
            }

            public TDerived Value { get; }
        }
    }

    public static class Type2
    {
        public static Type2<T> Of<T>()
        {
            ////return new Type<T>();
            return null!;
        }
    }

    public sealed class Type2<T>
    {
        private Type2()
        {
        }
    }
}
