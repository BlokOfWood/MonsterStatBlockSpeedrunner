namespace MonsterStatBlockSpeedrunner
{
    public class Ability
    {
        public string Name { get; }
        public string Description { get; }

        public Ability(string _name, string _description)
        {
            Name = _name;
            Description = _description;
        }
    }
    
    public class Action : Ability
    {
        public (byte Min, byte Max)? Recharge = null;  

        public Action(string _name, string _description, (byte _min, byte _max)? _recharge = null) : base(_name, _description)
        {
            if (_recharge != null)
            {
                if (_recharge.Value._min < 1)
                    throw new Exception($"Minimum recharge of Action {_name} less than 2");
                if (_recharge.Value._max > 6)
                    throw new Exception($"Maximum recharge of Action {_name} bigger than 6");
                if (_recharge.Value._min > _recharge.Value._max)
                    throw new Exception($"Minimum recharge of Action {_name} bigger than its maximum recharge");
                if (_recharge.Value._min == 1 && _recharge.Value._max == 6)
                    throw new Exception($"Recharge for Action {_name} will always trigger.");

                Recharge = _recharge;
            }
        }
    }

    public class Damage
    {
        public DamageType DamageType;
        public List<Dice> Dice;

        public Damage(DamageType _damageType, Dice[] _dice)
        {
            DamageType = _damageType;
            Dice = _dice.ToList();
        }
    }

    public class Attack : Action
    {
        public AttackType AttackType 
        { get => (Reach.normalRange == Reach.longRange) ? AttackType.Melee : AttackType.Ranged; }
        public AbilityScore UsedAbility;
        private short? overrideToHit;
        public short ToHit
        {
            get
            {
                if (overrideToHit == null)
                    return (short)(Helper.CalculateModifer(Program.Monster.AbilityScores[(int)UsedAbility]) + Program.Monster.ProficencyBonus.Value);
                else
                    return overrideToHit.Value;
            }
        }
        public (ushort normalRange, ushort longRange) Reach;
        public Damage[] Damage;
        

        public Attack(string _name, AbilityScore _usedAbility, Damage[] _damage, ushort _normalReach = 5, ushort _longReach = 5, (byte, byte)? _recharge = null, short? _toHit = null) : base(_name, "", _recharge)
        {
            overrideToHit = _toHit;
            UsedAbility = _usedAbility;
            Damage = _damage;


            Reach = (_normalReach, _longReach);
            if(_normalReach > _longReach)
                throw new Exception($"The normal reach given to Action {_name} is longer, than the long reach. ");
        }
    }
}