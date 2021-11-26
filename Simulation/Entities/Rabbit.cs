using System.Numerics;

namespace Simulation.Entities
{
    public class Rabbit : Creature
    {
        public Rabbit(Vector2 position, World world) : base(position, world)
        {
            int creditsLeft = World.WorldConfig.RabbitConfig.InitialPopulationCredits;

            var traits = new Dictionary<string, double>
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

            traits.Add("Fear", StaticRandom.Generator.Next(1, creditsLeft));

            MaxHealth = traits["MaxHealth"] * 1.1;
            Health = MaxHealth;
            MaxEnergy = traits["MaxEnergy"] * 1.1;
            Energy = MaxEnergy;
            SightRange = traits["SightRange"] * 0.5;
            MovementSpeed = traits["MovementSpeed"] * 0.25;
            InteractionRange = traits["InteractionRange"] * 0.18;
            Fear = traits["Fear"] * 8;
        }

        public Rabbit(
            Vector2 position, World world, uint generation, double maxHealth, double maxEnergy, double sightRange, double movementSpeed, double interactionRange,
            double fear) : base(position, world, generation)
        {
            MaxHealth = maxHealth;
            Health = MaxHealth;
            MaxEnergy = maxEnergy;
            Energy = MaxEnergy;
            SightRange = sightRange;
            MovementSpeed = movementSpeed;
            InteractionRange = interactionRange;
            Fear = fear;
        }

        public double Fear { get; }

        /*
        public override void DrawSelf(Canvas canvas)
        {
            var rabbitCanvas = new Canvas();
            var color = IsAlive ? Brushes.Blue : Brushes.Purple;
            var rectangle = new Rectangle()
            {
                Width = 1,
                Height = 1,
                Fill = color
            };
            rabbitCanvas.Children.Add(rectangle);
            if (World.Instance.WorldConfig.DrawRanges && IsAlive)
            {
                var sightRange = new Ellipse()
                {
                    Width = SightRange,
                    Height = SightRange,
                    Stroke = color
                };
                rabbitCanvas.Children.Add(sightRange);
                Canvas.SetLeft(sightRange, -SightRange / 2);
                Canvas.SetTop(sightRange, -SightRange / 2);
            }
            canvas.Children.Add(rabbitCanvas);
            Canvas.SetLeft(rabbitCanvas, Position.X);
            Canvas.SetTop(rabbitCanvas, Position.Y);
        }
        */

        protected override void PerformAction(Action action)
        {
            switch (action.Type)
            {
                case ActionType.MoveTo:
                    Move(action.Target);
                    break;

                case ActionType.MoveAway:
                    if (action.Target is EntitiesGroup entitiesGroup)
                    {
                        MoveAway(entitiesGroup);
                    }
                    else
                    {
                        MoveAway(action.Target);
                    }
                    break;

                case ActionType.Eat:
                    Thread.Sleep(World.WorldConfig.RabbitConfig.EatingTimeScaled);
                    if (World.RemoveEntity(action.Target))
                    {
                        var energy = Energy + 50;
                        if (energy > MaxEnergy)
                            energy = MaxEnergy;
                        Energy = energy;
                    }
                    break;

                case ActionType.Mate:
                    if (action.Target is Rabbit otherRabbit)
                    {
                        if (otherRabbit.WaitingToMate)
                        {
                            otherRabbit.InteractionEvent.Set();
                            Thread.Sleep(World.WorldConfig.RabbitConfig.MatingTimeScaled);
                            if (Gender == GenderType.Female)
                            {
                                PregnantAt = DateTime.Now;
                                PregnantWith = otherRabbit;
                                States.Add(State.Pregnant);
                            }
                        }
                        else
                        {
                            States.Add(State.WaitingToMate);
                            if (InteractionEvent.WaitOne(World.WorldConfig.RabbitConfig.WaitToMateTimeScaled))
                            {
                                Thread.Sleep(World.WorldConfig.RabbitConfig.MatingTimeScaled);

                                if (Gender == GenderType.Female)
                                {
                                    PregnantAt = DateTime.Now;
                                    PregnantWith = otherRabbit;
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

        private float CalculateDanger(List<Wolf> wolves)
        {
            float accumulatedDanger = 0;
            foreach (var wolf in wolves)
            {
                var distance = Vector2.Distance(Position, wolf.Position);
                accumulatedDanger += 1 / distance;
            }
            return accumulatedDanger;
        }

        protected override Action Think(List<Entity> closeByEntities)
        {
            var nearbyWolves = closeByEntities.OfType<Wolf>().Where(wolf => wolf.IsAlive).ToList();
            var danger = CalculateDanger(nearbyWolves);
            if (danger >= 1f / Fear)
            {
                return new Action(ActionType.MoveAway, new EntitiesGroup(new List<Entity>(nearbyWolves), World));
            }
            if (Energy < MaxEnergy / 2)
            {
                if (States.Add(State.SearchingForFood))
                {
                    _movingSince = DateTime.Now;
                }
                if (closeByEntities.Find(entity => entity is Fruit) is Fruit fruit)
                {
                    if (Vector2.Distance(Position, fruit.Position) <= InteractionRange)
                    {
                        States.Remove(State.SearchingForFood);
                        return new Action(ActionType.Eat, fruit);
                    }
                    return new Action(ActionType.MoveTo, fruit);
                }
                else if (_movingSince.AddMilliseconds(World.WorldConfig.RabbitConfig.MoveInOneDirectionTimeScaled) <= DateTime.Now)
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
            else if (closeByEntities.Find(entity => entity is Rabbit rabbit
                && rabbit.IsAlive
                && rabbit.Gender != Gender
                && rabbit.CanMate
                && !rabbit.IsPregnant
                && !IsPregnant) is Rabbit otherRabbit)
            {
                States.Remove(State.SearchingForMatingPartner);
                if (Vector2.Distance(Position, otherRabbit.Position) <= InteractionRange)
                {
                    return new Action(ActionType.Mate, otherRabbit);
                }
                return new Action(ActionType.MoveTo, otherRabbit);
            }
            else if (!States.Contains(State.Pregnant))
            {
                if (States.Add(State.SearchingForMatingPartner))
                {
                    _movingSince = DateTime.Now;
                }
                if (_movingSince.AddMilliseconds(World.WorldConfig.RabbitConfig.MoveInOneDirectionTimeScaled) <= DateTime.Now)
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
                && PregnantAt.AddMilliseconds(World.WorldConfig.RabbitConfig.PregnancyTimeScaled) <= DateTime.Now
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
            // TODO Add rabbit specific states updates
            base.UpdateStateSelf();
            // Rabbit specific status updates
        }

        protected override void DeathFromOldAge()
        {
            if (World.WorldConfig.DeathFromOldAge && CreatedAt.AddMilliseconds(World.WorldConfig.RabbitConfig.LifeExpectancyScaled) <= DateTime.Now)
            {
                Die();
            }
        }
    }
}