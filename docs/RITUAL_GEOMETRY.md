# Ritual geometry

`RitualGeometry` builds a triangle from three temporary candle positions or a five-point star path from five positions. Candle positions are sorted around their centroid, so placement order does not affect the result. Its line segments can be rendered independently from its point-in-shape query.

The pentagram uses an even-odd fill rule; its central empty space remains outside the damaging area. The helper rejects non-finite, too-close, and near-collinear placements. Placement range, candle lifetime, ritual cadence, enemy effects, and server validation belong to the gameplay manager and are not part of this geometry layer.
