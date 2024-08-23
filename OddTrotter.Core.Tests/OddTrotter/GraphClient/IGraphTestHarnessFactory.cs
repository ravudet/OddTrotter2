namespace OddTrotter.GraphClient
{
    using System.Threading.Tasks;

    /// <summary>
    /// A factory for generating new <see cref="GraphTestHarness"/> instances
    /// </summary>
    public interface IGraphTestHarnessFactory
    {
        /// <summary>
        /// Creates a new <see cref="GraphTestHarness"/> for use in a test case
        /// </summary>
        /// <returns>A new <see cref="GraphTestHarness"/> for use in a test case</returns>
        /// //// TODO document exceptions?
        Task<GraphTestHarness> CreateTestHarnessAsync();
    }
}
