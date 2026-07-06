using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace NutritionTracker
{
    public partial class AddFoodWindow : Window
    {

        // Indexes of Macro Food Information
        const int FoodNameIndex = 1;
        const int CalPerServingIndex = 2;
        const int FoodProteinIndex = 3;
        const int FoodTotalFatIndex = 4;
        const int FoodCarbsIndex = 5;
        const int FoodSodiumIndex = 6;
        const int FoodSaturatedFatIndex = 7;
        const int FoodCholesterolIndex = 8;
        const int FoodSugarIndex = 9;
        const int FoodCalciumIndex = 10;
        const int FoodIronIndex = 11;
        const int FoodPotassiumIndex = 12;
        const int FoodVitaminCIndex = 13;
        const int FoodVitaminEIndex = 14;
        const int FoodVitaminDIndex = 15;

        public string Date { get; private set; }
        public string FoodName { get; private set; }
        public float CaloriesPerServing { get; private set; }
        public float Servings { get; private set; }
        public float TotalCalories { get; private set; }

        // Master list of parsed foods in memory
        private List<FoodItem> _allFoods = new List<FoodItem>();

        public AddFoodWindow()
        {
            InitializeComponent();
            LoadFoodDatabase();
        }

        private void LoadFoodDatabase()
        {
            string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string csvPath = System.IO.Path.Combine(baseDir, "NutritionCSV", "USDA.csv");


            if (!File.Exists(csvPath))
            {
                // Message box shows if it can't be found
                MessageBox.Show($"Could not find the database at:\n{csvPath}");
                return;
            }

            try
            {
                var lines = File.ReadAllLines(csvPath);

                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;

                    // Custom CSV parser to account for the commas in my CSV file
                    string[] columns = SplitCsvLine(lines[i]);

                    if (columns.Length > FoodNameIndex)
                    {
                        _allFoods.Add(new FoodItem
                        {
                            Name = columns[FoodNameIndex],
                            RowData = columns
                        });
                    }
                }

                FoodNameBox.ItemsSource = _allFoods;
                var view = CollectionViewSource.GetDefaultView(FoodNameBox.ItemsSource);
                view.Filter = FilterFoodItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading food database: {ex.Message}");
            }
        }

        private string[] SplitCsvLine(string line)
        {
            List<string> result = new List<string>();
            bool isInQuotes = false;
            System.Text.StringBuilder currentField = new System.Text.StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    isInQuotes = !isInQuotes;
                }
                else if (c == ',' && !isInQuotes)
                {
                    result.Add(currentField.ToString().Trim());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }
            result.Add(currentField.ToString().Trim());
            return result.ToArray();
        }

        // Custom filter method
        private bool FilterFoodItems(object obj)
        {
            if (string.IsNullOrEmpty(FoodNameBox.Text)) return true;

            var food = obj as FoodItem;

            if (food == null || string.IsNullOrEmpty(food.Name))
                return false;

            // Returns case-insensitive value
            return food.Name.IndexOf(FoodNameBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void FoodNameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Captures when filtering triggers an input change or user types on their own
            if (FoodNameBox.SelectedItem is FoodItem selectedFood)
            {
                string[] data = selectedFood.RowData;

                // Helper method extracts an index and pushes it into a text
                SetBoxText(CaloriesPerServingBox, data, CalPerServingIndex);
                SetBoxText(ProteinBox, data, FoodProteinIndex);
                SetBoxText(TotalFatBox, data, FoodTotalFatIndex);
                SetBoxText(CarbsBox, data, FoodCarbsIndex);
                SetBoxText(SodiumBox, data, FoodSodiumIndex);
                SetBoxText(SaturatedFatBox, data, FoodSaturatedFatIndex);
                SetBoxText(CholesterolBox, data, FoodCholesterolIndex);
                SetBoxText(SugarBox, data, FoodSugarIndex);
                SetBoxText(CalciumBox, data, FoodCalciumIndex);
                SetBoxText(IronBox, data, FoodIronIndex);
                SetBoxText(PotassiumBox, data, FoodPotassiumIndex);
                SetBoxText(VitaminCBox, data, FoodVitaminCIndex);
                SetBoxText(VitaminEBox, data, FoodVitaminEIndex);
                SetBoxText(VitaminDBox, data, FoodVitaminDIndex);

                // Code to expand the Macro section automatically
                MacroExpander.IsExpanded = true;
            }
        }

        private void SetBoxText(TextBox box, string[] row, int index)
        {
            if (box != null && index < row.Length)
            {
                string value = row[index];

                // Default database field to 0 when blank
                if (string.IsNullOrWhiteSpace(value))
                {
                    box.Text = "0";
                }
                else
                {
                    box.Text = value;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime selectedDate = SelectedDatePicker.SelectedDate ?? DateTime.Today;
                Date = selectedDate.ToShortDateString();
                FoodName = FoodNameBox.Text;
                Servings = float.Parse(ServingsBox.Text);
                CaloriesPerServing = float.Parse(CaloriesPerServingBox.Text);
                TotalCalories = Servings * CaloriesPerServing;

                DialogResult = true;
            }
            catch
            {
                MessageBox.Show("Please enter valid numbers for servings and calories.");
            }
        }

        private void FoodNameBox_KeyUp(object sender, KeyEventArgs e)
        {
            // Ignore arrow keys, enter key, escape key, and tab key.
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Enter || e.Key == Key.Escape || e.Key == Key.Tab)
            {
                return;
            }

            // Captures the internal TextBox and saves the exact position of the blinking cursor
            TextBox internalTextBox = e.OriginalSource as TextBox;
            int cursorPosition = internalTextBox?.CaretIndex ?? 0;

            // Force the filter to re-evaluate the list based on new text
            var view = CollectionViewSource.GetDefaultView(FoodNameBox.ItemsSource);
            if (view != null)
            {
                view.Refresh();

                // Reaches into the Combo Box template and finds the arrow button
                var arrowButton = FoodNameBox.Template.FindName("PART_ToggleButton", FoodNameBox) as System.Windows.Controls.Primitives.ToggleButton;

                // Check if current typed text results in zero matches from CSV file
                if (view.IsEmpty)
                {
                    FoodNameBox.IsDropDownOpen = false;

                    // Hides the dropdown arrow completely
                    if (arrowButton != null) arrowButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Brings the dropdown arrow back
                    if (arrowButton != null) arrowButton.Visibility = Visibility.Visible;

                    // If there are matches, open the dropdown so user can see them
                    if (!FoodNameBox.IsDropDownOpen)
                    {
                        FoodNameBox.IsDropDownOpen = true;
                    }
                }
            }

            // Restores the cursor position and removes the blue highlight
            if (internalTextBox != null)
            {
                internalTextBox.CaretIndex = cursorPosition;
                internalTextBox.SelectionLength = 0; // <-- This line specifically removes the highlight
            }
        }
    }

    public class FoodItem
    {
        public string Name { get; set; }
        public string[] RowData { get; set; }

        public override string ToString() => Name;
    }
}
