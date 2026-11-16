using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheLastTestament.Common.Systems;

namespace TheLastTestament.Common.Players;

public sealed class ClassResourcePlayer : ModPlayer
{
    public int Corruption { get; private set; }
    public int Ether { get; private set; }
    public int PhaseCooldown { get; private set; }
    public bool IsPhasing { get; private set; }
    private int phaseTimeLeft;

    public bool TrySpendCorruption(int amount)
    {
        if (amount < 0 || Corruption < amount)
            return false;

        Corruption -= amount;
        SyncIfServer();
        return true;
    }

    public void AddCorruption(int amount)
    {
        if (amount > 0)
        {
            Corruption = Utils.Clamp(Corruption + amount, 0, ModContent.GetInstance<BalanceConfig>().CorruptionMaximum);
            SyncIfServer();
        }
    }

    public void ReduceCorruption(int amount)
    {
        if (amount > 0)
        {
            Corruption = Utils.Clamp(Corruption - amount, 0, ModContent.GetInstance<BalanceConfig>().CorruptionMaximum);
            SyncIfServer();
        }
    }

    public bool TryBeginPhase(int etherCost, int durationTicks, int cooldownTicks)
    {
        if (IsPhasing || PhaseCooldown > 0 || etherCost < 0 || durationTicks <= 0 || cooldownTicks <= 0 || Ether < etherCost)
            return false;

        Ether -= etherCost;
        IsPhasing = true;
        phaseTimeLeft = durationTicks;
        PhaseCooldown = durationTicks + cooldownTicks;
        SyncIfServer();
        return true;
    }

    public void AddEther(int amount)
    {
        if (amount > 0)
            Ether = Utils.Clamp(Ether + amount, 0, ModContent.GetInstance<BalanceConfig>().EtherMaximum);
    }

    public override void PostUpdate()
    {
        if (IsPhasing && --phaseTimeLeft <= 0)
            IsPhasing = false;

        if (PhaseCooldown > 0)
            PhaseCooldown--;

        int etherRegenInterval = System.Math.Max(1, ModContent.GetInstance<BalanceConfig>().EtherRegenInterval);
        if (!IsPhasing && Main.GameUpdateCount % (ulong)etherRegenInterval == 0)
            AddEther(1);
    }

    public override void PostUpdateEquips()
    {
        BalanceConfig config = ModContent.GetInstance<BalanceConfig>();
        if (Corruption >= config.CorruptionDangerThreshold)
            Player.statDefense -= config.HighCorruptionDefensePenalty;
    }

    public override void Initialize()
    {
        Corruption = 0;
        Ether = ModContent.GetInstance<BalanceConfig>().EtherMaximum;
        PhaseCooldown = 0;
        IsPhasing = false;
        phaseTimeLeft = 0;
    }

    public override void UpdateDead()
    {
        IsPhasing = false;
        phaseTimeLeft = 0;
        PhaseCooldown = 0;
    }

    public override void SaveData(TagCompound tag)
    {
        if (Corruption > 0)
            tag[nameof(Corruption)] = Corruption;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(Corruption)))
            Corruption = Utils.Clamp(tag.GetInt(nameof(Corruption)), 0, ModContent.GetInstance<BalanceConfig>().CorruptionMaximum);
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        ClassResourcePlayer copy = (ClassResourcePlayer)targetCopy;
        copy.Corruption = Corruption;
        copy.Ether = Ether;
        copy.PhaseCooldown = PhaseCooldown;
        copy.IsPhasing = IsPhasing;
        copy.phaseTimeLeft = phaseTimeLeft;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        ClassResourcePlayer previous = (ClassResourcePlayer)clientPlayer;
        bool meaningfulChange = Corruption != previous.Corruption
            || System.Math.Abs(Ether - previous.Ether) >= 5
            || IsPhasing != previous.IsPhasing
            || (PhaseCooldown == 0) != (previous.PhaseCooldown == 0);

        if (meaningfulChange && Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer)
            TheLastTestament.SendResourceState(Player.whoAmI);
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI != Main.myPlayer)
            return;

        TheLastTestament.SendResourceState(Player.whoAmI, fromWho, toWho);
    }

    public void ApplyNetworkState(int corruption, int ether, int phaseCooldown, bool isPhasing)
    {
        BalanceConfig config = ModContent.GetInstance<BalanceConfig>();
        Corruption = Utils.Clamp(corruption, 0, config.CorruptionMaximum);
        Ether = Utils.Clamp(ether, 0, config.EtherMaximum);
        PhaseCooldown = Utils.Clamp(phaseCooldown, 0, config.PhaseDurationTicks + config.PhaseCooldownTicks);
        IsPhasing = isPhasing;
        phaseTimeLeft = isPhasing ? System.Math.Min(PhaseCooldown, config.PhaseDurationTicks) : 0;
    }

    private void SyncIfServer()
    {
        if (Main.netMode == NetmodeID.Server)
            TheLastTestament.SendResourceState(Player.whoAmI, Player.whoAmI);
    }
}
