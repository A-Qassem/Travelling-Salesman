using System;
using System.Drawing;
using System.Windows.Forms;

namespace TSP
{
    public class UIComponents
    {
        // UI components
        public Panel ControlPanel { get; private set; }
        public Panel VisualizationPanel { get; private set; }
        public Panel StatsPanel { get; private set; }
        public TrackBar SpeedTrackBar { get; private set; }
        public TrackBar MutationTrackBar { get; private set; }
        public TrackBar CrossoverTrackBar { get; private set; }
        public TrackBar PopulationTrackBar { get; private set; }
        public TrackBar CitiesTrackBar { get; private set; }
        public Label SpeedLabel { get; private set; }
        public Label MutationLabel { get; private set; }
        public Label CrossoverLabel { get; private set; }
        public Label PopulationLabel { get; private set; }
        public Label CitiesLabel { get; private set; }
        public CheckBox ShowLabelsCheckbox { get; private set; }
        public CheckBox ShowCurrentBestCheckbox { get; private set; }
        public Label ImprovementRateLabel { get; private set; }
        public Label GenTimeLabel { get; private set; }
        public Label GenerationsPerSecLabel { get; private set; }
        public Label GenerationLabel { get; private set; }
        public Label BestCostLabel { get; private set; }
        public Button StartButton { get; private set; }
        public Button StopButton { get; private set; }
        public Button ResetButton { get; private set; }

        // Create all UI components
        public void CreateUIComponents(Form parentForm)
        {
            // Main layout with two panels
            ControlPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 300,
                BackColor = Color.FromArgb(245, 245, 250),
                Padding = new Padding(10)
            };

            VisualizationPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 248, 255),
                Padding = new Padding(10)
            };

            // Controls setup
            Label titleLabel = new Label
            {
                Text = "TSP Genetic Algorithm",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 10, 0, 20)
            };

            StartButton = new Button
            {
                Text = "Start Evolution",
                BackColor = Color.FromArgb(92, 184, 92),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Top,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 0, 5)
            };
            StartButton.FlatAppearance.BorderSize = 0;

            StopButton = new Button
            {
                Text = "Stop",
                BackColor = Color.FromArgb(217, 83, 79),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Top,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 0, 5)
            };
            StopButton.FlatAppearance.BorderSize = 0;

            ResetButton = new Button
            {
                Text = "Reset",
                BackColor = Color.FromArgb(91, 192, 222),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Dock = DockStyle.Top,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 0, 20)
            };
            ResetButton.FlatAppearance.BorderSize = 0;

            // Statistics & parameters section
            StatsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(250, 250, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Create a title for statistics panel
            Label statsTitle = new Label
            {
                Text = "Statistics",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            StatsPanel.Controls.Add(statsTitle);

            GenerationLabel = new Label
            {
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(10, 30),
                Text = "Generation: 0"
            };

            BestCostLabel = new Label
            {
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(10, 50),
                Text = "Best Distance: 0 units"
            };

            ImprovementRateLabel = new Label
            {
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(10, 70),
                Text = "Improvements: 0"
            };

            GenTimeLabel = new Label
            {
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(10, 90),
                Text = "Generation time: 0 ms"
            };

            GenerationsPerSecLabel = new Label
            {
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(10, 110),
                Text = "Gens/sec: 0"
            };

            // Add labels to stats panel
            StatsPanel.Controls.Add(GenerationLabel);
            StatsPanel.Controls.Add(BestCostLabel);
            StatsPanel.Controls.Add(ImprovementRateLabel);
            StatsPanel.Controls.Add(GenTimeLabel);
            StatsPanel.Controls.Add(GenerationsPerSecLabel);

            // Parameter sliders
            Label paramLabel = new Label
            {
                Text = "Parameters",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Top,
                Padding = new Padding(0, 20, 0, 10)
            };

            // Cities slider
            CitiesLabel = new Label { Text = "Cities: 5", AutoSize = true };
            CitiesTrackBar = new TrackBar
            {
                Minimum = 5,
                Maximum = 100,
                Value = 5,
                Width = 230,
                TickFrequency = 10
            };

            // Population slider
            PopulationLabel = new Label { Text = "Population: 50", AutoSize = true };
            PopulationTrackBar = new TrackBar
            {
                Minimum = 50,
                Maximum = 500,
                Value = 50,
                Width = 230,
                TickFrequency = 50
            };

            // Mutation slider
            MutationLabel = new Label { Text = "Mutation Rate: 0.10", AutoSize = true };
            MutationTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 10, // 0.1 * 100
                Width = 230,
                TickFrequency = 10
            };

            // Crossover slider
            CrossoverLabel = new Label { Text = "Crossover Rate: 0.80", AutoSize = true };
            CrossoverTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 80, // 0.8 * 100
                Width = 230,
                TickFrequency = 10
            };

            // Speed slider
            SpeedLabel = new Label { Text = "Speed: 1 gen/sec", AutoSize = true };
            SpeedTrackBar = new TrackBar
            {
                Minimum = 1,
                Maximum = 20,
                Value = 1,
                Width = 230,
                TickFrequency = 1
            };

            // Checkboxes for visualization options
            ShowLabelsCheckbox = new CheckBox
            {
                Text = "Show City Labels",
                Checked = true,
                AutoSize = true
            };

            ShowCurrentBestCheckbox = new CheckBox
            {
                Text = "Show Current Generation Path",
                Checked = true,
                AutoSize = true
            };

            // Create parameter panel for sliders
            FlowLayoutPanel paramPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };

            // Add sliders to parameter panel
            paramPanel.Controls.Add(CitiesLabel);
            paramPanel.Controls.Add(CitiesTrackBar);
            paramPanel.Controls.Add(new Label { Height = 10 }); // Spacer
            paramPanel.Controls.Add(PopulationLabel);
            paramPanel.Controls.Add(PopulationTrackBar);
            paramPanel.Controls.Add(new Label { Height = 10 }); // Spacer
            paramPanel.Controls.Add(MutationLabel);
            paramPanel.Controls.Add(MutationTrackBar);
            paramPanel.Controls.Add(new Label { Height = 10 }); // Spacer
            paramPanel.Controls.Add(CrossoverLabel);
            paramPanel.Controls.Add(CrossoverTrackBar);
            paramPanel.Controls.Add(new Label { Height = 10 }); // Spacer
            paramPanel.Controls.Add(SpeedLabel);
            paramPanel.Controls.Add(SpeedTrackBar);
            paramPanel.Controls.Add(new Label { Height = 20 }); // Spacer
            paramPanel.Controls.Add(ShowLabelsCheckbox);
            paramPanel.Controls.Add(new Label { Height = 5 }); // Spacer
            paramPanel.Controls.Add(ShowCurrentBestCheckbox);

            // Add controls to right panel
            ControlPanel.Controls.Add(paramPanel);
            ControlPanel.Controls.Add(StatsPanel);
            ControlPanel.Controls.Add(ResetButton);
            ControlPanel.Controls.Add(StopButton);
            ControlPanel.Controls.Add(StartButton);
            ControlPanel.Controls.Add(titleLabel);

            // Add panels to form
            parentForm.Controls.Add(VisualizationPanel);
            parentForm.Controls.Add(ControlPanel);
        }
    }
}