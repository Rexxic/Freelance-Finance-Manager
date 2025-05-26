using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Freelance_Finance_Manager
{
    public partial class Form1: Form
    {
        private TableLayoutPanel mainLayout;
        private Panel navPanel, contentPanel;
        private TableLayoutPanel contentGrid;
        private readonly Dictionary<string, Button> navButtons = new Dictionary<string, Button>();

        public Form1()
        {
            InitializeComponent();
            Text = "Freelance Finance Manager";
            ClientSize = new Size(1000, 600);
            BackColor = Color.FromArgb(240, 240, 245);
            Font = new Font("Segoe UI", 10);

            InitializeLayout();
            BuildNavigation();
            BuildContentGrid();
            HighlightButton("Dashboard");
        }

        private void InitializeLayout()
        {
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Controls.Add(mainLayout);
        }

        private void BuildNavigation()
        {
            navPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(28, 45, 85),
                Padding = new Padding(0, 20, 0, 20)
            };
            mainLayout.Controls.Add(navPanel, 0, 0);

            string[] pages = { "Dashboard", "Income", "Expenses", "Budget", "Taxes", "Settings" };
            foreach (string page in pages)
            {
                Button btn = CreateNavButton(page);
                btn.Click += (s, e) => HighlightButton(page);
                navPanel.Controls.Add(btn);
                navButtons.Add(page, btn);
            }
        }

        private Button CreateNavButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 65, 115);
            return btn;
        }

        private void HighlightButton(string page)
        {
            foreach (var kvp in navButtons)
            {
                kvp.Value.BackColor = kvp.Key == page
                    ? Color.FromArgb(40, 65, 115)
                    : Color.Transparent;
            }
            // TODO: Load content for the selected page
        }

        private void BuildContentGrid()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            mainLayout.Controls.Add(contentPanel, 1, 0);

            contentGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3
            };
            contentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            contentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            contentGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));
            contentGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));
            contentGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            contentPanel.Controls.Add(contentGrid);

            AddCard(contentGrid, 0, 0, Color.FromArgb(0, 150, 136), BuildBalanceCard);
            AddCard(contentGrid, 1, 0, Color.FromArgb(244, 67, 54), BuildIncomeCard);
            AddCard(contentGrid, 0, 1, Color.FromArgb(63, 81, 181), panel => BuildSimpleStat(panel, "0", "TOTAL INCOME", 28F));
            AddCard(contentGrid, 1, 1, Color.FromArgb(156, 39, 176), panel => BuildSimpleStat(panel, "0", "TOTAL EXPENSES", 28F));
            AddTableCard(contentGrid, 0, 2, "Recent Income");
            AddTableCard(contentGrid, 1, 2, "Recent Expenses");
        }

        private void AddCard(TableLayoutPanel grid, int col, int row, Color backColor, Action<Panel> buildAction)
        {
            Panel panel = CreateRoundedPanel(backColor);
            grid.Controls.Add(panel, col, row);
            buildAction(panel);
        }

        private void AddTableCard(TableLayoutPanel grid, int col, int row, string heading)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15),
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };
            grid.Controls.Add(panel, col, row);

            var tlp = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panel.Controls.Add(tlp);

            tlp.Controls.Add(new Label
            {
                Text = heading,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 45, 85),
                Padding = new Padding(5, 0, 0, 0)
            }, 0, 0);

            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(232, 234, 246);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(28, 45, 85);
            tlp.Controls.Add(dgv, 0, 1);
        }

        private void BuildBalanceCard(Panel parent)
        {
            var tlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            parent.Controls.Add(tlp);

            tlp.Controls.Add(new Label
            {
                Text = DateTime.Now.ToString("MMMM"),
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.BottomCenter
            }, 0, 0);

            tlp.Controls.Add(new Label
            {
                Text = "0",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 36F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            }, 0, 1);

            tlp.Controls.Add(new Label
            {
                Text = "BALANCE",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter
            }, 0, 2);
        }

        private void BuildIncomeCard(Panel parent)
        {
            var tlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            parent.Controls.Add(tlp);

            var info = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            info.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            info.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tlp.Controls.Add(info, 0, 0);

            info.Controls.Add(new Label
            {
                Text = "0",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 36F, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            }, 0, 0);

            info.Controls.Add(new Label
            {
                Text = "TOTAL INCOME",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter
            }, 0, 1);

            tlp.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(5) }, 1, 0);
        }

        private void BuildSimpleStat(Panel parent, string value, string label, float fontSize)
        {
            var tlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
            parent.Controls.Add(tlp);

            tlp.Controls.Add(new Label
            {
                Text = value,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", fontSize, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            }, 0, 0);

            tlp.Controls.Add(new Label
            {
                Text = label,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter
            }, 0, 1);
        }

        private Panel CreateRoundedPanel(Color backColor, int radius = 15)
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = backColor, Padding = new Padding(15), Margin = new Padding(10) };
            var path = new GraphicsPath();
            path.AddRoundedRectangle(panel.ClientRectangle, radius);
            panel.Region = new Region(path);
            panel.Resize += (s, e) =>
            {
                var newPath = new GraphicsPath();
                newPath.AddRoundedRectangle(panel.ClientRectangle, radius);
                panel.Region = new Region(newPath);
            };
            return panel;
        }
    }

    public static class GraphicsPathExtensions
    {
        public static void AddRoundedRectangle(this GraphicsPath path, Rectangle bounds, int diameter)
        {
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
        }
    }
}
