# The Last Testament

# The Last Testament

**The Last Testament** — сюжетный мод для Terraria в жанре тёмного фэнтези и хоррора. Мир продолжает жить на останках погибших богов, а их сны всё ещё достигают живых.

Это первая рабочая основа вертикального среза на tModLoader 1.4.4. Код и внутренние идентификаторы — на английском; игровые строки локализуются на русский и английский.

## Состояние проекта

- Проект собирается для стабильного tModLoader v2026.09.3.1 и .NET 8.
- В `master` есть базовая инфраструктура, два типа урона, модель Corruption/Ethereality, конфиг баланса, сохранение прогресса и данные детерминированной планировки Потерянного города.
- Фазовый рывок, распознаватель глифов и геометрия ритуалов находятся в отдельных незлитых ветках. Распознаватель и геометрия пока не подключены к игровым предметам или интерфейсам.
- Биомы, worldgen, враги, ресурсы-добыча, рецепты, Eternal Altar, сюжетная сцена и боссы ещё не реализованы.
- Одобренные концепты и новые варианты на просмотр отслеживаются отдельно; никаких финальных новых игровых текстур пока нет.
- Сборка проверялась. Не считай игровые системы или генерацию мира протестированными до фактического запуска в Terraria.

См. [карту разработки](docs/PREHARDMODE_ROADMAP.md), [прогрессию](docs/PROGRESSION.md), [реестр согласования арта](docs/ASSET_APPROVAL.md), [архитектуру](docs/ARCHITECTURE.md), [Subworld Library](docs/SUBWORLDS.md) и [статус проверок](docs/TESTING.md).

## Сборка

Установите стабильную версию tModLoader и .NET SDK 8. Проект загружает `tMLMod.targets` из папки tModLoader. Если Steam установлен в другом месте:

```powershell
dotnet build .\TheLastTestament.csproj -p:TML_INSTALL_PATH="D:\Games\Steam\steamapps\common\tModLoader"
```

## Локализация

Игровые строки находятся в `Localization/en-US_Mods.TheLastTestament.hjson` и `Localization/ru-RU_Mods.TheLastTestament.hjson`.

## Ветки и коммиты

Каждая функция разрабатывается в отдельной ветке. Сообщения коммитов — на русском. Текущая ветка основы: `feature/last-testament-foundation`.

---

**English:** The Last Testament is a narrative dark-fantasy and horror content mod for Terraria. The player is not a chosen savior; the mod reveals another layer of the world's history without rewriting Terraria lore. See the linked architecture, progression, lore, asset-pipeline, and testing notes for the current implementation status.
