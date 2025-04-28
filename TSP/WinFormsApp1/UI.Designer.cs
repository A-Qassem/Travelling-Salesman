namespace WinFormsApp1
{
    partial class UI
    {
        private System.ComponentModel.IContainer components = null;
        private Label labelBestCost;
        private Label labelGeneration;
        private Button buttonStart;
        private Button buttonStop;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // UI
            // 
            ClientSize = new Size(1405, 763);
            Name = "UI";
            Text = "Genetic Algorithm - TSP";
            WindowState = FormWindowState.Maximized;
            ResumeLayout(false);
        }
    }
}
