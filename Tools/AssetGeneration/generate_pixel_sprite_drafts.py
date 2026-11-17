"""Generate deterministic, native-resolution sprite drafts for owner review.

These outputs are previews only. They are intentionally kept outside Content/ and
must not be loaded by the mod until the owner approves the pixel sprites.
Requires Pillow: python -m pip install Pillow
"""

from pathlib import Path
from PIL import Image, ImageDraw


ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / "Assets" / "Previews" / "SpriteDrafts"
SCALE = 8

PALETTE = {
    "outline": (17, 17, 24, 255),
    "bark": (43, 34, 34, 255),
    "bark_mid": (68, 51, 43, 255),
    "bark_light": (95, 70, 53, 255),
    "moss": (57, 69, 52, 255),
    "burgundy": (105, 37, 48, 255),
    "burgundy_light": (145, 53, 57, 255),
    "bone": (218, 208, 177, 255),
    "bone_shadow": (161, 151, 133, 255),
    "cloth": (48, 43, 52, 255),
    "cloth_mid": (73, 62, 68, 255),
    "amber": (247, 172, 64, 255),
    "amber_core": (255, 226, 139, 255),
    "cold": (116, 188, 190, 255),
}


def canvas(size):
    return Image.new("RGBA", size, (0, 0, 0, 0))


def poly(draw, points, color):
    draw.polygon(points, fill=color)


def line(draw, points, color, width=1):
    draw.line(points, fill=color, width=width, joint="curve")


def make_withered_root():
    im = canvas((32, 30))
    d = ImageDraw.Draw(im)
    p = PALETTE
    poly(d, [(4, 26), (1, 28), (1, 29), (8, 27), (11, 29), (15, 27), (21, 29), (24, 27), (30, 28), (27, 25)], p["outline"])
    poly(d, [(7, 22), (3, 21), (1, 18), (2, 16), (5, 18), (9, 19), (11, 16), (12, 10), (14, 6), (20, 5), (24, 8), (25, 15), (28, 17), (31, 16), (31, 19), (27, 22), (24, 21), (23, 26), (19, 25), (17, 22), (13, 25), (10, 26)], p["outline"])
    poly(d, [(8, 21), (5, 20), (3, 18), (5, 19), (10, 18), (12, 12), (13, 7), (19, 6), (23, 9), (23, 16), (26, 19), (29, 18), (27, 21), (23, 20), (22, 24), (19, 23), (16, 20), (12, 23), (10, 24)], p["bark"])
    poly(d, [(13, 8), (15, 6), (19, 7), (21, 10), (19, 12), (15, 11)], p["bark_mid"])
    poly(d, [(13, 12), (15, 11), (18, 13), (20, 16), (17, 18), (12, 17)], p["bark_mid"])
    line(d, [(14, 8), (12, 10), (11, 15), (8, 17)], p["bark_light"])
    line(d, [(20, 9), (22, 12), (21, 17), (25, 19)], p["bark_light"])
    poly(d, [(12, 5), (14, 2), (15, 5), (18, 3), (18, 6)], p["bark"])
    poly(d, [(20, 6), (23, 3), (22, 8)], p["bark_mid"])
    poly(d, [(4, 19), (2, 17), (3, 20), (7, 21)], p["bark_mid"])
    poly(d, [(27, 19), (30, 17), (30, 19), (27, 21)], p["bark_mid"])
    poly(d, [(10, 12), (11, 8), (13, 7), (12, 12)], p["moss"])
    poly(d, [(20, 7), (22, 8), (24, 11), (22, 12)], p["moss"])
    d.point((15, 14), fill=p["burgundy_light"])
    d.point((20, 14), fill=p["burgundy_light"])
    d.point((16, 14), fill=p["outline"])
    d.point((21, 14), fill=p["outline"])
    return im


def make_candle_moth():
    im = canvas((24, 18))
    d = ImageDraw.Draw(im)
    p = PALETTE
    # Broad, drooping wings remain distinct when drawn at native Terraria scale.
    poly(d, [(11, 8), (8, 3), (4, 1), (1, 3), (2, 7), (6, 10), (10, 11)], p["outline"])
    poly(d, [(13, 8), (16, 3), (20, 1), (23, 3), (22, 7), (18, 10), (14, 11)], p["outline"])
    poly(d, [(11, 8), (8, 4), (4, 2), (2, 4), (3, 7), (7, 9), (10, 10)], p["bone"])
    poly(d, [(13, 8), (16, 4), (20, 2), (22, 4), (21, 7), (17, 9), (14, 10)], p["bone"])
    line(d, [(4, 3), (7, 5), (9, 8)], p["bone_shadow"])
    line(d, [(20, 3), (17, 5), (15, 8)], p["bone_shadow"])
    poly(d, [(5, 4), (7, 4), (8, 6), (6, 7)], p["burgundy"])
    poly(d, [(19, 4), (17, 4), (16, 6), (18, 7)], p["burgundy"])
    line(d, [(11, 7), (10, 5)], p["outline"])
    line(d, [(13, 7), (14, 5)], p["outline"])
    poly(d, [(10, 7), (12, 6), (14, 7), (15, 11), (13, 13), (10, 12)], p["outline"])
    poly(d, [(11, 8), (12, 7), (13, 8), (14, 11), (12, 12), (11, 11)], p["bark_mid"])
    poly(d, [(11, 12), (14, 11), (15, 14), (13, 16), (10, 15)], p["amber"])
    d.point((12, 13), fill=p["amber_core"])
    d.point((8, 11), fill=p["bone_shadow"])
    d.point((17, 11), fill=p["bone_shadow"])
    return im


def make_hollow_pilgrim():
    im = canvas((24, 40))
    d = ImageDraw.Draw(im)
    p = PALETTE
    # Staff and reliquary silhouette.
    line(d, [(2, 7), (2, 37)], p["outline"], 2)
    line(d, [(3, 8), (3, 35)], p["bark_light"])
    poly(d, [(1, 6), (2, 3), (4, 6), (3, 9)], p["bone_shadow"])
    poly(d, [(4, 17), (6, 16), (7, 21), (5, 23)], p["outline"])
    poly(d, [(5, 18), (6, 18), (6, 21), (5, 21)], p["bone"])
    # Hood and empty face.
    poly(d, [(8, 4), (11, 1), (16, 1), (20, 5), (21, 10), (18, 14), (11, 14), (7, 10)], p["outline"])
    poly(d, [(9, 5), (12, 2), (16, 2), (19, 5), (20, 9), (17, 12), (11, 12), (8, 9)], p["cloth_mid"])
    poly(d, [(11, 6), (16, 5), (18, 8), (16, 11), (11, 11), (9, 9)], p["outline"])
    # Slumped mantle and asymmetric cloak hem.
    poly(d, [(9, 12), (18, 12), (21, 18), (20, 30), (23, 37), (18, 39), (13, 37), (8, 39), (4, 36), (7, 27), (6, 18)], p["outline"])
    poly(d, [(10, 13), (17, 13), (19, 19), (18, 30), (21, 36), (18, 37), (13, 35), (8, 37), (6, 35), (9, 27), (8, 19)], p["cloth"])
    poly(d, [(9, 17), (12, 15), (13, 26), (10, 33), (8, 35), (7, 34)], p["cloth_mid"])
    poly(d, [(16, 14), (19, 19), (17, 26), (19, 34), (17, 35), (14, 29)], p["burgundy"])
    line(d, [(11, 14), (14, 17), (13, 24)], p["bone_shadow"])
    # Scarf, clasp, and pendant.
    poly(d, [(8, 12), (13, 14), (18, 12), (21, 14), (17, 17), (12, 16), (8, 14)], p["burgundy_light"])
    poly(d, [(14, 15), (16, 16), (16, 20), (14, 21), (13, 18)], p["burgundy"])
    d.point((14, 18), fill=p["bone"])
    # Feet.
    poly(d, [(7, 35), (11, 35), (11, 38), (5, 39)], p["outline"])
    poly(d, [(15, 35), (18, 35), (21, 38), (16, 39)], p["outline"])
    return im


def make_dormant_altar():
    im = canvas((48, 32))
    d = ImageDraw.Draw(im)
    p = PALETTE
    # Three candle flames on a low carved wood altar (3x2 tiles).
    for x, top in ((9, 7), (24, 4), (38, 7)):
        d.point((x, top), fill=p["amber_core"])
        d.point((x, top + 1), fill=p["amber"])
        d.rectangle((x - 1, top + 2, x + 1, top + 5), fill=p["bone"])
        d.point((x, top + 2), fill=p["bone_shadow"])
    # Candle cups and carved wood frame.
    poly(d, [(4, 12), (7, 10), (11, 10), (13, 12), (13, 21), (4, 21)], p["outline"])
    poly(d, [(5, 13), (8, 11), (10, 11), (12, 13), (12, 20), (5, 20)], p["bark_mid"])
    poly(d, [(17, 9), (20, 7), (28, 7), (31, 10), (31, 21), (17, 21)], p["outline"])
    poly(d, [(18, 10), (21, 8), (27, 8), (30, 10), (30, 20), (18, 20)], p["bark"])
    poly(d, [(35, 12), (38, 10), (42, 10), (45, 13), (44, 21), (35, 21)], p["outline"])
    poly(d, [(36, 13), (39, 11), (41, 11), (43, 13), (43, 20), (36, 20)], p["bark_mid"])
    # Top, legs and base.
    poly(d, [(3, 20), (45, 20), (47, 23), (1, 23)], p["outline"])
    poly(d, [(4, 21), (44, 21), (45, 22), (3, 22)], p["bark_light"])
    poly(d, [(5, 23), (10, 23), (9, 28), (6, 28)], p["outline"])
    poly(d, [(38, 23), (43, 23), (42, 28), (39, 28)], p["outline"])
    poly(d, [(2, 27), (46, 27), (45, 30), (4, 30)], p["outline"])
    poly(d, [(4, 28), (44, 28), (43, 29), (5, 29)], p["bark_mid"])
    # Central ivory devotional rune and burgundy drape.
    line(d, [(24, 11), (24, 18)], p["bone"])
    line(d, [(21, 14), (27, 14)], p["bone"])
    d.point((23, 12), fill=p["amber"])
    poly(d, [(20, 22), (28, 22), (27, 27), (24, 29), (21, 27)], p["burgundy"])
    d.point((24, 24), fill=p["amber"])
    # Chips, grain, and candle holders.
    for x in (8, 24, 39):
        d.point((x - 2, 19), fill=p["bone_shadow"])
        d.point((x + 2, 19), fill=p["bone_shadow"])
    line(d, [(8, 24), (10, 25)], p["bark_light"])
    line(d, [(38, 24), (40, 25)], p["bark_light"])
    return im


def save_preview(name, image):
    image.save(OUT / f"{name}.png")
    return image


def make_contact_sheet(sprites):
    cells = []
    for label, image in sprites:
        enlarged = image.resize((image.width * SCALE, image.height * SCALE), Image.Resampling.NEAREST)
        cell = Image.new("RGBA", (max(320, enlarged.width + 40), max(260, enlarged.height + 70)), (26, 26, 34, 255))
        x = (cell.width - enlarged.width) // 2
        y = 20 + (cell.height - 50 - enlarged.height) // 2
        cell.alpha_composite(enlarged, (x, y))
        cells.append((label, cell))

    sheet = Image.new("RGBA", (sum(cell.width for _, cell in cells), max(cell.height for _, cell in cells)), (20, 20, 28, 255))
    x = 0
    for _, cell in cells:
        sheet.alpha_composite(cell, (x, 0))
        x += cell.width
    return sheet


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    sprites = [
        ("WitheredRoot_32x30", make_withered_root()),
        ("CandleMoth_24x18", make_candle_moth()),
        ("HollowPilgrim_24x40", make_hollow_pilgrim()),
        ("DormantEternalAltar_48x32", make_dormant_altar()),
    ]
    saved = [(name, save_preview(name, image)) for name, image in sprites]
    make_contact_sheet(saved).save(OUT / "PixelSpriteDrafts_ContactSheet.png")


if __name__ == "__main__":
    main()
