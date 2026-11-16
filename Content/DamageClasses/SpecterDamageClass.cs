using Terraria.ModLoader;

namespace TheLastTestament.Content.DamageClasses;

public sealed class SpecterDamageClass : DamageClass
{
    public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        => damageClass == Generic ? StatInheritanceData.Full : StatInheritanceData.None;

    public override bool GetEffectInheritance(DamageClass damageClass)
        => damageClass == Generic;
}
