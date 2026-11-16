# Art direction proposal — awaiting approval

The following is a review sheet, not an approved final sprite design. It combines the user's dark fantasy and horror direction with the silhouettes and details already in the task brief. No final sprite or animation sheet should be generated until this sheet is approved or revised.

## Shared visual language

- Terraria-scale pixel art with sharp pixel edges and transparent backgrounds.
- Bone white and aged porcelain against charcoal cloth and blue-black shadow.
- Dull gold for old sacred objects; restrained dark red only where a living wound or broken divinity is visible.
- Cold pale blue for ghostly phenomena. No decorative gore.

## The Blind Confessor

- Narrow, tall silhouette in a worn charcoal robe, with a long white eye band and a small bone reliquary at the chest.
- A low, warm lantern hangs from one hand and is the only moving light source on his body.
- Suggested sprite footprint: approximately 40 by 56 pixels, subject to a preview sheet and in-game scale review.
- Idle: 4 frames, barely visible breathing; lantern swing offsets by one or two pixels.
- Walk: 8 frames, robe hem follows the step; lantern swing is slightly out of phase with his stride.
- Conversation/rite: 5 frames, one restrained hand gesture; lantern brightens once as the spoken line lands.

## Dream and arrival sequence

- Fade from the player's home into a short scene, then reveal a layered silhouette of the Lost City.
- Seeded, close street facades and broken bridges make each world read differently without changing saved Terraria tiles. The city should feel near and oppressive, not like a distant skyline.
- Small drifting moth-like lights provide local highlights in the street. Their exact shape, behavior, and light color still need a pixel-art review.
- The Confessor appears first as a low-contrast outline; his blindfold, robe, and lantern resolve in successive light strokes before he speaks.
- After the player places the requested home altar, later nights add one visible detail at a time. At the final stage, he steps forward from the world backdrop and becomes a regular town NPC.
- The city-layout generator should be reusable by a future Subworld Library adapter.

## The Weeping Seraph

- Tall floating silhouette in damaged ivory and muted-gold armor; a cracked white porcelain mask has no visible eyes.
- Three broken wing shapes alternate bone spars and dark organic membrane. A thin dark-red glow leaks only through armor cracks.
- Suggested body sheet footprint: approximately 112 by 128 pixels, to be validated against hitboxes and arena scale.
- Vigil palette: cold ivory, faded gold, pale light. Lament palette: charcoal, blood-dark red, and a thin white edge light.
- Attack telegraphs use readable wing and arm poses: folding in for a dash, lifting for feather fans, and opening unevenly before ring attacks.
- Transition: the mask fracture catches a white highlight, wings open, and red light travels through existing cracks. Death: the body descends, wings separate into pale fragments, then the mask breaks last.

## Proposed animation inventory

| Character | Proposed sequences |
| --- | --- |
| Confessor | Idle 4, walk 8, talk/rite 5, apparition resolve 4, step out 6 |
| Seraph | Hover 6, movement 4, attack telegraphs 3 each, attack 4, phase transition 8, death 8 |

## Approval needed

The user approved the Confessor and Seraph concept panels. The first dream is approved as a cinematic scene; an explorable Lost City remains a later goal. The close, dark city composition is preferred, with more localized light from moth-like creatures. The final game art must be redrawn as crisp pixel art and the animation sheets still need review before gameplay integration.
