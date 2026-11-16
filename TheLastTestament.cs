using System.IO;
using Terraria;
using Terraria.ModLoader;
using TheLastTestament.Common.Players;

namespace TheLastTestament;

public sealed class TheLastTestament : Mod
{
    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        PacketType packetType = (PacketType)reader.ReadByte();
        if (packetType != PacketType.SyncClassResources)
            return;

        int playerIndex = reader.ReadByte();
        int corruption = reader.ReadInt16();
        int ether = reader.ReadInt16();
        int phaseCooldown = reader.ReadInt16();
        bool isPhasing = reader.ReadBoolean();

        if (playerIndex < 0 || playerIndex >= Main.maxPlayers || !Main.player[playerIndex].active)
            return;

        if (Main.netMode == Terraria.ID.NetmodeID.Server)
        {
            // A client may only update its own resource state.
            if (whoAmI != playerIndex)
                return;

            Main.player[playerIndex].GetModPlayer<ClassResourcePlayer>()
                .ApplyNetworkState(corruption, ether, phaseCooldown, isPhasing);
            SendResourceState(playerIndex, whoAmI);
        }
        else
        {
            Main.player[playerIndex].GetModPlayer<ClassResourcePlayer>()
                .ApplyNetworkState(corruption, ether, phaseCooldown, isPhasing);
        }
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
}

internal enum PacketType : byte
{
    SyncClassResources = 1
}
