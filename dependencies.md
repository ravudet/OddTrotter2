Fx.Core
Fx.AspNetCoreHttpInteractionLibrary*
Fx.MemoryCachingInteractionLibrary*
OddTrotter.Core
OddTrotter.AspNetCoreHttpInteractionLibrary
OddTrotter.MemoryCachingInteractionLibrary
OddTrotterPortal
Fx.Games.Core
Fx.Games
FxGamesConsoleApplication

* doesn't exist yet

```mermaid
graph TD;
    Fx.Core-->Fx.AspNetCoreHttpInteractionLibrary*;

    Fx.Core-->Fx.MemoryCachingInteractionLibrary*;

    Fx.Core-->OddTrotter.Core;

    Fx.Core-->OddTrotter.AspNetCoreHttpInteractionLibrary;
    OddTrotter.Core-->OddTrotter.AspNetCoreHttpInteractionLibrary;
    Fx.AspNetCoreHttpInteractionLibrary*-->OddTrotter.AspNetCoreHttpInteractionLibrary;

    Fx.Core-->OddTrotter.MemoryCachingInteractionLibrary;
    OddTrotter.Core-->OddTrotter.MemoryCachingInteractionLibrary;
    Fx.MemoryCachingInteractionLibrary*-->OddTrotter.MemoryCachingInteractionLibrary;

    Fx.Core-->OddTrotterPortal;
    Fx.AspNetCoreHttpInteractionLibrary*-->OddTrotterPortal;
    Fx.MemoryCachingInteractionLibrary*-->OddTrotterPortal;
    OddTrotter.Core-->OddTrotterPortal;
    OddTrotter.AspNetCoreHttpInteractionLibrary-->OddTrotterPortal;
    OddTrotter.MemoryCachingInteractionLibrary-->OddTrotterPortal;
```
