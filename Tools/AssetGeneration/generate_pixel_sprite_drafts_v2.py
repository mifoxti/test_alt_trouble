from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "Assets" / "Source" / "Concepts" / "CursedGrove_Enemies_DetailedGuide.png"
OUTPUT = ROOT / "Assets" / "Previews" / "SpriteDraftsV2"

# Separate, isolated figures in the approved detailed concept guide.
FIGURES = [
    ("WitheredRoot", (18, 170, 870, 710), (64, 48), 28),
    ("CandleMoth", (878, 252, 1376, 606), (40, 32), 28),
    ("HollowPilgrim", (1428, 16, 2156, 714), (64, 64), 32),
]
SHEET_BACKGROUND = (24, 22, 27)


def make_sprite(source: Image.Image, crop_box, size, colors):
    crop = source.crop(crop_box)
    alpha = crop.getchannel("A")
    visible_bounds = alpha.point(lambda value: 255 if value > 24 else 0).getbbox()
    crop = crop.crop(visible_bounds)

    width, height = size
    factor = min((width - 2) / crop.width, (height - 2) / crop.height)
    resized_size = (max(1, round(crop.width * factor)), max(1, round(crop.height * factor)))
    resized = crop.resize(resized_size, Image.Resampling.LANCZOS)
    palette_rgb = resized.convert("RGB").quantize(
        colors=colors,
        method=Image.Quantize.FASTOCTREE,
        dither=Image.Dither.NONE,
    ).convert("RGB")
    sprite = Image.new("RGBA", size, (0, 0, 0, 0))
    colored = Image.merge("RGBA", (*palette_rgb.split(), resized.getchannel("A")))
    sprite.alpha_composite(colored, ((width - resized_size[0]) // 2, (height - resized_size[1]) // 2))
    return sprite


def main():
    OUTPUT.mkdir(parents=True, exist_ok=True)
    source = Image.open(SOURCE).convert("RGBA")
    rendered = []
    for name, crop_box, size, colors in FIGURES:
        sprite = make_sprite(source, crop_box, size, colors)
        sprite.save(OUTPUT / f"{name}_v2.png", optimize=True)
        rendered.append((name, sprite))

    cell_width, cell_height, preview_scale = 430, 500, 6
    sheet = Image.new("RGB", (cell_width * len(rendered), cell_height), SHEET_BACKGROUND)
    draw = ImageDraw.Draw(sheet)
    font = ImageFont.load_default(size=20)
    for index, (name, sprite) in enumerate(rendered):
        x0 = index * cell_width
        draw.text((x0 + 18, 16), f"{name}  {sprite.width}x{sprite.height}", font=font, fill=(235, 225, 205))
        enlarged = sprite.resize((sprite.width * preview_scale, sprite.height * preview_scale), Image.Resampling.NEAREST)
        x = x0 + (cell_width - enlarged.width) // 2
        y = 46 + (410 - enlarged.height) // 2
        panel = Image.new("RGBA", enlarged.size, (*SHEET_BACKGROUND, 255))
        panel.alpha_composite(enlarged)
        sheet.paste(panel.convert("RGB"), (x, y))
        native = sprite.resize((sprite.width * 2, sprite.height * 2), Image.Resampling.NEAREST)
        sheet.paste(native.convert("RGB"), (x0 + cell_width - 150, cell_height - 130), native.getchannel("A"))
    sheet.save(OUTPUT / "CursedGrove_SpriteDrafts_v2_ContactSheet.png", optimize=True)


if __name__ == "__main__":
    main()
