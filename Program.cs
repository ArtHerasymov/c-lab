using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Net;

namespace SecondLab
{

    //Interfaces
 
    interface ISpells
    {
        void CastMainSpell();
        void CastAuxiliarySpell();
    }
    interface IData
    {
        void ShowProfile();
        void ShowStats();
    }
    interface ISickness
    {
        String Sickness
        {
            get;
            set;
        }
        Boolean IsVaccinated
        {
            get;
            set;
        }
    }
    interface IGenderSickness : ISickness
    {
        String GenderSpecificIllness { get; set; }
    }


    // Classes, designating characters' behavoir 

    abstract class Human : ISickness, IData
    {
        private int age;
        private double height;
        private int weight;
        private String sickness;
        private static int population;
        protected static readonly int currentYear;
        protected String firstName;
        protected String lastName;
        protected int health;
        private String genderSpecificIllness;


        static Human()
        {
            population = 0;
            currentYear = DateTime.Now.Year;
        }

        public Human()
        {
            this.firstName = "John";
            this.lastName = "Doe";
            this.health = 100;
            population++;
        }
        public Human(String firstName, String lastName, int yearOfBirth, double height, int weight)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.age = currentYear - yearOfBirth;
            this.height = height;
            this.weight = weight;
            this.health = 100;
            population++;
        }


        public double Height
        {
            get => height;
            set
            {
                if (value < 0 || value > 2.5)
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(value)} is usually between 0 and 2.5 only.");
                height = value;
            }
        }

        public int Weight
        {
            get => weight;
            set
            {
                if (value < 0 || value > 400)
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(value)} is usually between 0 and 400 only.");

                weight = value;
            }
        }

        //Expression-bodied member
        public String Name => $"{firstName} {lastName}";

        public String Sickness
        {
            get => sickness;
            set
            {
                sickness = value;
                Console.WriteLine(firstName + " " + lastName + " got " + value);
                health -= 10;
            }
        }

        public Boolean IsVaccinated
        {
            get => IsVaccinated;
            set
            {
                if(value == true)
                {
                    Console.WriteLine(firstName + " " + lastName + " is vaccinated for {0}. Cured.", sickness);
                    health += 10;
                } else
                {
                    Console.WriteLine(firstName + " " + lastName + " is not vaccinated for {0}.", sickness);
                }
            }
        }

        public string GenderSpecificIllness { get => genderSpecificIllness; set => genderSpecificIllness = value; }

        // Implementation of IData interface
        public void ShowProfile()
        {
            Console.WriteLine("Name : " + firstName + ", " + lastName);
            Console.WriteLine("Age: " + age + ", Height: " + height + ", Weight:" + weight);
        }

        public void ShowStats()
        {
            Console.WriteLine("Population : " + population);
        }
    }
    abstract class Male : Human, IGenderSickness, IData
    {
        private int strength;
        private int masculinity;


        public Male()
        {
            firstName = "John";
            lastName = "Doe";
        }

        public Male(String firstName, String lastName, int yearOfBirth, double height, int weight, int strength, int masculinity)
             : base(firstName, lastName, yearOfBirth, height, weight)
        {
            this.strength = strength;
            this.masculinity = masculinity;
        }

        public int Strength { get; set; }
        public int Masculinity { get; set; }


        public new void ShowProfile()
        {
            base.ShowProfile();
            Console.WriteLine("Strength : " + strength + ", Masculinity : " + masculinity);
        }

    }
    abstract class Female : Human, IGenderSickness, IData
    {
        private int attractiveness;
        private int stealth;

        public Female()
        {
            firstName = "Jane";
            lastName = "Doe";
        }

        public Female(String firstName, String lastName, int yearOfBirth, double height, int weight, int attractiveness, int stealth)
             : base(firstName, lastName, yearOfBirth, height, weight)
        {
            this.attractiveness = attractiveness;
            this.stealth = stealth;
        }

        public String GenderSpecificIllness
        {
            get => GenderSpecificIllness;
            set
            {
                Console.WriteLine(firstName + " " + lastName + " is sick with " + value+ "Attractiveness is depleted");
                attractiveness -= 100;
            }

        }

        public new void ShowProfile()
        {
            base.ShowProfile();
            Console.WriteLine("Attractiveness : " + attractiveness + ", Stealth : " + stealth);
        }

    }

    [Serializable]
    class Warrior : Male, ISpells, IData, IComparable<Warrior>, ISerializable, IDisposable
    {
        private String sword;
        private int agility;
        private int hit;
        private int missed;
        private static double warriorStats;
        private string[] enemies = new string[10];
        private List<string> allies;
        private int enemiesCount;
        StreamWriter streamWriter;
        WebRequest request;
        WebResponse response;
        Stream dataStream;
        StreamReader reader;
        string responseFromServer;
        //Named constants instead of magic numbers
        const int normalizationCoefficient = 50; // Coefficient designed for user-friendly display in client code
        const double healhReductionCoefficient = 2 * Math.PI / 4; // Statistically calculated coefficient based on arena dataset


        public int CompareTo(Warrior obj)
        {
            int compare;

            if (this.agility > obj.agility) compare = 1;
            else compare = -1;

            return compare;
        }

        public void GetObjectData(SerializationInfo info , StreamingContext context)
        {
            info.AddValue("agility", agility, typeof(int));
            info.AddValue("sword" , Sword, typeof(String));
        }

        public Warrior(SerializationInfo info , StreamingContext context)
        {
            agility = (int)info.GetValue("agility", typeof(int));
            Sword = (String)info.GetValue("sword", typeof(String));
        }


        static Warrior()
        {
            warriorStats = 1;
        }

        public Warrior()
            : base()
        {
            agility = 1;
            hit = 1;
            missed = 1;
            enemiesCount = 0;
            // Getting info about warrior from arena's public API
        }

        public Warrior(String firstName, String lastName, int yearOfBirth, double height, int weight,
            int strength, int masculinity, String sword, int agility, int hit)
            : base(firstName, lastName, yearOfBirth, height, weight, strength, masculinity)
        {
            try
            {
                if (agility > 100) throw new CheaterDisclosureException("agility", agility);
                this.Sword = sword;
                this.agility = agility;
                this.hit = hit;
                warriorStats += hit;
                enemiesCount = 0;
            }
            catch(CheaterDisclosureException)
            {
                agility = 0;
            }

            
        }

        public int Hit
        {
            get { return hit; }
            set
            {
                hit = value;
                warriorStats = hit / missed;
            }
        }

        public int Missed
        {
            get { return missed; }
            set
            {
                missed = value;
                warriorStats = hit / missed * normalizationCoefficient;
            }
        }

        public int Agility
        {
            get { return agility; }
            set
            {
                agility = value;
            }
        }

        public string Sword { get => sword; set => sword = value; }

        public void CastMainSpell()
        {
            Console.WriteLine("Warrior punches");
        }

        public void CastAuxiliarySpell()
        {
            Console.WriteLine("Warrior blocks");
        }

        public void AddEnemy(string enemy)
        {
            if(enemiesCount < 9)
            {
                this.enemies[enemiesCount] = enemy;
                this.enemiesCount++;
            } else
            {
                Console.WriteLine("Can't take any more enemies. Sorry :(");
            }
        }

        public void AddAlly(string ally)
        {
            this.allies.Add(ally);
        }

        Action<Arena, BattleEventArgs> fightAction;
        Func<bool> fightFunc;

        public void RequestDuel(Arena arena, BattleEventArgs bargs)
        {
            // According to research, average warrior is incapable of conducting a fight with the health rate below 30%
            if (bargs.healthOfWarrior < 30)   
            {
                fightAction = Retrieve;
            // Battle with stakes makes sense in case of both participants having full health charge
            } else if(bargs.healthOfWarrior == 100) {
                fightFunc = InitializeBattleWithStakes;
                if(fightFunc())
                {
                    // According to stats, average punch of a warrior constitutes 10% of overall health rate
                    health -= (int)(10 * healhReductionCoefficient);
                    Console.WriteLine("Warrior's health was reduced by 10");
                } else
                {
                    Console.WriteLine("Warrior remains undamaged");
                }

            }
            else
            {
                fightAction = Fight;
            }

        }

        public bool InitializeBattleWithStakes()
        {
            Console.WriteLine("Warrior" + firstName + " " + lastName + "is fighting with stakes!");
            Random rand = new Random();
            return rand.NextDouble() >= 0.5;
        }

        public void Retrieve(Arena arena, BattleEventArgs bargs)
        {
            Console.WriteLine("Warrior " + firstName + " " + lastName + " is declining to fight due to low health at arena " + arena.name);
        }

        public void Fight(Arena arena, BattleEventArgs bargs)
        {
                Console.WriteLine("Warrior " + firstName + " " + lastName + " is fighting at arena" + arena.name + ". High chances to win.");            
        }


        public new void ShowProfile()
        {
            Console.WriteLine("__________________________");
            base.ShowProfile();
            Console.WriteLine("Special skills: ");
            Console.WriteLine("Sword : " + Sword + ", agility : " + agility);
        }

        public void WriteWarriorInfo()
        {
            this.streamWriter.WriteLine(this.Strength);
        }

        public new void ShowStats()
        {
            Console.WriteLine("Warrior stats : " + warriorStats);
        }


        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            Console.WriteLine("dis");
            if (disposed) return;
            if(disposing)
            {
                Console.WriteLine($"Warrior {firstName} cleared");
            }
            disposed = true;
 
        }

        ~Warrior() {
            Console.WriteLine("~");
            Dispose(true);
        }
    }
    class Priestess : Female, ISpells, IData
    {
        private String staff;
        private int knowledge;
        private int distance;
        private int healed;
        private static double priestStats;

        static Priestess() { priestStats = 1; }

        public int Knowledge
        {
            get { return Knowledge1; }
            set { Knowledge1 = value; }
        }

        public int Distance
        {
            get { return Distance1; }
            set { Distance1 = value; }
        }

        public int Healed
        {
            get { return Healed1; }
            set
            {
                Healed1 = value;
                priestStats++;
            }
        }

        public string Staff { get => staff; set => staff = value; }
        public int Knowledge1 { get => knowledge; set => knowledge = value; }
        public int Distance1 { get => distance; set => distance = value; }
        public int Healed1 { get => healed; set => healed = value; }

        public Priestess(String firstName, String lastName, int yearOfBirth, double height, int weight,
            int attractiveness, int stealth, String staff, int knowledge, int distance, int healed)
            : base(firstName, lastName, yearOfBirth, height, weight, attractiveness, stealth)
        {

            try
            {
                if (knowledge > 100) throw new CheaterDisclosureException("knowledge", knowledge);

                this.Knowledge1 = knowledge;
                this.Distance1 = distance;
                this.Staff = staff;
                this.Healed1 = healed;
                priestStats += healed;
            }
            catch(CheaterDisclosureException)
            {
                this.Knowledge1 = 100;
            }
           
        }

        public Priestess()
        {
        }

        public void CastMainSpell()
        {
            Console.WriteLine("Priestess conjures");
        }

        public void CastAuxiliarySpell()
        {
            Console.WriteLine("Priestess heals");
        }

        Action<Arena, BattleEventArgs> fightAction;
        Func<bool> fightFunc;

        public void RequestDuel(Arena arena, BattleEventArgs bargs)
        {
            if (bargs.healthOfPriestess < 40)
            {
                fightAction = Retrieve;
            }
            else if (bargs.healthOfPriestess == 100)
            {
                fightFunc = InitializeBattleWithStakes;
                if (fightFunc())
                {
                    this.health -= 5; // Women rule
                    Console.WriteLine("Priestess's health was reduced by 5");
                }
                else
                {
                    Console.WriteLine("Priestess remains undamaged");
                }

            }
            else
            {
                fightAction = Fight;
            }

        }

        public bool InitializeBattleWithStakes()
        {
            Console.WriteLine("Warrior" + firstName + " " + lastName + "is fighting with stakes!");
            Random rand = new Random();
            return rand.NextDouble() >= 0.5;
        }

        public void Retrieve(Arena arena, BattleEventArgs bargs)
        {
            Console.WriteLine("Warrior " + firstName + " " + lastName + " is declining to fight due to low health at arena " + arena.name);
        }

        public void Fight(Arena arena, BattleEventArgs bargs)
        {
            Console.WriteLine("Warrior " + firstName + " " + lastName + " is fighting at arena" + arena.name + ". High chances to win.");
        }

        public new void ShowProfile()
        {
            Console.WriteLine("__________________________");
            base.ShowProfile();
            Console.WriteLine("Special skills:");
            Console.WriteLine("Staff:" + Staff + "knowledge: " + Knowledge1 + "distance of heal: " + Distance1);
        }

        public new void ShowStats()
        {
            Console.WriteLine("Priestess stats : " + priestStats);
            Console.WriteLine("__________________________");
        }


    }
    class Village
    {
        // Unambiguous(descriptive names) names
        private static int population;
        private static int warriorsCount;
        private static int priestsCount;
        private static double warriorsStats;
        private static double priestsStats;

        private Village() { }

        //Function names should say what they do
        public static void AddWarrior(Warrior warrior)
        {
            warriorsCount++;
            population++;
            warriorsStats += warrior.Hit;
        }

        public static void AddPriestess(Priestess priest)
        {
            priestsCount++;
            population++;
            priestsStats += priest.Healed;
        }
        public static void ShowVillageStats()
        {
            Console.WriteLine("__________________________");
            Console.WriteLine("Population : " + population);
            Console.WriteLine("Stats of priestesses : " + priestsStats);
            Console.WriteLine("Stats of warriors : " + warriorsStats);
        }


        //Encapsulate Conditionals
        public static int DetermineBetterFighter(Warrior leftOponent, Warrior rightOponent)
        {
            if (leftOponent.Agility > rightOponent.Agility) return 1;
            else if (leftOponent.Agility < rightOponent.Agility) return 0;
            else return -1;
        }

        public static int DetermineBetterHealer(Priestess leftHealer, Priestess rightHealer)
        {
            if (leftHealer.Distance + leftHealer.Knowledge > rightHealer.Distance + rightHealer.Knowledge) return 1;
            else if (leftHealer.Distance + leftHealer.Knowledge < rightHealer.Distance + rightHealer.Knowledge) return 0;
            else return -1;
        }

    }


    // Classes required for event system, lambda expressions and anonymous methods

    public class BattleEventArgs : EventArgs
    {
        public int duration;
        public int healthOfWarrior;
        public int healthOfPriestess;
        public int currentTime = 0;

        public BattleEventArgs(int duration,
            int healthOfWarrior, int healthOfPriestess)
        {
            try
            {
                if (duration < 0 || duration > 20) throw new InvalidDurationOfBattleException(duration);
                this.duration = duration;
                this.healthOfWarrior = healthOfWarrior;
                this.healthOfPriestess = healthOfPriestess;
            } 
            catch(InvalidDurationOfBattleException)
            {
                this.duration = 5;
            }
        }

        public BattleEventArgs() : this(60, 100, 100) { }
    }

    public delegate void BattleHandle(Arena a, BattleEventArgs bargs);

    public class Arena
    {
        public event BattleHandle BattleOnArenaEvent;
        public String name;

        public Arena(string name)
        {
            this.name = name;
        }

        public void Fight()
        {
            int warriorInitialHealth, priestessInitialHealth, duration;
            BattleEventArgs bargs;
            try
            {
                Console.Write("Enter the duration of the fight : ");
                duration = Int32.Parse(Console.ReadLine());
                Console.Write("Enter initial health of the warrior: ");
                warriorInitialHealth = Int32.Parse(Console.ReadLine());
                Console.Write("Enter initial health of the priestess: ");
                priestessInitialHealth = Int32.Parse(Console.ReadLine());
                bargs = new BattleEventArgs(duration, 100, 100);
            }
            catch
            {
                bargs = new BattleEventArgs();
            }

            Console.WriteLine("Arena {0} is hosting the battle for {1} rounds...\n", this.name, bargs.duration);
            if (BattleOnArenaEvent != null)
                BattleOnArenaEvent((Arena)this, bargs);
;        }
    }

    class Lockroom
    {
        private Warrior firstCompetitor;
        private Priestess secondCompetitor;

        public Lockroom(Arena arena, Warrior firstCompetitor, Priestess secondCompetitor)
        {

            Action<Arena, BattleEventArgs> battleAction;
            battleAction = delegate (Arena a, BattleEventArgs bargs) { };

            
            arena.BattleOnArenaEvent += delegate (Arena a, BattleEventArgs bargs) {
                Console.WriteLine("Participant 1 is ready to fight!");
                firstCompetitor.RequestDuel(a, bargs);
            };

            arena.BattleOnArenaEvent += (a, bargs) => {
                Console.WriteLine("Participant 2 is ready to fight");
                secondCompetitor.RequestDuel(a, bargs);
            }; ;
        }

        internal Warrior FirstCompetitor { get => firstCompetitor; set => firstCompetitor = value; }
        internal Priestess SecondCompetitor { get => secondCompetitor; set => secondCompetitor = value; }
    }

    // Custom exception handling 
    public class CheaterDisclosureException: Exception
    {
        public CheaterDisclosureException(String nameOfInvalidArgument , int valueOfInvalidArgument) 
        {
            Console.WriteLine("CheaterDisclosureException: invalid {0} parameter : {1} ", nameOfInvalidArgument, valueOfInvalidArgument);
        }
    }

    public class InvalidDurationOfBattleException : Exception
    {
        public InvalidDurationOfBattleException(int valueOfInvalidArgument)
        {
            Console.WriteLine("InvalidBattleParametersException : invalid duration {0}", valueOfInvalidArgument);
        }
    }


    //Enum with combined values
    [Flags]
    enum Attack {
        FIRE_THROW = 0,
        ICE_PUNCH,
        MAGIC_BOLT = 2,
        ACCELERATED_PUNCH = 4,
        PETRIFICATION = 8,

        FIRETHROW_MAGICBOLT = FIRE_THROW | MAGIC_BOLT,
        ACCELERATEDPUNCH_PETRIFICTATION = ACCELERATED_PUNCH | PETRIFICATION
    };

    //Custom collection and implementation of INumerator, INumerable
    class Army: System.Collections.IEnumerable
    {
        Warrior[] _army = {
            new Warrior("Arthur", "King", 1998 , 190, 70, 100, 100, "SuperSword" , 80, 12),
            new Warrior("Bill", "Murrey", 1998 , 190, 70, 100, 100, "MegaSword" , 76 ,12),
            new Warrior("Johny", "Depp", 1998 , 190, 70, 100, 100, "DuperSword" , 45, 12)
    };

        public Warrior this[string sword]
        {
            get
            {
                foreach (Warrior w in _army)
                {
                    Console.WriteLine(w.Sword);
                    if (w.Sword.Equals(sword)) return w;
                }
                return null;
            }
            set
            {
                foreach (Warrior w in _army)
                {
                    if (w.Sword == sword) w.Sword = sword;
                }
            }
        }
  
        public System.Collections.IEnumerator GetEnumerator()
        {
            return new ArmyEnumerator(_army);
        }

        class ArmyEnumerator : System.Collections.IEnumerator
        {
            private Warrior[] _army;
            
            private int _position = -1;

            public ArmyEnumerator(Warrior[] army)
            {
                _army = army;
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return _army[_position];
                }
            }

            bool System.Collections.IEnumerator.MoveNext()
            {
                _position++;
                return (_position < _army.Length);
            }

            void System.Collections.IEnumerator.Reset()
            {
                _position = -1;
            }
        }
    }

    static class LINQExtension
    {
        public static void PrintOut(this System.Collections.IEnumerable source)
        {
            foreach(Object a in source)
            {
                Console.WriteLine("Element of IEnumerable : " + a.ToString()); 
            }
        }

        public static void ShowMoto(this Army source)
        {
            Console.WriteLine("MY MOTO!!");
        } 
    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start of program");
            WeakReference[] references = new WeakReference[1000];

            for(int i = 0; i < references.Length; i++)
            {
                Warrior w = new Warrior();
                references[i] = new WeakReference(w);  
                w = null;
                GC.WaitForPendingFinalizers();
            }

            Console.WriteLine("After GC.Collect is invoked : {0}", GC.GetTotalMemory(true));

            Warrior[] warriors = new Warrior[references.Length];

            for(int i = 0; i < references.Length; i++)
            {
                warriors[i] = (Warrior)references[i].Target;
            }

            Console.WriteLine("End of program");
            
            Console.ReadKey();
        }

        public static void SerializeItem(string filename, IFormatter formatter)
        {
            Warrior w = new Warrior();
            w.Sword = "The sword of death";

            Warrior[] warr =
            {
                new Warrior(),
                new Warrior(),
                new Warrior()
            };
          
            warr[0].Sword = "kek";
            warr[1].Sword = "lol";
            warr[2].Sword = "cheburek";
            

            FileStream fileStream = new FileStream(filename, FileMode.Create);
            formatter.Serialize(fileStream, warr);
            fileStream.Close();
            Console.WriteLine("Item serialized");
        }

        public static void DeserializeItem(string fileName , IFormatter formatter)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            Warrior[] warriorArray = (Warrior[])formatter.Deserialize(fileStream);
            foreach(Warrior war in warriorArray)
            {
            }
        }
    }
}
