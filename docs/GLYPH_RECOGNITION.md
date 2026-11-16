# Glyph recognition core

`GlyphTemplateRecognizer` is a graphics-free one-stroke recognizer for the Occultist's future drawing interface. It resamples paths to 64 evenly spaced points, aligns their indicative angle, scales them into a normalized square, centers them, and compares corresponding samples with average Euclidean distance.

Callers supply the glyph templates. The Wound, Binding, and Renunciation path shapes are intentionally not fixed here: their iconography and drawing gestures still need visual review. The default match threshold is `0.28`; the returned confidence is a normalized presentation value and should be shown together with whether the threshold was met.

Input collection, the drawing overlay, feedback animation, glyph effects, and multiplayer action validation remain separate implementation work. UI and animation visuals wait for the user's pixel-art approval.
