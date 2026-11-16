using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TheLastTestament.Common.Systems;

public sealed class WorldProgressSystem : ModSystem
{
    public static bool DownedWeepingSeraph { get; private set; }
    public static bool ConfessorDreamSeen { get; private set; }
    public static bool HomeAltarPlaced { get; private set; }
    public static byte ConfessorArrivalStage { get; private set; }
    public static int LostCitySeed { get; private set; }

    public static void MarkWeepingSeraphDowned() => DownedWeepingSeraph = true;
    public static void MarkConfessorDreamSeen() => ConfessorDreamSeen = true;
    public static void MarkHomeAltarPlaced() => HomeAltarPlaced = true;

    public static void AdvanceConfessorArrival()
        => ConfessorArrivalStage = (byte)System.Math.Min(ConfessorArrivalStage + 1, 3);

    public override void ClearWorld()
    {
        DownedWeepingSeraph = false;
        ConfessorDreamSeen = false;
        HomeAltarPlaced = false;
        ConfessorArrivalStage = 0;
        LostCitySeed = 0;
    }

    public override void OnWorldLoad()
    {
        if (LostCitySeed == 0)
            LostCitySeed = Main.worldID;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (DownedWeepingSeraph)
            tag[nameof(DownedWeepingSeraph)] = true;

        if (ConfessorDreamSeen)
            tag[nameof(ConfessorDreamSeen)] = true;

        if (HomeAltarPlaced)
            tag[nameof(HomeAltarPlaced)] = true;

        tag[nameof(ConfessorArrivalStage)] = ConfessorArrivalStage;
        tag[nameof(LostCitySeed)] = LostCitySeed;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        DownedWeepingSeraph = tag.ContainsKey(nameof(DownedWeepingSeraph));
        ConfessorDreamSeen = tag.ContainsKey(nameof(ConfessorDreamSeen));
        HomeAltarPlaced = tag.ContainsKey(nameof(HomeAltarPlaced));
        ConfessorArrivalStage = (byte)System.Math.Clamp((int)tag.GetByte(nameof(ConfessorArrivalStage)), 0, 3);
        LostCitySeed = tag.ContainsKey(nameof(LostCitySeed)) ? tag.GetInt(nameof(LostCitySeed)) : Main.worldID;
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(DownedWeepingSeraph);
        writer.Write(ConfessorDreamSeen);
        writer.Write(HomeAltarPlaced);
        writer.Write(ConfessorArrivalStage);
        writer.Write(LostCitySeed);
    }

    public override void NetReceive(BinaryReader reader)
    {
        DownedWeepingSeraph = reader.ReadBoolean();
        ConfessorDreamSeen = reader.ReadBoolean();
        HomeAltarPlaced = reader.ReadBoolean();
        ConfessorArrivalStage = (byte)System.Math.Clamp((int)reader.ReadByte(), 0, 3);
        LostCitySeed = reader.ReadInt32();
    }
}
