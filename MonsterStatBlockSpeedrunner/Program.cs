using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonsterStatBlockSpeedrunner
{    
    public class Monster
    {
        [Header("Details")]
        public string? Name { get; }
        public CreatureSize? Size { get; }
        public CreatureType? Type { get; }
        public string? Alignment { get; }

        [Header("Stats")]

        public byte? ArmorClass { get; private set; }

        [SpecialParse("HitpointParser")]
        public SortedDictionary<Die, byte>? HitPoints { get; }

        [SpecialParse("MovementSpeedParser", false)]
        public ushort[]? MovementSpeed { get; }

        [Header("Ability Scores")]
        [SpecialParse("AbilityScoresParser", false)]
        public byte[] AbilityScores { get; }

        [Header("Additional Details")]
        [SpecialParse("StringArrayParser")]
        public string[] DamageResistances { get; }
        [SpecialParse("StringArrayParser")]
        public string[] DamageImmunities { get; }
        [SpecialParse("ConditionImmunitiesParser")]
        public SortedSet<Condition> ConditionImmunities { get; }
        [SpecialParse("StringArrayParser")]
        public string[] Languages { get; }
        [SpecialParse("ChallengeRatingParser")]
        public short? ChallengeRating { get; }
        [SpecialParse("ProficencyBonusParser")]
        public short? ProficencyBonus 
        {
            get 
            {
                if (ChallengeRating == null)
                    return null;

                return (short)(ChallengeRating < 0
                ? 2
                : 1 + MathF.Ceiling(ChallengeRating.Value / 4f));
            }
        }

        [Header("Traits")]
        [SpecialParse("AbilityArrayParser", false)]
        public List<Ability> Traits { get; }

        [Header("Actions")]
        [SpecialParse("AbilityArrayParser", false)]
        public List<Ability> Actions { get; }

        public Monster()
        {
            HitPoints = new SortedDictionary<Die, byte> { { Die.d12, 5 }, { Die.d10, 4 }, { Die.Flat, 5 } };
            MovementSpeed = new ushort[] { 0, 0, 30, 0, 0 };
            AbilityScores = new byte[] { 18, 19, 17, 10, 10, 10 };
            Type = CreatureType.Aberration;
            ConditionImmunities = new SortedSet<Condition>() { Condition.Blinded, Condition.Incapacitated, Condition.Charmed };
            DamageResistances = new string[] { "Force, Bludgeoning" };
            ChallengeRating = 5;

            Traits = new List<Ability>();
            Actions = new List<Ability>();
        }

        public void DisplaySheet()
        {
            Console.Clear();
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                /* Display Headers */
                Header? header = (Header?)property.GetCustomAttribute(typeof(Header), false);
                if (header != null)
                {
                    Console.WriteLine(Helper.PadBoth(header.ToString(), 40, '-'));
                }

                /* Creates the name of the Property*/
                StringBuilder propertyName = new StringBuilder();
                propertyName.Append(property.Name[0]);

                /* Divides the names into words where Upper Cases are. */
                for (int i = 1; i < property.Name.Length; i++)
                {
                    if (char.IsUpper(property.Name[i]))
                    {
                        propertyName.Append(' ');
                    }
                    propertyName.Append(property.Name[i]);
                }

                /* Handle null value case*/
                object? value = property.GetValue(this, null);
                if (value == null)
                {
                    Console.WriteLine($"{propertyName}: Unknown");
                    continue;
                }

                /* Handle Special Parser */
                SpecialParse? parser = (SpecialParse?)property.GetCustomAttribute(typeof(SpecialParse), false);
                if (parser != null)
                {
                    if(parser.PrintPropertyName)
                        Console.Write(propertyName + ": ");
                    parser.Invoke(value);
                }
                else
                    Console.WriteLine($"{propertyName}: {property.GetValue(this, null)}");
            }
        }
    }

    public class Program
    {
        public static Monster Monster;

        static void Main()
        {
            Monster = new Monster();

            Damage[] damages = new Damage[] { new Damage(DamageType.Bludgeoning, new Dice[] { new Dice(Die.d6, 5) }), 
                new Damage(DamageType.Radiant, new Dice[] { new Dice(Die.d6, 5) }), 
                new Damage(DamageType.Cold, new Dice[] { new Dice(Die.d8, 5), new Dice(Die.Flat, 3) }) };
            Monster.Actions.Add( new Attack("Bite", AbilityScore.STR, damages, 5, 30, (5,6), 16));
            Monster.Actions.Add(new Attack("Bite", AbilityScore.STR, damages));
            Monster.Actions.Add(new Attack("Bite", AbilityScore.STR, damages));

            Monster.DisplaySheet();
        }

        static string Ask(string question)
        {
            Console.WriteLine(question);

            string? answer = Console.ReadLine();
            if (answer == null)
            {
                Console.WriteLine("oopsie, no value");
                throw new Exception("NULL CONSOLE.READLINE()!");
            }

            return answer;
        }
    }
}