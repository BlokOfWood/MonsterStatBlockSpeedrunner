using System.Reflection;
using System.Text;

namespace MonsterStatBlockSpeedrunner
{

    [AttributeUsage(AttributeTargets.Property)]
    public class Header : Attribute
    {
        string header;

        public Header(string _header)
        {
            header = _header;
        }

        public override string ToString() => header;
    }

    public class Dice
    {
        public Die Type { get; }
        public byte Amount { get; }

        public Dice(Die _die, byte _amount)
        {
            Type = _die;
            Amount = _amount;
        }

        public static implicit operator Dice((Die, byte) a) => new Dice(a.Item1, a.Item2);
        public static implicit operator Dice(KeyValuePair<Die, byte> a) => new Dice(a.Key, a.Value);
        public static implicit operator (Die, byte)(Dice a) => (a.Type, a.Amount);
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SpecialParse : Attribute
    {
        public bool PrintPropertyName { get; private set; }
        MethodInfo? method;

        public SpecialParse(string parserName, bool printPropertyName = true)
        {
            MethodInfo? _method = typeof(ObjectParsers).GetMethod(parserName, BindingFlags.Static | BindingFlags.Public);
            if (_method == null)
            {
                throw new Exception($"Invalid Object Parser Name: {parserName}");
            }
            method = _method;

            PrintPropertyName = printPropertyName;
        }

        public void Invoke(object x)
        {
            if (method != null)
                method.Invoke(null, new object[] { x });
        }
    }

    public class Helper
    {
        public static string PadBoth(string source, int length, char paddingChar, bool shouldHaveBorder = true)
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;

            if (shouldHaveBorder)
                return $"/{source.PadLeft(padLeft, paddingChar).PadRight(length, paddingChar)}/";
            else
                return source.PadLeft(padLeft, paddingChar).PadRight(length, paddingChar);
        }

        public static string ForceSign(int number)
            => number < 0 ? number.ToString() : $"+{number}";

        public static int CalculateAverage(List<Dice> DiceArray) =>
            (int)DiceArray.Sum(x => x.Amount * ((double)x.Type / 2 + 0.5));

        public static string DiceToString(List<Dice> DiceArray)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < DiceArray.Count; i++)
            {
                sb.Append(DiceArray[i].Amount);
                if (DiceArray[i].Type != Die.Flat)
                    sb.Append(Enum.GetName(DiceArray[i].Type));

                if (i < DiceArray.Count - 1)
                    sb.Append(" + ");
            }

            return sb.ToString();
        }

        public static short CalculateModifer(byte _abilityScore)
            => (short)((_abilityScore - 10) / 2);
    }

    public static class ObjectParsers
    {
        public static void HitpointParser(object x)
        {
            if (x.GetType() != typeof(SortedDictionary<Die, byte>))
                throw new Exception($"Trying to call Hitpoint Parser with type: {x.GetType().Name}");

            List<Dice> HitPoints = ((SortedDictionary<Die, byte>)x).ToList().ConvertAll(x => new Dice(x.Key, x.Value));
            HitPoints.Reverse();

            if (HitPoints.Count == 0)
            {
                Console.Write("0");
                return;
            }

            Console.WriteLine($"{Helper.CalculateAverage(HitPoints)} ({Helper.DiceToString(HitPoints)})");
        }

        public static void MovementSpeedParser(object x)
        {
            if (x.GetType() != typeof(ushort[]))
                throw new Exception($"Trying to call Movement Speed Parser with type: {x.GetType().Name}");

            ushort[] MovementSpeedArray = (ushort[])x;

            if (MovementSpeedArray.Length != Enum.GetNames(typeof(MovementType)).Length)
                throw new Exception("Movement Speed Array array not with correct amount of elements.");

            Console.Write($"Speed {MovementSpeedArray[0]} ft.");

            //Prints Movement Speeds.
            for (int i = 1; i < MovementSpeedArray.Length; i++)
            {
                if (MovementSpeedArray[i] != 0)
                    Console.Write($", {(MovementType)i} {MovementSpeedArray[i]} ft.");
            }
            Console.WriteLine();
        }

        public static void AbilityScoresParser(object x)
        {
            if (x.GetType() != typeof(byte[]))
                throw new Exception($"Trying to call Ability Scores Parser with type: {x.GetType().Name}");

            byte[] AbilityScoresArray = (byte[])x;

            if (AbilityScoresArray.Length != Enum.GetNames(typeof(AbilityScore)).Length)
                throw new Exception("Ability Scores array not with correct amount of elements.");

            StringBuilder output = new StringBuilder();
            for (int i = 0; i < AbilityScoresArray.Length; i++)
            {
                output.Append(Helper.PadBoth(((AbilityScore)i).ToString(), 5, ' ', false));
            }
            Console.WriteLine(Helper.PadBoth(output.ToString(), 40, ' ', false));

            output.Clear();
            for (int i = 0; i < AbilityScoresArray.Length; i++)
            {
                output.Append(Helper.PadBoth(AbilityScoresArray[i].ToString(), 5, ' ', false));
            }
            Console.WriteLine(Helper.PadBoth(output.ToString(), 40, ' ', false));

            output.Clear();
            for (int i = 0; i < AbilityScoresArray.Length; i++)
            {
                int modifier = Helper.CalculateModifer(AbilityScoresArray[i]);
                output.Append(Helper.PadBoth((modifier >= 0 ? "+" : "") + modifier.ToString(), 5, ' ', false));
            }
            Console.WriteLine(Helper.PadBoth(output.ToString(), 40, ' ', false));
        }

        public static void ConditionImmunitiesParser(object x)
        {
            if (x.GetType() != typeof(SortedSet<Condition>))
                throw new Exception($"Trying to call Ability Scores Parser with type: {x.GetType().Name}");

            SortedSet<Condition> ConditionImmunitiesArray = (SortedSet<Condition>)x;

            bool first = true;
            foreach (Condition condition in ConditionImmunitiesArray)
            {
                if (!first)
                    Console.Write(", ");
                first = false;

                Console.Write(condition);
            }
            Console.WriteLine();
        }

        public static void StringArrayParser(object x)
        {
            if (x.GetType() != typeof(string[]))
                throw new Exception($"Trying to call String Array Parser with type: {x.GetType().Name}");

            string[] StringArray = (string[])x;

            foreach (string str in StringArray)
                Console.Write(str);
            Console.WriteLine();
        }

        public static void AbilityArrayParser(object x)
        {
            if (x.GetType() != typeof(List<Ability>))
                throw new Exception($"Trying to call Ability Array Parser with type: {x.GetType().Name}");

            List<Ability> AbilityArray = (List<Ability>)x;

            foreach (Ability ability in AbilityArray)
            {
                /* Print Name. */
                Console.Write($"\n{ability.Name}");

                /* Print Recharge if present. */
                if (ability.GetType() == typeof(Action) || ability.GetType() == typeof(Attack))
                {
                    Action action = (Action)ability;
                    if (action.Recharge != null)
                    {
                        (int min, int max) = action.Recharge.Value;
                        string rechargeText = min == max ? min.ToString() : $"{min}-{max}";
                        Console.Write($" (Recharge {rechargeText})");
                    }
                }

                /*Print header closing dot. */
                Console.Write(". ");

                if (ability.GetType() == typeof(Attack))
                {
                    Attack attack = (Attack)ability;
                    if (attack.AttackType == AttackType.Melee)
                        Console.Write("Melee Weapon Attack: ");
                    else
                        Console.Write("Ranged Weapon Attack: ");

                    Console.Write($"{Helper.ForceSign(attack.ToHit)} to hit, ");

                    if (attack.AttackType == AttackType.Melee)
                        Console.Write($"reach {attack.Reach.normalRange} ft., ");
                    else
                        Console.Write($"reach {attack.Reach.normalRange}/{attack.Reach.longRange} ft., ");

                    Console.Write("one target. ");
                    Console.Write($"Hit: ");

                    bool first = true;
                    if (attack.Damage.Length > 1)
                        foreach (Damage i in attack.Damage.Skip(1))
                        {
                            if (!first)
                                Console.Write(" plus ");
                            Console.Write($"{Helper.CalculateAverage(i.Dice)} ({Helper.DiceToString(i.Dice)}) {i.DamageType.ToString().ToLower()} damage");
                            first = false;
                        }

                    Console.WriteLine(".");
                }
                else
                {
                    Console.WriteLine(ability.Description);
                }
            }
            Console.WriteLine();
        }

        public static void ChallengeRatingParser(object x)
        {
            if (x.GetType() != typeof(short))
                throw new Exception($"Trying to call Challenge Rating Parser with type: {x.GetType().Name}");

            short ChallengeRating = (short)x;
            if (ChallengeRating >= 0)
                Console.WriteLine(ChallengeRating);
            else
                Console.WriteLine($"1/{Math.Pow(2, -ChallengeRating)}");
        }

        public static void ProficencyBonusParser(object x)
        {
            if (x.GetType() != typeof(short))
                throw new Exception($"Trying to call Proficency Bonus Parser with type: {x.GetType().Name}");

            short ProfiencyBonus = (short)x;
            Console.WriteLine($"{Helper.ForceSign(ProfiencyBonus)}");
        }
    }
}