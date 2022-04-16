using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterStatBlockSpeedrunner
{
    public enum AbilityScore
    {
        STR = 0,
        DEX = 1,
        CON = 2,
        INT = 3,
        WIS = 4,
        CHA = 5
    }

    public enum AttackType
    {
        Melee = 0,
        Ranged = 1
    }

    public enum Condition
    {
        Blinded = 0,
        Charmed = 1,
        Deafened = 2,
        Exhaustion = 3,
        Frightened = 4,
        Grappled = 5,
        Incapacitated = 6,
        Invisible = 7,
        Paralyzed = 8,
        Petrified = 9,
        Poisoned = 10,
        Prone = 11,
        Restrained = 12,
        Stunned = 13,
        Unconscious = 14,
    }

    public enum CreatureSize
    {
        Tiny = 0,
        Small = 1,
        Medium = 2,
        Large = 3,
        Huge = 4,
        Gargantuan = 5
    }

    public enum CreatureType
    {
        Aberration = 0,
        Beast = 1,
        Celestial = 2,
        Construct = 3,
        Dragon = 4,
        Elemental = 5,
        Fey = 6,
        Fiend = 7,
        Giant = 8,
        Humanoid = 9,
        Monstrosity = 10,
        Ooze = 11,
        Plant = 12,
        Undead = 13
    }

    public enum DamageType
    {
        Acid = 0,
        Bludgeoning = 1,
        Cold = 2,
        Fire = 3,
        Force = 4,
        Lightning = 5,
        Necrotic = 6,
        Piercing = 7,
        Poison = 8,
        Psychic = 9,
        Radiant = 10,
        Slashing = 11,
        Thunder = 12,
    }

    public enum Die
    {
        Flat = 1,
        d4 = 4,
        d6 = 6,
        d8 = 8,
        d10 = 10,
        d12 = 12,
        d20 = 20
    }

    public enum MovementType
    {
        Walk = 0,
        Burrow = 1,
        Climb = 2,
        Fly = 3,
        Swim = 4,
    }
}
