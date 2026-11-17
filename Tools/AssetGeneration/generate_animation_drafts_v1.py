from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[2]
SPRITES = ROOT / "Assets" / "Previews" / "SpriteDraftsV2"
OUTPUT = ROOT / "Assets" / "Previews" / "AnimationDraftsV1"
FRAME_COUNT = 4


def shifted(image, dx=0, dy=0):
    frame = Image.new("RGBA", image.size, (0, 0, 0, 0))
    frame.alpha_composite(image, (dx, dy))
    return frame


def root_idle(base, phase):
    # A slow settling motion, with a restrained pulse in the eyes.
    frame = shifted(base, dy=(0, -1, 0, 1)[phase])
    return frame


def moth_flap(base, phase):
    # The outer wing tips pivot around their attachment near x=24.
    slopes = (-0.20, -0.08, 0.20, 0.08)
    slope = slopes[phase]
    frame = Image.new("RGBA", base.size, (0, 0, 0, 0))
    source = base.load()
    target = frame.load()
    for y in range(base.height):
        for x in range(base.width):
            pixel = source[x, y]
            if x < 24:
                new_y = round(y + (x - 23) * slope)
                if 0 <= new_y < base.height and pixel[3] > target[x, new_y][3]:
                    target[x, new_y] = pixel
            else:
                target[x, y] = pixel
    # Candle and abdomen glow are brightest as the wings pass their midpoint.
    for y in range(0, 24):
        for x in range(24, 40):
            r, g, b, a = target[x, y]
            if a > 150 and r > 160 and 45 < g < 210 and b < 100:
                target[x, y] = ((255, 153, 51, 255), (255, 198, 79, 255), (255, 226, 122, 255), (255, 198, 79, 255))[phase]
    return frame


def pilgrim_walk(base, phase):
    body_dx = (0, -1, 0, 1)[phase]
    body_dy = (0, -1, 0, -1)[phase]
    frame = shifted(base, body_dx, body_dy)

    # Alternate the two boot silhouettes by a pixel to suggest a slow, heavy step.
    original = frame.copy()
    draw = ImageDraw.Draw(frame)
    draw.rectangle((9, 56, 49, 63), fill=(0, 0, 0, 0))
    feet = original.crop((9, 56, 50, 64))
    left = feet.crop((0, 0, 23, 8))
    right = feet.crop((23, 0, 41, 8))
    left_dx, right_dx = ((-2, 2), (0, 0), (2, -2), (0, 0))[phase]
    frame.alpha_composite(left, (9 + left_dx, 56))
    frame.alpha_composite(right, (32 + right_dx, 56))

    # Lantern/fire pixels pulse softly; the palette stays within the approved amber range.
    px = frame.load()
    fire = ((255, 137, 47, 255), (255, 183, 62, 255), (255, 219, 104, 255), (255, 183, 62, 255))[phase]
    for y in range(30, 54):
        for x in range(18, 53):
            r, g, b, a = px[x, y]
            if a > 150 and r > 170 and 35 < g < 200 and b < 100:
                px[x, y] = fire
    return frame


def save_strip(name, frames):
    width, height = frames[0].size
    strip = Image.new("RGBA", (width * len(frames), height), (0, 0, 0, 0))
    for i, frame in enumerate(frames):
        strip.alpha_composite(frame, (i * width, 0))
    strip.save(OUTPUT / f"{name}_AnimationDraft.png", optimize=True)
    return strip


def main():
    OUTPUT.mkdir(parents=True, exist_ok=True)
    root = Image.open(SPRITES / "WitheredRoot_v2.png").convert("RGBA")
    moth = Image.open(SPRITES / "CandleMoth_v2.png").convert("RGBA")
    pilgrim = Image.open(SPRITES / "HollowPilgrim_v2.png").convert("RGBA")
    tracks = [
        ("WitheredRoot_Idle", [root_idle(root, i) for i in range(FRAME_COUNT)], "Сухокорень — покой · 4 кадра · 8 тиков/кадр"),
        ("CandleMoth_Flap", [moth_flap(moth, i) for i in range(FRAME_COUNT)], "Свечная моль — взмах крыльев · 4 кадра · 4 тика/кадр"),
        ("HollowPilgrim_Walk", [pilgrim_walk(pilgrim, i) for i in range(FRAME_COUNT)], "Пустой паломник — тяжёлый шаг · 4 кадра · 6 тиков/кадр"),
    ]
    sheets = [(name, save_strip(name, frames), title) for name, frames, title in tracks]
    for (name, frames, _title) in tracks:
        duration = {"WitheredRoot_Idle": 160, "CandleMoth_Flap": 80, "HollowPilgrim_Walk": 120}[name]
        frames[0].save(
            OUTPUT / f"{name}_Preview.gif",
            save_all=True,
            append_images=frames[1:],
            duration=duration,
            loop=0,
            disposal=2,
            transparency=0,
        )

    cell_w, scale, row_h, header_h = 1120, 4, 260, 42
    sheet = Image.new("RGB", (cell_w, header_h + row_h * len(sheets)), (24, 22, 27))
    draw = ImageDraw.Draw(sheet)
    font = ImageFont.truetype(r"C:\Windows\Fonts\segoeui.ttf", 16)
    for row, (name, strip, title) in enumerate(sheets):
        y0 = header_h + row * row_h
        draw.text((18, y0 - 27), title, font=font, fill=(235, 225, 205))
        preview = strip.resize((strip.width * scale, strip.height * scale), Image.Resampling.NEAREST)
        x = (cell_w - preview.width) // 2
        backdrop = Image.new("RGBA", preview.size, (35, 32, 39, 255))
        backdrop.alpha_composite(preview)
        sheet.paste(backdrop.convert("RGB"), (x, y0 + (row_h - preview.height) // 2))
    sheet.save(OUTPUT / "CursedGrove_AnimationDrafts_ContactSheet.png", optimize=True)


if __name__ == "__main__":
    main()
