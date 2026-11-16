using Terraria.ModLoader.Config;

namespace TheLastTestament.Common.Systems;

public sealed class BalanceConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Range(50, 200)]
    public int CorruptionMaximum { get; set; } = 100;

    [Range(40, 180)]
    public int CorruptionDangerThreshold { get; set; } = 80;

    [Range(0, 20)]
    public int HighCorruptionDefensePenalty { get; set; } = 5;

    [Range(50, 200)]
    public int EtherMaximum { get; set; } = 100;

    [Range(1, 30)]
    public int EtherRegenInterval { get; set; } = 4;

    [Range(1, 60)]
    public int PhaseDurationTicks { get; set; } = 10;

    [Range(15, 180)]
    public int PhaseCooldownTicks { get; set; } = 40;

    [Range(1, 50)]
    public int PhaseEtherCost { get; set; } = 20;
}
