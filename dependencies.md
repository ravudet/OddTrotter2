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
    Fx.Core-->OddTrotter.Core;
    Fx.Core-->Fx.AspNetCoreHttpInteractionLibrary*;
    Fx.Core-->Fx.MemoryCachingInteractionLibrary*;

    Fx.Core-->OddTrotter.Core;
    Fx.AspNetCoreHttpInteractionLibrary*-->OddTrotter.AspNetCoreHttpInteractionLibrary;
```
