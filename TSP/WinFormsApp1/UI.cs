using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
namespace TSP
{
    public partial class MainForm : Form
    {
        private GeneticAlgorithm _geneticAlgorithm;
        private UIComponents _uiComponents;
        private Visualization _visualization;
        private StatsTracker _statsTracker;
        private Timer _evolutionTimer;
        private Timer _uiUpdateTimer;
        private bool _isRunning = false;
        private int _lastUpdateGeneration = -1;

        // Constants for visualization area
        private const int PADDING = 50;
        private int _visualWidth;
        private int _visualHeight;

        internal MainForm()
        {
            InitializeComponent();
            Text = "TSP Genetic Algorithm Solver";
            Size = new Size(1200, 800);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Initialize components
            _geneticAlgorithm = new GeneticAlgorithm();
            _uiComponents = new UIComponents();
            _visualization = new Visualization();
            _statsTracker = new StatsTracker();

            // Create UI elements
            _uiComponents.CreateUIComponents(this);

            // Setup visualization area dimensions
            _visualWidth = _uiComponents.VisualizationPanel.Width - (2 * PADDING);
            _visualHeight = _uiComponents.VisualizationPanel.Height - (2 * PADDING);

            // Timer for evolution
            _evolutionTimer = new Timer();
            _evolutionTimer.Tick += OnEvolutionTick;

            // UI update timer (updates at 30fps max)
            _uiUpdateTimer = new Timer();
            _uiUpdateTimer.Interval = 33; // ~30fps
            _uiUpdateTimer.Tick += OnUIUpdateTick;
            _uiUpdateTimer.Start();

            // Wire up events
            _uiComponents.StartButton.Click += OnStartButtonClick;
            _uiComponents.StopButton.Click += OnStopButtonClick;
            _uiComponents.ResetButton.Click += OnResetButtonClick;
            _uiComponents.VisualizationPanel.Paint += OnVisualizationPanelPaint;
            _uiComponents.SpeedTrackBar.ValueChanged += OnSpeedValueChanged;
            _uiComponents.MutationTrackBar.ValueChanged += OnMutationValueChanged;
            _uiComponents.CrossoverTrackBar.ValueChanged += OnCrossoverValueChanged;
            _uiComponents.PopulationTrackBar.ValueChanged += OnPopulationValueChanged;
            _uiComponents.CitiesTrackBar.ValueChanged += OnCitiesValueChanged;
            _uiComponents.ShowLabelsCheckbox.CheckedChanged += OnShowLabelsChanged;
            _uiComponents.ShowCurrentBestCheckbox.CheckedChanged += OnShowCurrentBestChanged;

            // Set initial UI state
            _uiComponents.StopButton.Enabled = false;
            UpdateParameterLabels();

            // Initial setup
            ResetSimulation();
        }

        private void ResetSimulation()
        {
            _isRunning = false;
            _evolutionTimer.Stop();
            _lastUpdateGeneration = -1;

            // Get current cities count
            int cityCount = _uiComponents.CitiesTrackBar.Value;

            // Setup cities (random positions) 
            bool useCircularLayout = Control.ModifierKeys == Keys.Shift;
            _geneticAlgorithm.SetupCities(cityCount, _visualWidth, _visualHeight, PADDING, !useCircularLayout);

            // Initialize population
            _geneticAlgorithm.InitializePopulation();

            // Update UI parameters from sliders
            UpdateGeneticAlgorithmParameters();

            // Enable/disable buttons
            _uiComponents.StartButton.Enabled = true;
            _uiComponents.StopButton.Enabled = false;
            _uiComponents.CitiesTrackBar.Enabled = true;
            _uiComponents.PopulationTrackBar.Enabled = true;

            // Reset stats display
            UpdateStatsDisplay();

            // Refresh visualization
            _uiComponents.VisualizationPanel.Invalidate();
        }

        private void UpdateGeneticAlgorithmParameters()
        {
            _geneticAlgorithm.PopulationSize = _uiComponents.PopulationTrackBar.Value;
            _geneticAlgorithm.MutationRate = _uiComponents.MutationTrackBar.Value / 100.0;
            _geneticAlgorithm.CrossoverRate = _uiComponents.CrossoverTrackBar.Value / 100.0;
        }

        private void UpdateParameterLabels()
        {
            _uiComponents.CitiesLabel.Text = $"Cities: {_uiComponents.CitiesTrackBar.Value}";
            _uiComponents.PopulationLabel.Text = $"Population: {_uiComponents.PopulationTrackBar.Value}";
            _uiComponents.MutationLabel.Text = $"Mutation Rate: {_uiComponents.MutationTrackBar.Value / 100.0:F2}";
            _uiComponents.CrossoverLabel.Text = $"Crossover Rate: {_uiComponents.CrossoverTrackBar.Value / 100.0:F2}";
            _uiComponents.SpeedLabel.Text = $"Speed: {_uiComponents.SpeedTrackBar.Value} gen/sec";
        }

        private void UpdateStatsDisplay()
        {
            _uiComponents.GenerationLabel.Text = $"Generation: {_geneticAlgorithm.GenerationCount}";

            if (_geneticAlgorithm.BestCost < int.MaxValue)
            {
                _uiComponents.BestCostLabel.Text = $"Best Distance: {_geneticAlgorithm.BestCost} units";

                if (_geneticAlgorithm.InitialBestCost < int.MaxValue)
                {
                    double improvement = _statsTracker.CalculateImprovementPercentage(
                        _geneticAlgorithm.InitialBestCost,
                        _geneticAlgorithm.BestCost
                    );

                    _uiComponents.ImprovementRateLabel.Text = $"Improvements: {_geneticAlgorithm.TotalImprovements} ({improvement:F1}%)";
                }
            }

            _uiComponents.GenTimeLabel.Text = $"Generation time: {_statsTracker.LastGenerationTime} ms";
            _uiComponents.GenerationsPerSecLabel.Text = $"Gens/sec: {_statsTracker.GenerationsPerSecond:F1}";
        }

        #region Event Handlers

        private void OnVisualizationPanelPaint(object sender, PaintEventArgs e)
        {
            if (_geneticAlgorithm.Cities.Count > 0)
            {
                _visualization.ShowCityLabels = _uiComponents.ShowLabelsCheckbox.Checked;
                _visualization.ShowCurrentPath = _uiComponents.ShowCurrentBestCheckbox.Checked;
                _visualization.Improved = _geneticAlgorithm.Improved;

                // Set background color
                _uiComponents.VisualizationPanel.BackColor = _visualization.GetBackgroundColor(_geneticAlgorithm.Improved);

                // Draw TSP visualization
                _visualization.DrawTSP(
                    e.Graphics,
                    _geneticAlgorithm.Cities,
                    _geneticAlgorithm.BestPathIndices,
                    _geneticAlgorithm.CurrentBestPathIndices
                );
            }
        }

        private void OnEvolutionTick(object sender, EventArgs e)
        {
            if (!_isRunning) return;

            _statsTracker.StartGenerationTimer();

            bool improved = _geneticAlgorithm.NextGeneration();

            _statsTracker.StopGenerationTimer();
            _statsTracker.TrackCost(_geneticAlgorithm.BestCost);

            // Update UI every 5 generations if running fast
            int speedValue = _uiComponents.SpeedTrackBar.Value;
            bool shouldUpdate =
                improved ||
                _geneticAlgorithm.GenerationCount - _lastUpdateGeneration >= Math.Max(1, speedValue / 5);

            if (shouldUpdate)
            {
                _lastUpdateGeneration = _geneticAlgorithm.GenerationCount;
                UpdateStatsDisplay();
                _uiComponents.VisualizationPanel.Invalidate();
            }
        }

        private void OnUIUpdateTick(object sender, EventArgs e)
        {
            // This timer ensures UI is responsive even when evolution is running at high speed
            if (_isRunning && _lastUpdateGeneration != _geneticAlgorithm.GenerationCount)
            {
                UpdateStatsDisplay();
                _uiComponents.VisualizationPanel.Invalidate();
                _lastUpdateGeneration = _geneticAlgorithm.GenerationCount;
            }
        }

        private void OnStartButtonClick(object sender, EventArgs e)
        {
            _isRunning = true;
            _uiComponents.StartButton.Enabled = false;
            _uiComponents.StopButton.Enabled = true;
            _uiComponents.CitiesTrackBar.Enabled = false;
            _uiComponents.PopulationTrackBar.Enabled = false;

            // Set timer based on speed
            int speed = _uiComponents.SpeedTrackBar.Value;
            _evolutionTimer.Interval = 1000 / speed; // Convert to milliseconds
            _evolutionTimer.Start();
        }

        private void OnStopButtonClick(object sender, EventArgs e)
        {
            _isRunning = false;
            _evolutionTimer.Stop();
            _uiComponents.StartButton.Enabled = true;
            _uiComponents.StopButton.Enabled = false;
        }

        private void OnResetButtonClick(object sender, EventArgs e)
        {
            ResetSimulation();
        }

        private void OnSpeedValueChanged(object sender, EventArgs e)
        {
            int speed = _uiComponents.SpeedTrackBar.Value;
            _uiComponents.SpeedLabel.Text = $"Speed: {speed} gen/sec";

            if (_isRunning)
            {
                _evolutionTimer.Interval = 1000 / speed; // Convert to milliseconds
            }
        }

        private void OnMutationValueChanged(object sender, EventArgs e)
        {
            double rate = _uiComponents.MutationTrackBar.Value / 100.0;
            _uiComponents.MutationLabel.Text = $"Mutation Rate: {rate:F2}";
            _geneticAlgorithm.MutationRate = rate;
        }

        private void OnCrossoverValueChanged(object sender, EventArgs e)
        {
            double rate = _uiComponents.CrossoverTrackBar.Value / 100.0;
            _uiComponents.CrossoverLabel.Text = $"Crossover Rate: {rate:F2}";
            _geneticAlgorithm.CrossoverRate = rate;
        }

        private void OnPopulationValueChanged(object sender, EventArgs e)
        {
            int size = _uiComponents.PopulationTrackBar.Value;
            _uiComponents.PopulationLabel.Text = $"Population: {size}";
            _geneticAlgorithm.PopulationSize = size;
        }

        private void OnCitiesValueChanged(object sender, EventArgs e)
        {
            int count = _uiComponents.CitiesTrackBar.Value;
            _uiComponents.CitiesLabel.Text = $"Cities: {count}";

            // Only reset cities if not currently running
            if (!_isRunning)
            {
                ResetSimulation();
            }
        }

        private void OnShowLabelsChanged(object sender, EventArgs e)
        {
            _visualization.ShowCityLabels = _uiComponents.ShowLabelsCheckbox.Checked;
            _uiComponents.VisualizationPanel.Invalidate();
        }

        private void OnShowCurrentBestChanged(object sender, EventArgs e)
        {
            _visualization.ShowCurrentPath = _uiComponents.ShowCurrentBestCheckbox.Checked;
            _uiComponents.VisualizationPanel.Invalidate();
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1180, 760);
            this.Name = "MainForm";
            this.ResumeLayout(false);
        }
    }
}