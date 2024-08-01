namespace OddTrotter.ActorProvisioning
{
    using System;

    public sealed class ActorProvisioningException : Exception
    {
        public ActorProvisioningException(string message)
            : base(message)
        {
        }

        public ActorProvisioningException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
