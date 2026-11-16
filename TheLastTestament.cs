using System.IO;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;
using TheLastTestament.Common.Players;
using TheLastTestament.Common.Systems;

namespace TheLastTestament;

public sealed class TheLastTestament : Mod
{
    internal static ModKeybind PhaseShiftKeybind { get; private set; } = null!;

    public override void Load()
    {
        PhaseShiftKeybind = KeybindLoader.RegisterKeybind(this, "PhaseShift", Keys.LeftShift);
    }

    public override void Unload()
    {
        PhaseShiftKeybind = null!;
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        PacketType packetType = (PacketType)reader.ReadByte();
        if (packetType == PacketType.RequestPhaseShift)
        {
            int requestingPlayer = reader.ReadByte();
            if (Main.netMode != Terraria.ID.NetmodeID.Server || whoAmI != requestingPlayer
                || requestingPlayer < 0 || requestingPlayer >= Main.maxPlayers
                || !Main.player[requestingPlayer].active)
                return;

            TryBeginPhase(Main.player[requestingPlayer]);
            return;
        }

        if (packetType != PacketType.SyncClassResources)
            return;

        // Resource state only flows from server to clients. Gameplay requests use
        // dedicated packets and are validated against the server's own state.
        if (Main.netMode == Terraria.ID.NetmodeID.Server)
            return;

        int playerIndex = reader.ReadByte();
        int corruption = reader.ReadInt16();
        int ether = reader.ReadInt16();
        int phaseCooldown = reader.ReadInt16();
        bool isPhasing = reader.ReadBoolean();

        if (playerIndex < 0 || playerIndex >= Main.maxPlayers || !Main.player[playerIndex].active)
            return;

        Main.player[playerIndex].GetModPlayer<ClassResourcePlayer>()
            .ApplyNetworkState(corruption, ether, phaseCooldown, isPhasing);
    }

    internal static void SendResourceState(int playerIndex, int ignoreClient = -1, int toClient = -1)
    {
        if (Main.netMode == Terraria.ID.NetmodeID.SinglePlayer)
            return;

        ClassResourcePlayer resource = Main.player[playerIndex].GetModPlayer<ClassResourcePlayer>();
        ModPacket packet = ModContent.GetInstance<TheLastTestament>().GetPacket();
        packet.Write((byte)PacketType.SyncClassResources);
        packet.Write((byte)playerIndex);
        packet.Write((short)resource.Corruption);
        packet.Write((short)resource.Ether);
        packet.Write((short)resource.PhaseCooldown);
        packet.Write(resource.IsPhasing);
        packet.Send(toClient, ignoreClient);
    }

    internal static void RequestPhaseShift(int playerIndex)
    {
        if (Main.netMode == Terraria.ID.NetmodeID.SinglePlayer)
        {
            TryBeginPhase(Main.player[playerIndex]);
            return;
        }

        if (Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
            return;

        ModPacket packet = ModContent.GetInstance<TheLastTestament>().GetPacket();
        packet.Write((byte)PacketType.RequestPhaseShift);
        packet.Write((byte)playerIndex);
        packet.Send();
    }

    private static void TryBeginPhase(Player player)
    {
        BalanceConfig config = ModContent.GetInstance<BalanceConfig>();
        ClassResourcePlayer resources = player.GetModPlayer<ClassResourcePlayer>();
        if (resources.TryBeginPhase(config.PhaseEtherCost, config.PhaseDurationTicks, config.PhaseCooldownTicks))
            SendResourceState(player.whoAmI);
        else if (Main.netMode == Terraria.ID.NetmodeID.Server)
            SendResourceState(player.whoAmI, toClient: player.whoAmI);
    }
}

internal enum PacketType : byte
{
    SyncClassResources = 1,
    RequestPhaseShift = 2
}
