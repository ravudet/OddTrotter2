/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.QueryContext
{
    internal sealed class MockEmpty : IEmpty
    {
        /// <summary>
        /// placeholder
        /// </summary>
        private MockEmpty()
        {
        }

        /// <summary>
        /// placeholder
        /// </summary>
        public static MockEmpty Instance { get; } = new MockEmpty();
    }
}
