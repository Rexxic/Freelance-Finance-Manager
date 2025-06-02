using Freelance_Finance_Manager.Repositories;
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
            Array.Reverse(pages);

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

            // Entferne alten ContentPanel (falls vorhanden)
            if (contentPanel != null)
            {
                mainLayout.Controls.Remove(contentPanel);
                contentPanel.Dispose();
            }

            // Neues contentPanel erzeugen
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            mainLayout.Controls.Add(contentPanel, 1, 0);

            // Seite aufbauen
            switch (page)
            {
                case "Dashboard":
                    BuildContentGrid();
                    break;
                case "Income":
                    LoadIncomeView();
                    break;
                case "Expenses":
                    LoadExpensesView();
                    break;
                case "Budget":
                    LoadBudgetView();
                    break;

            }

        }

        private void LoadIncomeView()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            contentPanel.Controls.Add(layout);

            // Eingabeformular
            var formPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(245, 245, 250)
            };
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.Controls.Add(formPanel, 0, 0);

            var lblAmount = new Label { Text = "Betrag:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var txtAmount = new TextBox { Dock = DockStyle.Fill };

            var lblDate = new Label { Text = "Datum:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var dtpDate = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };

            var lblCategory = new Label { Text = "Kategorie:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var cmbCategory = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };
            // Kategorien vom Typ Income laden
            var catRepo = new CategoryRepository();
            var incomeCategories = catRepo.GetAll().Where(c => c.Type == CategoryType.Income).ToList();
            cmbCategory.Items.AddRange(incomeCategories.Select(c => c.Name).ToArray());


            var lblDesc = new Label { Text = "Beschreibung:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var txtDesc = new TextBox { Dock = DockStyle.Fill };

            var btnSave = new Button
            {
                Text = "Speichern",
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                Dock = DockStyle.Right,
                Height = 30
            };

            formPanel.Controls.Add(lblAmount, 0, 0);
            formPanel.Controls.Add(txtAmount, 1, 0);
            formPanel.Controls.Add(lblDate, 0, 1);
            formPanel.Controls.Add(dtpDate, 1, 1);
            formPanel.Controls.Add(lblCategory, 0, 2);
            formPanel.Controls.Add(cmbCategory, 1, 2);
            formPanel.Controls.Add(lblDesc, 0, 3);
            formPanel.Controls.Add(txtDesc, 1, 3);
            formPanel.Controls.Add(new Label(), 0, 4);
            formPanel.Controls.Add(btnSave, 1, 4);

            // Datentabelle
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            layout.Controls.Add(dgv, 0, 1);

            LoadIncomesIntoGrid(dgv); // Initiale Anzeige

            btnSave.Click += (s, e) =>
            {
                if (!decimal.TryParse(txtAmount.Text, out decimal amount))
                {
                    MessageBox.Show("Bitte gültigen Betrag eingeben.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var categoryName = cmbCategory.Text.Trim();
                if (string.IsNullOrEmpty(categoryName))
                {
                    MessageBox.Show("Bitte eine Kategorie angeben.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kategorie finden oder anlegen
                var cat = catRepo.GetByNameAndType(categoryName, CategoryType.Income);
                if (cat == null)
                {
                    cat = new Category { Name = categoryName, Type = CategoryType.Income };
                    catRepo.Add(cat);
                }


                var income = new Income
                {
                    UserID = 1, // Beispiel-ID, anpassen je nach Login-System
                    Amount = amount,
                    Date = dtpDate.Value,
                    Description = txtDesc.Text,
                    CategoryID = cat.CategoryID
                };

                try
                {
                    var repo = new IncomeRepository();
                    repo.Add(income);
                    MessageBox.Show("Einnahme gespeichert.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadIncomesIntoGrid(dgv);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Speichern:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void LoadIncomesIntoGrid(DataGridView grid)
        {
            try
            {
                var repo = new IncomeRepository();
                var data = repo.GetAll();

                var table = new DataTable();
                table.Columns.Add("Betrag", typeof(decimal));
                table.Columns.Add("Datum", typeof(DateTime));
                table.Columns.Add("Kategorie", typeof(string));
                table.Columns.Add("Beschreibung", typeof(string));

                foreach (var item in data)
                {
                    table.Rows.Add(item.Amount, item.Date, new CategoryRepository().GetById(item.CategoryID).Name ?? "", item.Description);
                }

                grid.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Daten:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void BuildContentGrid()
        {
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
        private void LoadExpensesView()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            contentPanel.Controls.Add(layout);

            // Eingabeformular
            var formPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(250, 245, 245)
            };
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.Controls.Add(formPanel, 0, 0);

            var lblAmount = new Label { Text = "Betrag:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var txtAmount = new TextBox { Dock = DockStyle.Fill };

            var lblDate = new Label { Text = "Datum:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var dtpDate = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };

            var lblCategory = new Label { Text = "Kategorie:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var cmbCategory = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDown,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };

            // Kategorien vom Typ Income laden
            var catRepo = new CategoryRepository();
            var incomeCategories = catRepo.GetAll().Where(c => c.Type == CategoryType.Expense).ToList();
            cmbCategory.Items.AddRange(incomeCategories.Select(c => c.Name).ToArray());

            var lblDesc = new Label { Text = "Beschreibung:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var txtDesc = new TextBox { Dock = DockStyle.Fill };

            var btnSave = new Button
            {
                Text = "Speichern",
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                Dock = DockStyle.Right,
                Height = 30
            };

            formPanel.Controls.Add(lblAmount, 0, 0);
            formPanel.Controls.Add(txtAmount, 1, 0);
            formPanel.Controls.Add(lblDate, 0, 1);
            formPanel.Controls.Add(dtpDate, 1, 1);
            formPanel.Controls.Add(lblCategory, 0, 2);
            formPanel.Controls.Add(cmbCategory, 1, 2);
            formPanel.Controls.Add(lblDesc, 0, 3);
            formPanel.Controls.Add(txtDesc, 1, 3);
            formPanel.Controls.Add(new Label(), 0, 4);
            formPanel.Controls.Add(btnSave, 1, 4);

            // Datentabelle
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            layout.Controls.Add(dgv, 0, 1);

            LoadExpensesIntoGrid(dgv); // Initiale Anzeige

            btnSave.Click += (s, e) =>
            {
                if (!decimal.TryParse(txtAmount.Text, out decimal amount))
                {
                    MessageBox.Show("Bitte gültigen Betrag eingeben.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var categoryName = cmbCategory.Text.Trim();
                if (string.IsNullOrEmpty(categoryName))
                {
                    MessageBox.Show("Bitte eine Kategorie angeben.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kategorie finden oder anlegen
                var cat = catRepo.GetByNameAndType(categoryName, CategoryType.Expense);
                if (cat == null)
                {
                    cat = new Category { Name = categoryName, Type = CategoryType.Expense };
                    catRepo.Add(cat);
                }


                var expense = new Expense
                {
                    UserID = 1, // Beispielwert
                    Amount = amount,
                    Date = dtpDate.Value,
                    Description = txtDesc.Text,
                    CategoryID = cat.CategoryID
                };

                try
                {
                    var repo = new ExpenseRepository();
                    repo.Add(expense);
                    MessageBox.Show("Ausgabe gespeichert.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadExpensesIntoGrid(dgv);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Speichern:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void LoadExpensesIntoGrid(DataGridView grid)
        {
            try
            {
                var repo = new ExpenseRepository();
                var data = repo.GetAll();

                var table = new DataTable();
                table.Columns.Add("Betrag", typeof(decimal));
                table.Columns.Add("Datum", typeof(DateTime));
                table.Columns.Add("Kategorie", typeof(string));
                table.Columns.Add("Beschreibung", typeof(string));

                foreach (var item in data)
                {
                    table.Rows.Add(item.Amount, item.Date, new CategoryRepository().GetById(item.CategoryID).Name ?? "", item.Description);
                }

                grid.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Daten:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBudgetView()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            contentPanel.Controls.Add(layout);

            var formPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(245, 250, 245)
            };
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.Controls.Add(formPanel, 0, 0);

            var lblMonth = new Label { Text = "Monat (YYYYMM):", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var txtMonth = new TextBox { Dock = DockStyle.Fill };

            var lblIncome = new Label { Text = "Geplante Einnahmen:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var txtIncome = new TextBox { Dock = DockStyle.Fill };

            var lblExpense = new Label { Text = "Geplante Ausgaben:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var txtExpense = new TextBox { Dock = DockStyle.Fill };

            var btnSave = new Button
            {
                Text = "Speichern",
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                Dock = DockStyle.Right,
                Height = 30
            };

            formPanel.Controls.Add(lblMonth, 0, 0);
            formPanel.Controls.Add(txtMonth, 1, 0);
            formPanel.Controls.Add(lblIncome, 0, 1);
            formPanel.Controls.Add(txtIncome, 1, 1);
            formPanel.Controls.Add(lblExpense, 0, 2);
            formPanel.Controls.Add(txtExpense, 1, 2);
            formPanel.Controls.Add(new Label(), 0, 3);
            formPanel.Controls.Add(btnSave, 1, 3);

            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            layout.Controls.Add(dgv, 0, 1);

            LoadBudgetIntoGrid(dgv);

            btnSave.Click += (s, e) =>
            {
                if (!int.TryParse(txtMonth.Text, out int month) || month < 202001 || month > 209912)
                {
                    MessageBox.Show("Ungültiger Monat. Format: YYYYMM", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!decimal.TryParse(txtIncome.Text, out decimal income) || !decimal.TryParse(txtExpense.Text, out decimal expense))
                {
                    MessageBox.Show("Bitte gültige Zahlen für Einnahmen/Ausgaben eingeben.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var forecast = new BudgetForecast
                {
                    UserID = 1, // Beispiel-ID
                    Month = month,
                    PlannedIncome = income,
                    PlannedExpense = expense
                };

                try
                {
                    var repo = new BudgetForecastRepository();
                    repo.Add(forecast);
                    MessageBox.Show("Budget gespeichert.", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBudgetIntoGrid(dgv);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Speichern:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }


        private void LoadBudgetIntoGrid(DataGridView grid)
        {
            try
            {
                var repo = new BudgetForecastRepository();
                var data = repo.GetAll();

                var table = new DataTable();
                table.Columns.Add("Monat", typeof(int));
                table.Columns.Add("Geplante Einnahmen", typeof(decimal));
                table.Columns.Add("Geplante Ausgaben", typeof(decimal));

                foreach (var item in data)
                {
                    table.Rows.Add(item.Month, item.PlannedIncome, item.PlannedExpense);
                }

                grid.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Daten:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
