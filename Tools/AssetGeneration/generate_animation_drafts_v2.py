from collections import deque
from math import sin, pi
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[2]
SOURCE_DIR = ROOT / "Assets" / "Previews" / "SpriteDraftsV2"
OUTPUT = ROOT / "Assets" / "Previews" / "AnimationDraftsV2"
FRAME_COUNT = 8
TRANSPARENT = (0, 0, 0, 0)


def scaled_center(image, sx, sy):
    w, h = image.size
    nw, nh = max(1, round(w * sx)), max(1, round(h * sy))
    small = image.resize((nw, nh), Image.Resampling.NEAREST)
    out = Image.new("RGBA", (w, h), TRANSPARENT)
    out.alpha_composite(small, ((w - nw) // 2, (h - nh) // 2))
    return out


def take_layer(base, predicate):
    layer = Image.new("RGBA", base.size, TRANSPARENT)
    src, dst = base.load(), layer.load()
    for y in range(base.height):
        for x in range(base.width):
            color = src[x, y]
            if color[3] and predicate(x, y, color):
                dst[x, y] = color
                src[x, y] = TRANSPARENT
    return layer


def take_polygon_layer(base, points):
    mask = Image.new("L", base.size, 0)
    ImageDraw.Draw(mask).polygon(points, fill=255)
    return take_layer(base, lambda x, y, c: mask.getpixel((x, y)) > 0)


def put_layer(canvas, layer, angle=0, center=None, offset=(0, 0)):
    if angle:
        layer = layer.rotate(angle, resample=Image.Resampling.NEAREST, expand=False, center=center)
    if offset != (0, 0):
        moved = Image.new("RGBA", layer.size, TRANSPARENT)
        moved.alpha_composite(layer, offset)
        layer = moved
    canvas.alpha_composite(layer)


def remove_isolated_specks(image):
    alpha = image.getchannel("A")
    values = alpha.load()
    visited = set()
    specks = []
    for y in range(image.height):
        for x in range(image.width):
            if values[x, y] <= 127 or (x, y) in visited:
                continue
            queue = deque([(x, y)])
            visited.add((x, y))
            part = []
            while queue:
                cx, cy = queue.popleft()
                part.append((cx, cy))
                for ny in range(max(0, cy - 1), min(image.height, cy + 2)):
                    for nx in range(max(0, cx - 1), min(image.width, cx + 2)):
                        if values[nx, ny] > 127 and (nx, ny) not in visited:
                            visited.add((nx, ny))
                            queue.append((nx, ny))
            if len(part) < 10:
                specks.extend(part)
    pixels = image.load()
    for x, y in specks:
        for ny in range(max(0, y - 1), min(image.height, y + 2)):
            for nx in range(max(0, x - 1), min(image.width, x + 2)):
                pixels[nx, ny] = TRANSPARENT
    return image


def source_sprite(name):
    return remove_isolated_specks(Image.open(SOURCE_DIR / f"{name}_v2.png").convert("RGBA"))


def moth_sprite_with_flap_margin():
    source = source_sprite("CandleMoth")
    padded = Image.new("RGBA", (56, 48), TRANSPARENT)
    padded.alpha_composite(source, (8, 8))
    return padded


def root_frame(base, phase):
    # Whole body breathes; the outer roots and red vine lag behind the trunk.
    t = 2 * pi * phase / FRAME_COUNT
    body = scaled_center(base, 1 + 0.032 * sin(t), 1 + 0.04 * sin(t + 0.5))
    left_roots = take_layer(body, lambda x, y, c: y >= 28 and x <= 16)
    right_roots = take_layer(body, lambda x, y, c: y >= 28 and x >= 48)
    vine = take_layer(body, lambda x, y, c: 16 <= y <= 44 and 12 <= x <= 52 and c[0] > c[1] * 1.2 and c[0] > c[2] * 0.9)
    moss = take_layer(body, lambda x, y, c: x < 38 and y < 30 and c[1] > c[0] * 1.08 and c[1] > c[2] * 0.9)
    sprig = take_layer(body, lambda x, y, c: x < 20 and y < 22 and c[0] > c[1] * 1.15 and c[0] > c[2] * 0.85)
    out = body.copy()
    swing = (0, 4, 7, 4, 0, -4, -7, -4)[phase]
    put_layer(out, left_roots, angle=-swing, center=(14, 32), offset=(0, (0, 0, 1, 1, 0, -1, -1, 0)[phase]))
    put_layer(out, right_roots, angle=swing, center=(49, 32), offset=(0, (0, -1, -1, 0, 0, 1, 1, 0)[phase]))
    # Vine settles after the roots instead of following the same beat.
    put_layer(out, moss, angle=(0, -2, -4, -2, 0, 2, 4, 2)[phase], center=(25, 21), offset=(0, (0, -1, -1, 0, 0, 1, 1, 0)[phase]))
    put_layer(out, sprig, angle=(0, 3, 6, 3, 0, -3, -6, -3)[phase], center=(15, 14))
    put_layer(out, vine, angle=(0, 2, 4, 2, 0, -2, -4, -2)[phase], center=(37, 24), offset=(0, (0, 0, -1, -1, 0, 1, 1, 0)[phase]))
    return out


def moth_frame(base, phase):
    t = 2 * pi * phase / FRAME_COUNT
    breath = (1.0, 1.025, 1.04, 1.025, 1.0, 0.98, 0.97, 0.98)[phase]
    body = scaled_center(base, breath, 1 + 0.035 * sin(t + 0.4))
    # Split the large wing plates, abdomen, feelers, and tiny lower legs from the body.
    legs = take_layer(body, lambda x, y, c: y >= 35 and x <= 32 and c[0] < 175 and c[1] < 145)
    wings = take_layer(body, lambda x, y, c: x < 32)
    tail = take_layer(body, lambda x, y, c: 31 <= x <= 45 and y >= 28)
    feelers = take_layer(body, lambda x, y, c: x >= 38 and y <= 20 and not (c[0] > 170 and c[1] > 55 and c[2] < 130))
    head = take_layer(body, lambda x, y, c: x >= 35 and 17 <= y <= 30 and not (c[0] > 170 and c[1] > 55 and c[2] < 130))
    out = body.copy()
    wing_angle = (-20, -13, -5, 7, 20, 13, 5, -7)[phase]
    put_layer(out, wings, angle=wing_angle, center=(31, 24))
    tail_angle = (10, 7, 2, -5, -10, -7, -2, 5)[phase]
    tail_y = (4, 3, 1, -2, -4, -3, -1, 2)[phase]
    put_layer(out, tail, angle=tail_angle, center=(33, 28), offset=(0, tail_y))
    put_layer(out, legs, angle=(6, 4, 0, -5, -7, -4, 0, 5)[phase], center=(30, 34), offset=(0, (0, 0, -1, -1, 0, 1, 1, 0)[phase]))
    put_layer(out, head, angle=(0, -3, -6, -3, 0, 3, 6, 3)[phase], center=(39, 25), offset=((0, -1, -1, 0, 1, 1, 0, 0)[phase], 0))
    put_layer(out, feelers, angle=(-12, -7, 0, 9, 12, 7, 0, -9)[phase], center=(40, 18), offset=(0, (0, -1, -1, 0, 1, 1, 0, 0)[phase]))
    # Candlelight pulses on a different beat from the abdomen.
    px = out.load()
    glow = (255, 207, 103, 255) if phase in (2, 3, 6, 7) else (224, 132, 58, 255)
    for y in range(out.height):
        for x in range(out.width):
            r, g, b, a = px[x, y]
            if a > 150 and x >= 32 and r > 175 and 50 < g < 210 and b < 120:
                px[x, y] = glow
    return out


def pilgrim_frame(base, phase):
    # The Pilgrim carries every detail through a slow weight transfer around the planted staff.
    # This keeps the cloak, hood, reliquaries, and scarf moving as one coherent pixel silhouette.
    lean = (-4.0, -3.0, -1.0, 2.0, 4.0, 3.0, 1.0, -2.0)[phase]
    body = scaled_center(base, 1 + (0.03 if phase in (2, 3, 6, 7) else -0.015),
                         1 + (0.025 if phase in (0, 1, 4, 5) else -0.012))
    out = body.rotate(lean, resample=Image.Resampling.NEAREST, expand=False, center=(54, 61))
    bob = (0, -1, -1, 0, 0, 1, 1, 0)[phase]
    if bob:
        moved = Image.new("RGBA", out.size, TRANSPARENT)
        moved.alpha_composite(out, (0, bob))
        out = moved

    # A short, delayed sway at the scarf tip makes the fabric lag behind each planted step.
    px = out.load()
    scarf_tip_shift = (0, -2, -4, -2, 0, 2, 4, 2)[phase]
    scarf_pixels = []
    for y in range(13, 31):
        for x in range(0, 18):
            r, g, b, a = px[x, y]
            if a > 100 and r > g * 1.08 and r > 55 and r > b * 0.85:
                scarf_pixels.append((x, y, (r, g, b, a)))
    for x, y, color in scarf_pixels:
        px[x, y] = TRANSPARENT
    for x, y, color in scarf_pixels:
        put = (x, y + scarf_tip_shift)
        if 0 <= put[1] < out.height:
            px[put[0], put[1]] = color

    # Eyes briefly shine inside the hood; the lantern remains a secondary, steadier light.
    if phase in (2, 3, 6, 7):
        eye = (255, 116, 62, 255) if phase in (3, 7) else (161, 55, 52, 255)
        px[41, 12] = eye
        px[42, 12] = eye
        px[44, 12] = eye
        px[45, 12] = eye
    return out


def write_track(name, frames, frame_ticks, title, scale):
    w, h = frames[0].size
    strip = Image.new("RGBA", (w * len(frames), h), TRANSPARENT)
    for index, frame in enumerate(frames):
        strip.alpha_composite(frame, (index * w, 0))
    strip.save(OUTPUT / f"{name}_8frames.png", optimize=True)
    frames[0].save(OUTPUT / f"{name}_Preview.gif", save_all=True, append_images=frames[1:],
                   duration=round(frame_ticks * 1000 / 60), loop=0, disposal=2)

    sheet = Image.new("RGB", (w * scale * 4, h * scale * 2 + 36), (24, 22, 27))
    draw = ImageDraw.Draw(sheet)
    font = ImageFont.truetype(r"C:\Windows\Fonts\segoeui.ttf", 16)
    draw.text((10, 8), title, font=font, fill=(238, 226, 208))
    for index, frame in enumerate(frames):
        enlarged = frame.resize((w * scale, h * scale), Image.Resampling.NEAREST)
        x = (index % 4) * enlarged.width
        y = 32 + (index // 4) * enlarged.height
        panel = Image.new("RGBA", enlarged.size, (35, 32, 39, 255))
        panel.alpha_composite(enlarged)
        sheet.paste(panel.convert("RGB"), (x, y))
    sheet.save(OUTPUT / f"{name}_Frames_ContactSheet.png", optimize=True)
    return strip


def main():
    OUTPUT.mkdir(parents=True, exist_ok=True)
    tracks = [
        ("WitheredRoot_Idle", source_sprite("WitheredRoot"), root_frame, 16, "Сухокорень · дыхание, лоза и корни · 8 кадров", 6),
        ("CandleMoth_Flight", moth_sprite_with_flap_margin(), moth_frame, 6, "Свечная моль · крылья, встречный хвост, усики и лапки · 8 кадров", 9),
        ("HollowPilgrim_Walk", source_sprite("HollowPilgrim"), pilgrim_frame, 18, "Пустой паломник · медленный шаг, опора на посох, шарф и капюшон · 8 кадров", 6),
    ]
    strips = []
    for name, base, animate, ticks, title, scale in tracks:
        frames = [animate(base, phase) for phase in range(FRAME_COUNT)]
        strips.append((title, write_track(name, frames, ticks, title, scale)))

    width, scale, row_h = 1600, 3, 250
    sheet = Image.new("RGB", (width, 36 + row_h * len(strips)), (24, 22, 27))
    draw = ImageDraw.Draw(sheet)
    font = ImageFont.truetype(r"C:\Windows\Fonts\segoeui.ttf", 16)
    for row, (title, strip) in enumerate(strips):
        y0 = 36 + row * row_h
        draw.text((18, y0 - 25), title, font=font, fill=(238, 226, 208))
        preview = strip.resize((strip.width * scale, strip.height * scale), Image.Resampling.NEAREST)
        if preview.width > width - 24:
            fit = max(1, (width - 24) // strip.width)
            preview = strip.resize((strip.width * fit, strip.height * fit), Image.Resampling.NEAREST)
        panel = Image.new("RGBA", preview.size, (35, 32, 39, 255))
        panel.alpha_composite(preview)
        sheet.paste(panel.convert("RGB"), ((width - preview.width) // 2, y0 + (row_h - preview.height) // 2))
    sheet.save(OUTPUT / "CursedGrove_AnimationDrafts_v2_ContactSheet.png", optimize=True)


if __name__ == "__main__":
    main()
