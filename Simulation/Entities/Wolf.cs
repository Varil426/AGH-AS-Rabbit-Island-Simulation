using System.Numerics;
using System.Threading;

namespace Simulation.Entities
{
    internal class Wolf : Creature
    {
        public Wolf(Vector2 position, World world) : base(position, world)
        {
            MaxHealth = StaticRandom.Generator.Next(90, 110);
            Health = MaxHealth;
            MaxEnergy = StaticRandom.Generator.Next(90, 110);
            Energy = MaxEnergy;
            SightRange = StaticRandom.Generator.Next(25, 50);
            MovementSpeed = StaticRandom.Generator.Next(5, 20);
            InteractionRange = StaticRandom.Generator.Next(4, 8);
            Attack = StaticRandom.Generator.Next(30, 120);

            int creditsLeft = World.WorldConfig.WolvesConfig.InitialPopulationCredits;

            var traits = new Dictionary<string, int>
            {
                { "MaxHealth", 20 },
                { "MaxEnergy", 20 },
                { "SightRange", 20 },
                { "MovementSpeed", 20 },
                { "InteractionRange", 20 }
            };

            while (creditsLeft > 10)
            {
                var selectedKey = RandomKeyFormDictionary(traits).First();
                var oldValue = traits[selectedKey];
                var newValue = oldValue + StaticRandom.Generator.Next(10);
                var additionCost = AdditionalCost(traits[selectedKey], newValue);
                creditsLeft -= additionCost;
                traits[selectedKey] = newValue;
            }

            traits.Add("Attack", StaticRandom.Generator.Next(1, creditsLeft));

            MaxHealth = traits["MaxHealth"];
            Health = MaxHealth;
            MaxEnergy = traits["MaxEnergy"];
            Energy = MaxEnergy;
            SightRange = traits["SightRange"] * 0.55;
            MovementSpeed = traits["MovementSpeed"] * 0.28;
            InteractionRange = traits["InteractionRange"] * 0.18;
            Attack = traits["Attack"] * 10;
        }

        public Wolf(Vector2 position, World world, uint generation, double maxHealth, double maxEnergy, double sightRange, double movementSpeed, double interactionRange, double attack) : base(position, world, generation)
        {
            MaxHealth = maxHealth;
            Health = MaxHealth;
            MaxEnergy = maxEnergy;
            Energy = MaxEnergy;
            SightRange = sightRange;
            MovementSpeed = movementSpeed;
            InteractionRange = interactionRange;
            Attack = attack;
        }

        public double Attack { get; }

        /*
        public override void DrawSelf(Canvas canvas)
        {
            var wolfCanvas = new Canvas();
            var color = IsAlive ? Brushes.Red : Brushes.Purple;
            var rectangle = new Rectangle()
            {
                Width = 1,
                Height = 1,
                Fill = color
            };
            wolfCanvas.Children.Add(rectangle);
            if (World.Instance.WorldConfig.DrawRanges && IsAlive)
            {
                var sightRange = new Ellipse()
                {
                    Width = SightRange,
                    Height = SightRange,
                    Stroke = color
                };
                wolfCanvas.Children.Add(sightRange);
                Canvas.SetLeft(sightRange, -SightRange / 2);
                Canvas.SetTop(sightRange, -SightRange / 2);
            }
            canvas.Children.Add(wolfCanvas);
            Canvas.SetLeft(wolfCanvas, Position.X);
            Canvas.SetTop(wolfCanvas, Position.Y);
        }
        */

        protected override void PerformAction(Action action)
        {
            switch (action.Type)
            {
                case ActionType.MoveTo:
                    Move(action.Target);
                    break;

                case ActionType.Attack:
                    if (action.Target is Creature creature)
                    {
                        creature.CreatureThread?.Interrupt();
                        creature.LoseHealth(Attack);
                    }
                    else
                    {
                        throw new Exception("Invalid target");
                    }
                    break;

                case ActionType.Eat:
                    Thread.Sleep(World.WorldConfig.WolvesConfig.EatingTimeScaled);
                    if (action.Target is Rabbit rabbit && !rabbit.IsAlive && World.RemoveEntity(action.Target))
                    {
                        var energy = Energy + 80;
                        if (energy > MaxEnergy)
                            energy = MaxEnergy;
                        Energy = energy;
                    }
                    break;

                case ActionType.Mate:
                    if (action.Target is Wolf anotherWolf)
                    {
                        if (anotherWolf.WaitingToMate)
                        {
                            anotherWolf.InteractionEvent.Set();
                            Thread.Sleep(World.WorldConfig.WolvesConfig.MatingTimeScaled);
                            if (Gender == GenderType.Female)
                            {
                                PregnantAt = DateTime.Now;
                                PregnantWith = anotherWolf;
                                States.Add(State.Pregnant);
                            }
                        }
                        else
                        {
                            States.Add(State.WaitingToMate);
                            if (InteractionEvent.WaitOne(World.WorldConfig.WolvesConfig.WaitToMateTimeScaled))
                            {
                                Thread.Sleep(World.WorldConfig.WolvesConfig.MatingTimeScaled);

                                if (Gender == GenderType.Female)
                                {
                                    PregnantAt = DateTime.Now;
                                    PregnantWith = anotherWolf;
                                    States.Add(State.Pregnant);
                                }
                            }
                            States.Remove(State.WaitingToMate);
                        }
                    }
                    break;

                case ActionType.Nothing:
                    break;

                default:
                    throw new Exception("Illegal action");
            }
        }

        protected override Action Think(List<Entity> closeByEntities)
        {
            // TODO Improve this
            if (Energy < MaxEnergy / 3)
            {
                if (States.Add(State.SearchingForFood))
                {
                    _movingSince = DateTime.Now;
                }
                if (closeByEntities.Find(entity => entity is Rabbit) is Rabbit rabbit)
                {
                    if (Vector2.Distance(this.Position, rabbit.Position) <= InteractionRange)
                    {
                        if (rabbit.IsAlive)
                        {
                            return new Action(ActionType.Attack, rabbit);
                        }
                        else
                        {
                            States.Remove(State.SearchingForFood);
                            return new Action(ActionType.Eat, rabbit);
                        }
                    }
                    return new Action(ActionType.MoveTo, rabbit);
                }
                else if (_movingSince.AddMilliseconds(World.WorldConfig.WolvesConfig.MoveInOneDirectionTimeScaled) <= DateTime.Now)
                {
                    var possibleValues = Enum.GetValues(typeof(RelativePosition.Direction)).Length;
                    var direction = (RelativePosition.Direction)StaticRandom.Generator.Next(possibleValues);
                    _movingSince = DateTime.Now;
                    _moveDirection = new RelativePosition(this, direction, World);
                    return new Action(ActionType.MoveTo, _moveDirection);
                }
                else
                {
                    return new Action(ActionType.MoveTo, _moveDirection);
                }
            }
            else if (closeByEntities.Find(entity => entity is Wolf wolf
                && wolf.IsAlive
                && wolf.CanMate
                && wolf.Gender != Gender
                && !wolf.IsPregnant
                && !IsPregnant) is Wolf anotherWolf)
            {
                States.Remove(State.SearchingForMatingPartner);
                if (this.Gender != anotherWolf.Gender)
                {
                    if (Vector2.Distance(this.Position, anotherWolf.Position) <= this.InteractionRange)
                    {
                        return new Action(ActionType.Mate, anotherWolf);
                    }
                    return new Action(ActionType.MoveTo, anotherWolf);
                }
            }
            else if (!States.Contains(State.Pregnant))
            {
                if (States.Add(State.SearchingForMatingPartner))
                {
                    _movingSince = DateTime.Now;
                }
                if (_movingSince.AddMilliseconds(World.WorldConfig.WolvesConfig.MoveInOneDirectionTimeScaled) <= DateTime.Now)
                {
                    var possibleValues = Enum.GetValues(typeof(RelativePosition.Direction)).Length;
                    var direction = (RelativePosition.Direction)StaticRandom.Generator.Next(possibleValues);
                    _movingSince = DateTime.Now;
                    _moveDirection = new RelativePosition(this, direction, World);
                    return new Action(ActionType.MoveTo, _moveDirection);
                }
                else
                {
                    return new Action(ActionType.MoveTo, _moveDirection);
                }
            }

            return new Action(ActionType.Nothing, this);
        }

        protected override void UpdatePregnancyStatus()
        {
            if (Gender == GenderType.Female
                && States.Contains(State.Pregnant)
                && PregnantAt.AddMilliseconds(World.WorldConfig.WolvesConfig.PregnancyTimeScaled) <= DateTime.Now
                && PregnantWith != null)
            {
                var offspring = World.GenerateOffspring(this, PregnantWith);
                foreach (Creature creature in offspring)
                {
                    World.AddCreature(creature);
                }
                States.Remove(State.Pregnant);
            }
        }

        protected override void UpdateStateSelf()
        {
            // TODO Add wolf specific states updates
            base.UpdateStateSelf();
        }

        protected override void DeathFromOldAge()
        {
            if (World.WorldConfig.DeathFromOldAge && CreatedAt.AddMilliseconds(World.WorldConfig.WolvesConfig.LifeExpectancyScaled) <= DateTime.Now)
            {
                Die();
            }
        }

        /*
        public class WolfMap : CreatureMap<Wolf>
        {
            public WolfMap()
            {
                Map(wolf => wolf.Attack).Name("attack");
            }
        }
        */
    }
}