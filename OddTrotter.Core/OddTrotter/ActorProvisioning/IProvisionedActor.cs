namespace OddTrotter.ActorProvisioning
{
    using System;
    
    using OddTrotter.TokenIssuance;

    /// <summary>
    /// An <see cref="Actor"/> that will clean itself up when disposed
    /// </summary>
    public interface IProvisionedActor : IAsyncDisposable
    {
        /// <summary>
        /// The <see cref="Actor"/> that was created, that will be removed when this instance of <see cref="IProvisionedActor"/> is <see cref="IAsyncDisposable.DisposeAsync"/>d
        /// </summary>
        Actor Actor { get; }
    }
}