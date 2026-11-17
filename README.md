# The Last Testament

**The Last Testament** — сюжетный мод для Terraria в жанре тёмного фэнтези и хоррора. Мир продолжает жить на останках погибших богов, а их сны всё ещё достигают живых.

Это первая рабочая основа вертикального среза на tModLoader 1.4.4. Код и внутренние идентификаторы — на английском; игровые строки локализуются на русский и английский.

## Состояние проекта

- Проект переименован из старого эксперимента `Alternative Troubles`.
- Настроена сборка для стабильного tModLoader v2026.09.3.1 и .NET 8.
- Добавлены независимые типы урона Оккультиста и Призрака, модель ресурсов, конфигурация баланса и сохранение прогресса.
- Добавлен детерминированный планировщик города для сна: одинаковый seed даёт одинаковую планировку; позже эти же данные сможет использовать доступный игровой подмир.
- Старые шутливые предметы и незавершённый пример босса удалены из активного мода. Исходный коммит остаётся в истории Git.
- Добавлены первые игровые AI трёх противников Проклятой рощи: Сухокорень, Свечная моль и Пустой паломник. Они пока появляются ночью в лесу до появления отдельного биома.
- Добавлен временный ванильный лут: древесина, пепельный блок и каменный блок.
- Сборка с новыми противниками прошла. Запуск в Terraria и игровые сценарии ещё не проверялись.
- Художественные направления Исповедника и Серафима согласованы на уровне концепт-превью; финальные спрайты и листы анимаций будут пиксельными и проходят отдельное согласование.

См. [архитектуру](docs/ARCHITECTURE.md), [предложение по визуальному стилю](docs/ART_DIRECTION_PROPOSAL.md), [исследование Subworld Library](docs/SUBWORLDS.md) и [статус проверок](docs/TESTING.md).

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
