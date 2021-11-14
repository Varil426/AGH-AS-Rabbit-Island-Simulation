using Simulation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Visualization.EntitiesVisualization;

namespace Visualization
{
    /// <summary>
    /// Interaction logic for SimulationWindow.xaml
    /// </summary>
    public partial class SimulationWindow : Window
    {
        private readonly Thread _thread;

        private bool _threadRun;

        private readonly Simulation.Simulation _simulation;

        private readonly List<ISimulationEntityVisualization> entitiesRepresentation = new();

        //private readonly World world = World.Instance;

        private readonly Canvas canvas;

        public SimulationWindow(Simulation.Simulation simulation)
        {
            InitializeComponent();

            _simulation = simulation;
            SubscribeToSimulationEvents();

            canvas = new Canvas
            {
                Width = _simulation.World.WorldMap.Size.Item1,
                Height = _simulation.World.WorldMap.Size.Item2,
                Background = Brushes.Green
            };
            Plane.Children.Add(canvas);

            _thread = new Thread(DrawSimulation)
            {
                IsBackground = true
            };
            _thread.Start();
        }

        private void SubscribeToSimulationEvents()
        {
            _simulation.EntityAdded += SimulationEntityAdded;
            _simulation.EntityRemoved += SimulationEntityRemoved;
        }

        private void SimulationEntityAdded(Entity entity)
        {
            lock (entitiesRepresentation)
            {
                switch (entity)
                {
                    case Rabbit rabbit:
                        entitiesRepresentation.Add(new RabbitVisualization(rabbit));
                        break;
                    case Fruit fruit:
                        entitiesRepresentation.Add(new FruitVisualization(fruit));
                        break;

                    case Wolf wolf:
                        entitiesRepresentation.Add(new WolfVisualization(wolf));
                        break;

                    default:
                        throw new ArgumentException("Unrecognized type.");
                }
            }
        }

        private void SimulationEntityRemoved(Entity entity)
        {
            lock (entitiesRepresentation)
            {
                var visualization = entitiesRepresentation.FirstOrDefault(visualization => visualization.Entity == entity);
                if (visualization != null)
                {
                    entitiesRepresentation.Remove(visualization);
                }
            }
        }

        private void DrawSimulation()
        {
            var timeout = 1000 / 30;
            _threadRun = true;
            while (_threadRun)
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        canvas.Children.Clear();

                        lock (entitiesRepresentation)
                        {
                            foreach (var entity in entitiesRepresentation)
                            {
                                entity.DrawSelf(canvas);
                            }
                        }
                    });
                    Thread.Sleep(timeout);
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                }
            }
        }

        public void StopAndClose()
        {
            Close();
            _threadRun = false;
        }
    }
}