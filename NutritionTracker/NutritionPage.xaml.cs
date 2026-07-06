using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.SkiaSharpView.WPF;
using LiveChartsCore;
using System.Linq;
using LiveChartsCore.SkiaSharpView;
using System.Data.SQLite;
using Dapper;

namespace NutritionTracker
{
    public partial class NutritionPage : Page
    {
        // Initialize a collection of entries from the AddFoodWindow page
        // Initialize a collection of ISeries for the LiveCharts2 Pie chart.
        public ObservableCollection<DailyEntry> Entries { get; set; } = new ObservableCollection<DailyEntry>();

        public ObservableCollection<ISeries> Series { get; set; } = new ObservableCollection<ISeries>();
        public NutritionPage()
        {
            InitializeComponent();

            DataGrid.ItemsSource = Entries;

            // 1. Create database file and table if they don't exist yet
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NutritionTracker");
            if (!Directory.Exists(appDataFolder)) Directory.CreateDirectory(appDataFolder);

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // SQL command to create a structured table if it doesn't already exist
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS FoodEntries (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        FoodName TEXT,
                        CaloriesPerServing REAL,
                        TotalServings REAL
                    );";

                connection.Execute(createTableQuery);
            }

            this.Loaded += (s, e) =>
            {
                LoadFromFile();
                UpdateChart();
            };
        }

        private void AddFoodBtn_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddFoodWindow();
            if (addWindow.ShowDialog() == true)
            {
                var newEntry = new DailyEntry
                {
                    Date = addWindow.Date,
                    FoodName = addWindow.FoodName,
                    CaloriesPerServing = addWindow.CaloriesPerServing,
                    TotalServings = addWindow.Servings
                };

                Entries.Add(newEntry);
                SaveToFile();

                UpdateChart();
            }
        }

        private readonly string _connectionString = $"Data Source={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NutritionTracker", "NutritionData.db")};Version=3";

        private void SaveToFile()
        {
            try
            {
                var latestEntry = Entries.LastOrDefault();
                if (latestEntry == null) return;

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    // SQL command to insert a new row
                    // Dapper automatically takes the properties from latestEntry and injects them into the '@' variables
                    string insertQuery = @"
                        INSERT INTO FoodEntries (Date, Foodname, CaloriesPerServing, TotalServings)
                        VALUES (@Date, @FoodName, @CaloriesPerServing, @TotalServings);";

                    connection.Execute(insertQuery, latestEntry);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Save error: {ex.Message}");
            }
        }

        private void LoadFromFile()
        {
            try
            {
               using (var connection = new SQLiteConnection( _connectionString))
               {
                    connection.Open();

                    // SQL command that fetches every row out of the table
                    string selectQuery = "SELECT Date, FoodName, CaloriesPerServing, TotalServings FROM FoodEntries;";

                    // Dapper then reads the database columns and turns them back into a List of DailyEntry objects
                    var loadedEntries = connection.Query<DailyEntry>(selectQuery).ToList();

                    Entries.Clear();
                    foreach (var item in loadedEntries)
                    {
                        Entries.Add(item);
                    }
               }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Load error: {ex.Message}");
            }
        }

        public class DailyEntry
        {
            public string Date { get; set; }
            public string FoodName { get; set; } = string.Empty;
            public double CaloriesPerServing { get; set; }
            public double TotalServings { get; set; }

            public double TotalCalories => CaloriesPerServing * TotalServings;
        }

        public void UpdateChart()
        {
            Series.Clear();

            var currentItems = DataGrid.ItemsSource as IEnumerable<DailyEntry> ?? Entries;

            foreach (var item in currentItems)
            {
                Series.Add(new PieSeries<double>
                {
                    Values = new double[] { item.TotalCalories },
                    Name = item.FoodName
                });
            }
        }

        // Function that updates the DataGrid based on a date selected in a Date Picker
        // It then updates the Pie Chart after changing the DataGrid
        public void UpdateDataGridBySelectedDate()
        { 
            if (FilteredDate.SelectedDate == null)
            {
                DataGrid.ItemsSource = Entries;
            }
            else
            {
                string filterDate = FilteredDate.SelectedDate.Value.ToShortDateString();

                var filteredList = Entries.Where(entry => entry.Date == filterDate).ToList();

                DataGrid.ItemsSource = filteredList;
            }

            UpdateChart();
        }

        private void FilteredDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDataGridBySelectedDate();
        }
    }
}