# Cursed Grove placement planner

`CursedGrovePlacementPlanner` chooses deterministic horizontal candidate ranges for surface grove patches. It reserves world-edge padding, keeps each candidate at least its half-width plus 220 tiles from the world spawn, separates regions by a 180-tile gap, and caps each placement search at 64 attempts. The xorshift generator is independent of `System.Random` version changes.

The planner does not modify tiles or claim that a biome generated successfully. The later worldgen pass must sample terrain, avoid oceans and protected structures, and confirm a suitable walkable surface before stamping approved biome tiles. If the world is too narrow or all bounded attempts fail, the planner returns fewer regions rather than looping indefinitely.
