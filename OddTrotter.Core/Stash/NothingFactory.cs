////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Stash
{
    using System;

    /// <summary>
    /// you can add this to the csproj:
    /// <ItemGroup>
    /// <Using Static = "true" Include="CalendarV2.Fx.NothingFactory" />
    /// </ItemGroup>
    /// 
    /// and then you can just say `Nothing` to get an intance of `Nothing`
    /// </summary>
    public static class NothingFactory
    {
        public static Nothing Nothing { get; } = new Nothing();
    }
}
