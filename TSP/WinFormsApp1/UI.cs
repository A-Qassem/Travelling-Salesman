using Timer = System.Threading.Timer;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class UI : Form
    {
        Random rand = new Random();
        int n = 5;
        Point[] cities;
        List<List<int>> population = new List<List<int>>();
        List<int> bestPath = new List<int>();
        int bestCost = int.MaxValue;
        int populationSize = 50;
        double mutationRate = 0.1;
        double crossoverRate = 0.8;
        Timer timer;
        int generationCount = 0;
        bool improved = false;
        int animationSpeed = 1000; // ms between generations
        Color backgroundColor = Color.FromArgb(240, 248, 255);
        Color improvedColor = Color.FromArgb(230, 255, 230);
        Color cityColor = Color.OrangeRed;
        Color pathColor = Color.MediumSeaGreen;
        Color currentPathColor = Color.FromArgb(150, 70, 130, 180);
        bool showCurrentPath = true;
        List<int> currentBestPath = new List<int>();
        bool showCityLabels = true;

        // Performance tracking
        Stopwatch generationTimer = new Stopwatch();
        Queue<int> recentCosts = new Queue<int>();
        const int maxCostsToTrack = 20;

        // Statistics
        int initialBestCost = int.MaxValue;
        int gensSinceImprovement = 0;
        int totalImprovements = 0;

        // UI components
        Panel controlPanel;
        Panel visualizationPanel;
        Panel statsPanel;
        TrackBar speedTrackBar;
        TrackBar mutationTrackBar;
        TrackBar crossoverTrackBar;
        TrackBar populationTrackBar;
        TrackBar citiesTrackBar;
        Label speedLabel;
        Label mutationLabel;
        Label crossoverLabel;
        Label populationLabel;
        Label citiesLabel;
        CheckBox showLabelsCheckbox;
        CheckBox showCurrentBestCheckbox;
        Label improvementRateLabel;
        Label genTimeLabel;
        Label generationsPerSecLabel;

        public UI()
        {
            InitializeComponent();
            this.Text = "Traveling Salesman - Genetic Algorithm";
            this.Width = 1800;
            this.Height = 1080;
            this.BackColor = Color.White;
            this.Icon = SystemIcons.Application;

            SetupUI();
            GenerateCities();
            InitializePopulation();
            UpdateLabels();
        }

        void SetupUI()
        {
            // Main layout with two panels
            controlPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 300,
                BackColor = Color.FromArgb(245, 245, 250),
                Padding = new Padding(10)
            };

            visualizationPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = backgroundColor,
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

            Button buttonStart = new Button
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
            buttonStart.FlatAppearance.BorderSize = 0;
            buttonStart.Click += buttonStart_Click;

            Button buttonStop = new Button
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
            buttonStop.FlatAppearance.BorderSize = 0;
            buttonStop.Click += buttonStop_Click;

            Button buttonReset = new Button
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
            buttonReset.FlatAppearance.BorderSize = 0;
            buttonReset.Click += buttonReset_Click;

            // Statistics & parameters section
            statsPanel = new Panel
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
            statsPanel.Controls.Add(statsTitle);

            labelGeneration = new Label
            {
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(10, 30)
            };

            labelBestCost = new Label
            {
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(10, 50)
            };

            improvementRateLabel = new Label
            {
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(10, 70),
                Text = "Improvements: 0"
            };

            genTimeLabel = new Label
            {
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(10, 90),
                Text = "Generation time: 0 ms"
            };

            generationsPerSecLabel = new Label
            {
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(10, 110),
                Text = "Gens/sec: 0"
            };

            // Add labels to stats panel
            statsPanel.Controls.Add(labelGeneration);
            statsPanel.Controls.Add(labelBestCost);
            statsPanel.Controls.Add(improvementRateLabel);
            statsPanel.Controls.Add(genTimeLabel);
            statsPanel.Controls.Add(generationsPerSecLabel);

            // Parameter sliders
            Label paramLabel = new Label
            {
                Text = "Parameters",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Top,
                Padding = new Padding(0, 20, 0, 10)
            };

            // Cities slider
            citiesLabel = new Label { Text = "Cities: " + n, AutoSize = true };
            citiesTrackBar = new TrackBar
            {
                Minimum = 5,
                Maximum = 100,
                Value = n,
                Width = 230,
                TickFrequency = 10
            };
            citiesTrackBar.ValueChanged += (s, e) =>
            {
                n = citiesTrackBar.Value;
                citiesLabel.Text = "Cities: " + n;
            };

            // Population slider
            populationLabel = new Label { Text = "Population: " + populationSize, AutoSize = true };
            populationTrackBar = new TrackBar
            {
                Minimum = 50,
                Maximum = 500,
                Value = populationSize,
                Width = 230,
                TickFrequency = 50
            };
            populationTrackBar.ValueChanged += (s, e) =>
            {
                populationSize = populationTrackBar.Value;
                populationLabel.Text = "Population: " + populationSize;
            };

            // Mutation slider
            mutationLabel = new Label { Text = "Mutation Rate: " + mutationRate, AutoSize = true };
            mutationTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)(mutationRate * 100),
                Width = 230,
                TickFrequency = 10
            };
            mutationTrackBar.ValueChanged += (s, e) =>
            {
                mutationRate = mutationTrackBar.Value / 100.0;
                mutationLabel.Text = "Mutation Rate: " + mutationRate.ToString("0.00");
            };

            // Crossover slider
            crossoverLabel = new Label { Text = "Crossover Rate: " + crossoverRate, AutoSize = true };
            crossoverTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)(crossoverRate * 100),
                Width = 230,
                TickFrequency = 10
            };
            crossoverTrackBar.ValueChanged += (s, e) =>
            {
                crossoverRate = crossoverTrackBar.Value / 100.0;
                crossoverLabel.Text = "Crossover Rate: " + crossoverRate.ToString("0.00");
            };

            // Speed slider
            speedLabel = new Label { Text = "Speed: " + (1000 / animationSpeed) + " gen/sec", AutoSize = true };
            speedTrackBar = new TrackBar
            {
                Minimum = 1,
                Maximum = 20,
                Value = 1000 / animationSpeed,
                Width = 230,
                TickFrequency = 1
            };
            speedTrackBar.ValueChanged += (s, e) =>
            {
                animationSpeed = 1000 / Math.Max(1, speedTrackBar.Value);
                speedLabel.Text = "Speed: " + speedTrackBar.Value + " gen/sec";
                // Update timer if running
                if (timer != null)
                {
                    StopGA();
                    StartGA();
                }
            };

            // Checkboxes for visualization options
            showLabelsCheckbox = new CheckBox
            {
                Text = "Show City Labels",
                Checked = showCityLabels,
                AutoSize = true
            };
            showLabelsCheckbox.CheckedChanged += (s, e) =>
            {
                showCityLabels = showLabelsCheckbox.Checked;
                visualizationPanel.Invalidate();
            };

            showCurrentBestCheckbox = new CheckBox
            {
                Text = "Show Current Generation Path",
                Checked = showCurrentPath,
                AutoSize = true
            };
            showCurrentBestCheckbox.CheckedChanged += (s, e) =>
            {
                showCurrentPath = showCurrentBestCheckbox.Checked;
                visualizationPanel.Invalidate();
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
            paramPanel.Controls.Add(citiesLabel);
            paramPanel.Controls.Add(citiesTrackBar);
            paramPanel.Controls.Add(new Label { Height = 10 }); // Spacer
            paramPanel.Controls.Add(populationLabel);
            paramPanel.Controls.Add(populationTrackBar);
            paramPanel.Controls.Add(new Label { Height = 10 }); // Spacer
            paramPanel.Controls.Add(mutationLabel);
            paramPanel.Controls.Add(mutationTrackBar);
            paramPanel.Controls.Add(new Label { Height = 10 }); // Spacer
            paramPanel.Controls.Add(crossoverLabel);
            paramPanel.Controls.Add(crossoverTrackBar);
            paramPanel.Controls.Add(new Label { Height = 10 }); // Spacer
            paramPanel.Controls.Add(speedLabel);
            paramPanel.Controls.Add(speedTrackBar);
            paramPanel.Controls.Add(new Label { Height = 20 }); // Spacer
            paramPanel.Controls.Add(showLabelsCheckbox);
            paramPanel.Controls.Add(new Label { Height = 5 }); // Spacer
            paramPanel.Controls.Add(showCurrentBestCheckbox);

            // Add controls to right panel
            controlPanel.Controls.Add(paramPanel);
            controlPanel.Controls.Add(statsPanel);
            controlPanel.Controls.Add(buttonReset);
            controlPanel.Controls.Add(buttonStop);
            controlPanel.Controls.Add(buttonStart);
            controlPanel.Controls.Add(titleLabel);

            // Visualization panel setup
            visualizationPanel.Paint += (s, e) => OnPaint(e);

            // Add panels to form
            this.Controls.Add(visualizationPanel);
            this.Controls.Add(controlPanel);
        }


        void GenerateCities()
        {


            // Option 1: Random cities

            // Option 2: Circle arrangement (uncomment to use)

            CitiesPositions(1);
        }
        void CitiesPositions(int type)
        {
            cities = new Point[n];
            int padding = 80;
            int width = visualizationPanel.Width - (padding * 2);
            int height = visualizationPanel.Height - (padding * 2);
            if (type == 1)
            {
                for (int i = 0; i < n; i++)
                    cities[i] = new Point(
                    rand.Next(padding, padding + width),
                        rand.Next(padding, padding + height)
                    );
            }
            else
            {
                double radius = Math.Min(width, height) / 2.5;
                double centerX = padding + width / 2;
                double centerY = padding + height / 2;

                for (int i = 0; i < n; i++)
                {
                    double angle = 2 * Math.PI * i / n;
                    int x = (int)(centerX + radius * Math.Cos(angle));
                    int y = (int)(centerY + radius * Math.Sin(angle));
                    cities[i] = new Point(x, y);
                }
            }
        }
        void InitializePopulation()
        {
            population.Clear();
            List<int> indices = new List<int>();
            for (int i = 1; i < n; i++)
                indices.Add(i);

            for (int i = 0; i < populationSize; i++)
                population.Add(indices.OrderBy(x => rand.Next()).ToList());

            bestCost = int.MaxValue;
            initialBestCost = int.MaxValue;
            bestPath.Clear();
            currentBestPath.Clear();
            generationCount = 0;
            gensSinceImprovement = 0;
            totalImprovements = 0;
            recentCosts.Clear();
        }

        void StartGA()
        {
            if (timer == null)
                timer = new Timer(NextGeneration, null, 0, animationSpeed);
        }

        void StopGA()
        {
            timer?.Dispose();
            timer = null;
        }

        void NextGeneration(object state)
        {
            generationTimer.Restart();

            population = population.OrderBy(p => Fitness(p)).ToList();
            generationCount++;
            gensSinceImprovement++;

            int currentBestCost = Fitness(population[0]);
            currentBestPath = new List<int>(population[0]);

            // Track for statistics
            if (recentCosts.Count >= maxCostsToTrack)
                recentCosts.Dequeue();
            recentCosts.Enqueue(currentBestCost);

            if (currentBestCost < bestCost)
            {
                if (initialBestCost == int.MaxValue)
                    initialBestCost = currentBestCost;

                bestCost = currentBestCost;
                bestPath = new List<int>(population[0]);
                improved = true;
                gensSinceImprovement = 0;
                totalImprovements++;
            }

            List<List<int>> newPopulation = new List<List<int>>();

            // Keep top performers (elitism)
            int eliteCount = Math.Max(1, populationSize / 20); // 5% elitism
            for (int i = 0; i < eliteCount; i++)
                newPopulation.Add(new List<int>(population[i]));

            // Fill the rest with crossover + mutation
            while (newPopulation.Count < populationSize)
            {
                var parent1 = TournamentSelection();
                var parent2 = TournamentSelection();

                List<int> child;

                // Apply crossover based on crossover rate
                if (rand.NextDouble() < crossoverRate)
                    child = Crossover(parent1, parent2);
                else
                    child = new List<int>(parent1); // Just copy parent1

                Mutate(child);
                newPopulation.Add(child);
            }

            population = newPopulation;

            generationTimer.Stop();
            long generationTime = generationTimer.ElapsedMilliseconds;

            this.Invoke((MethodInvoker)delegate
            {
                UpdateLabels(generationTime);
                visualizationPanel.Invalidate();
            });
        }

        int Fitness(List<int> path)
        {
            int cost = 0, k = 0;
            foreach (var city in path)
            {
                if (k < cities.Length && city < cities.Length) // Add safety check
                    cost += Distance(cities[k], cities[city]);
                k = city;
            }
            if (k < cities.Length) // Add safety check
                cost += Distance(cities[k], cities[0]);
            return cost;
        }

        int Distance(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return (int)Math.Sqrt(dx * dx + dy * dy);
        }

        List<int> TournamentSelection()
        {
            int k = Math.Max(3, population.Count / 50); // Dynamic tournament size
            var selected = new List<List<int>>();
            for (int i = 0; i < k; i++)
                selected.Add(population[rand.Next(population.Count)]);
            return selected.OrderBy(p => Fitness(p)).First();
        }

        List<int> Crossover(List<int> parent1, List<int> parent2)
        {
            // Ordered Crossover (OX)
            int start = rand.Next(parent1.Count);
            int end = rand.Next(start, parent1.Count);

            var child = Enumerable.Repeat(0, parent1.Count).ToList();
            for (int i = start; i <= end; i++)
                child[i] = parent1[i];

            int idx = 0;
            for (int i = 0; i < parent2.Count; i++)
            {
                if (!child.Contains(parent2[i]))
                {
                    while (idx < child.Count && child[idx] != 0) idx++;
                    if (idx < child.Count)
                        child[idx] = parent2[i];
                }
            }

            return child;
        }

        void Mutate(List<int> path)
        {
            // Swap mutation
            if (rand.NextDouble() < mutationRate)
            {
                int i = rand.Next(path.Count);
                int j = rand.Next(path.Count);
                int temp = path[i];
                path[i] = path[j];
                path[j] = temp;
            }

            // Additional mutation type: 2-opt local improvement
            if (rand.NextDouble() < mutationRate * 0.5)
            {
                int i = rand.Next(path.Count);
                int j = rand.Next(path.Count);
                if (i > j) { int temp = i; i = j; j = temp; }
                if (j - i >= 2) // Only reverse if segment is at least 2 elements
                {
                    // Reverse the segment between i and j
                    while (i < j)
                    {
                        int temp = path[i];
                        path[i] = path[j];
                        path[j] = temp;
                        i++; j--;
                    }
                }
            }
        }

        private void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Flash background on improvement
            if (improved)
            {
                visualizationPanel.BackColor = improvedColor;
                improved = false;
            }
            else
            {
                visualizationPanel.BackColor = backgroundColor;
            }

            // Draw grid background
            using (Pen gridPen = new Pen(Color.FromArgb(20, 0, 0, 0)))
            {
                int gridSize = 25;
                for (int x = 0; x < visualizationPanel.Width; x += gridSize)
                    g.DrawLine(gridPen, x, 0, x, visualizationPanel.Height);

                for (int y = 0; y < visualizationPanel.Height; y += gridSize)
                    g.DrawLine(gridPen, 0, y, visualizationPanel.Width, y);
            }

            // Draw current generation best path if enabled and available
            if (showCurrentPath && currentBestPath.Count > 0 && cities.Length > 0)
            {
                using (Pen pathPen = new Pen(currentPathColor, 1.5f))
                {
                    Point prev = cities[0];
                    foreach (var idx in currentBestPath)
                    {
                        if (idx < cities.Length) // Safety check
                        {
                            Point next = cities[idx];
                            g.DrawLine(pathPen, prev, next);
                            prev = next;
                        }
                    }
                    g.DrawLine(pathPen, prev, cities[0]);
                }
            }

            // Draw best overall path if available
            if (bestPath.Count > 0 && cities.Length > 0)
            {
                using (Pen pathPen = new Pen(pathColor, 2.5f))
                {
                    Point prev = cities[0];
                    foreach (var idx in bestPath)
                    {
                        if (idx < cities.Length) // Safety check
                        {
                            Point next = cities[idx];
                            g.DrawLine(pathPen, prev, next);
                            // Draw an arrow to show direction
                            DrawArrow(g, prev, next, pathPen);
                            prev = next;
                        }
                    }
                    g.DrawLine(pathPen, prev, cities[0]);
                    DrawArrow(g, prev, cities[0], pathPen);
                }
            }

            // Draw cities
            for (int i = 0; i < cities.Length; i++)
            {
                Point city = cities[i];
                int size = i == 0 ? 14 : 12;
                Color fillColor = i == 0 ? Color.Blue : cityColor;

                g.FillEllipse(new SolidBrush(fillColor), city.X - size / 2, city.Y - size / 2, size, size);
                g.DrawEllipse(new Pen(Color.Black, 1.5f), city.X - size / 2, city.Y - size / 2, size, size);

                // Draw city labels if enabled
                if (showCityLabels)
                {
                    string label = i.ToString();
                    Font labelFont = new Font("Arial", 9, FontStyle.Bold);
                    SizeF textSize = g.MeasureString(label, labelFont);

                    // Draw semi-transparent white background for text
                    g.FillRectangle(
                        new SolidBrush(Color.FromArgb(180, 255, 255, 255)),
                        city.X - textSize.Width / 2,
                        city.Y - size / 2 - textSize.Height - 2,
                        textSize.Width,
                        textSize.Height
                    );

                    g.DrawString(
                        label,
                        labelFont,
                        Brushes.Black,
                        city.X - textSize.Width / 2,
                        city.Y - size / 2 - textSize.Height - 2
                    );
                }
            }

        }

        private void DrawArrow(Graphics g, Point start, Point end, Pen pen)
        {
            // Only draw arrows if cities are far enough apart
            double distance = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
            if (distance < 50) return;

            // Calculate arrow position (at 70% of the way from start to end)
            float ratio = 0.7f;
            Point arrowPos = new Point(
                (int)(start.X + (end.X - start.X) * ratio),
                (int)(start.Y + (end.Y - start.Y) * ratio)
            );

            // Calculate angle
            double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);

            // Create arrow head
            Point[] arrowHead = new Point[3];
            int arrowSize = 8;
            arrowHead[0] = arrowPos;
            arrowHead[1] = new Point(
                (int)(arrowPos.X - arrowSize * Math.Cos(angle - Math.PI / 6)),
                (int)(arrowPos.Y - arrowSize * Math.Sin(angle - Math.PI / 6))
            );
            arrowHead[2] = new Point(
                (int)(arrowPos.X - arrowSize * Math.Cos(angle + Math.PI / 6)),
                (int)(arrowPos.Y - arrowSize * Math.Sin(angle + Math.PI / 6))
            );

            // Draw arrow head
            g.FillPolygon(new SolidBrush(pen.Color), arrowHead);
        }

        void UpdateLabels(long generationTime = 0)
        {
            labelBestCost.Text = $"Best Distance: {bestCost} units";
            labelGeneration.Text = $"Generation: {generationCount}";

            // Calculate improvement percentage if we have an initial cost
            if (initialBestCost != int.MaxValue && initialBestCost != 0)
            {
                double improvementPct = 100.0 * (initialBestCost - bestCost) / initialBestCost;
                improvementRateLabel.Text = $"Improvement: {improvementPct:F2}% ({totalImprovements})";
            }

            genTimeLabel.Text = $"Generation time: {generationTime} ms";

            // Calculate generations per second
            if (recentCosts.Count > 0 && generationTime > 0)
            {
                double gensPerSec = 1000.0 / generationTime;
                generationsPerSecLabel.Text = $"Gens/sec: {gensPerSec:F2}";
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            citiesTrackBar.Enabled = false;
            populationTrackBar.Enabled = false;
            StartGA();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            StopGA();
            citiesTrackBar.Enabled = true;
            populationTrackBar.Enabled = true;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            StopGA();
            GenerateCities();
            InitializePopulation();
            UpdateLabels();
            visualizationPanel.Invalidate();
            citiesTrackBar.Enabled = true;
            populationTrackBar.Enabled = true;
        }
    }
}