﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rabbit_Island.Entities
{
    internal class Rabbit : Creature
    {
        public static class RabbitsValues
        {
            /// <summary>
            /// Scales values to match the Time Rate of the World.
            /// </summary>
            public static void RefreshTimeScalar()
            {
                int timeScalar = 1000 / (int)World.Instance.WorldConfig.TimeRate;
                EatingTime = 120 * timeScalar;
                MatingTime = 300 * timeScalar;
                WaitToMateTime = 50 * timeScalar;
            }

            /// <summary>
            /// Time needed to eat a fruit.
            /// </summary>
            public static int EatingTime { get; private set; }

            /// <summary>
            /// Time neede to mate.
            /// </summary>
            public static int MatingTime { get; private set; }

            /// <summary>
            /// Wait for other rabbit to mate time.
            /// </summary>
            public static int WaitToMateTime { get; private set; }
        }

        public Rabbit(float x, float y) : base(x, y)
        {
            Random random = new Random();
            MaxHealth = random.Next(90, 110);
            Health = MaxHealth;
            MaxEnergy = random.Next(90, 110);
            Energy = MaxEnergy;
            SightRange = random.Next(50);
            MovementSpeed = random.Next(5, 20);
            InteractionRange = random.Next(10);
            Fear = random.Next(10);
        }

        public int Fear { get; }

        public override void DrawSelf(Canvas canvas)
        {
            var rabbitCanvas = new Canvas();
            var color = Brushes.Blue;
            var rectangle = new Rectangle()
            {
                Width = 1,
                Height = 1,
                Fill = color
            };
            rabbitCanvas.Children.Add(rectangle);
            if (World.Instance.WorldConfig.DrawRanges)
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

        protected override void PerformAction(Action action)
        {
            switch (action.Type)
            {
                case ActionType.MoveTo:
                    Move(action.Target);
                    break;

                case ActionType.Eat:
                    Thread.Sleep(RabbitsValues.EatingTime);
                    var energy = Energy + 50;
                    if (energy > MaxEnergy)
                        energy = MaxEnergy;
                    Energy = energy;
                    World.Instance.RemoveEntity(action.Target);
                    break;

                case ActionType.Mate:
                    // TODO Improve intertherad communication
                    if (action.Target is Rabbit otherRabbit)
                    {
                        if (otherRabbit.WaitingToMate)
                        {
                            otherRabbit.CreatureThread!.Interrupt();
                            Thread.Sleep(RabbitsValues.MatingTime);
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
                            try
                            {
                                Thread.Sleep(RabbitsValues.WaitToMateTime);
                            }
                            catch (ThreadInterruptedException)
                            {
                                Thread.Sleep(RabbitsValues.MatingTime);
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

                default:
                    throw new Exception("Illegal action");
            }
        }

        protected override Action Think(List<Entity> closeByEntities)
        {
            // TODO Improve this
            if (Energy < MaxEnergy / 2)
            {
                if (closeByEntities.Find(entity => entity is Fruit) is Fruit fruit)
                {
                    if (Vector2.Distance(Position, fruit.Position) <= InteractionRange)
                    {
                        return new Action(ActionType.Eat, fruit);
                    }
                    return new Action(ActionType.MoveTo, fruit);
                }
            }
            else if (closeByEntities.Find(entity => entity is Rabbit rabbit
                && rabbit.Gender != Gender
                && !rabbit.IsPregnant) is Rabbit otherRabbit)
            {
                if (Vector2.Distance(Position, otherRabbit.Position) <= InteractionRange)
                {
                    return new Action(ActionType.Mate, otherRabbit);
                }
                return new Action(ActionType.MoveTo, otherRabbit);
            }
            var destination = new Vector2(50, 100);
            return new Action(ActionType.MoveTo, new Point(destination));
        }

        protected override void UpdateStateSelf()
        {
            // TODO Add rabbit specific states updates
            // TODO Check for rabbit pregnancy, and if finished create new rabbits
            base.UpdateStateSelf();
        }
    }
}