namespace System.Linq
{
    using Fx.Either;

    public static class FirstOrDefault
    {
        public static FirstOrDefault<TFirst, TDefault> Create<TFirst, TDefault>(IEither<TFirst, TDefault> either)
        {
            return new FirstOrDefault<TFirst, TDefault>(either);
        }
    }
}
