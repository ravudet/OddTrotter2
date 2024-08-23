namespace OddTrotter.ActorProvisioning
{
    using System;

    public sealed class ActorCleanupException : Exception
    {
        public ActorCleanupException(string message)
            : base(message)
        {
        }

        public ActorCleanupException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
