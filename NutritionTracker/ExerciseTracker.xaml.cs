using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using Dapper;
using System.Linq;
using System;

namespace NutritionTracker
{
    public partial class ExerciseTracker : Page
    {
        public ObservableCollection<ExerciseEntry> ExerciseEntries { get; set; } = new ObservableCollection<ExerciseEntry>();

        // SQLite connection string
        private readonly string _dbConnectionString = "Data Source=WeeklyExercises.db;Version=3;";
        private string _currentDay = "Monday";
        private bool _isInitialized = false;

        public ExerciseTracker()
        {
            InitializeComponent();

            try
            {
                ExerciseDataGrid.ItemsSource = ExerciseEntries;

                // Set up the DataBase and load the default day
                InitializeDatabase();
                LoadExercisesForDay(_currentDay);

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}",
                                "Startup Crash",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        public class ExerciseEntry
        {
            public int Id { get; set; }
            public string ExerciseName { get; set; }
            public int Reps { get; set; }
            public int Sets { get; set; }
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS Exercises (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DayOfWeek TEXT,
                        ExerciseName TEXT,
                        Reps INTEGER,
                        Sets INTEGER
                    )");
            }
        }

        private void LoadExercisesForDay(string dayOfWeek)
        {
            _currentDay = dayOfWeek;
            ExerciseEntries.Clear();

            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                // Dapper maps the SQL results into ExerciseEntry objects
                var loadedExercises = connection.Query<ExerciseEntry>(
                    "SELECT * FROM Exercises WHERE DayOfWeek = @Day",
                    new { Day = dayOfWeek }
                    ).ToList();

                foreach (var exercise in loadedExercises)
                {
                    ExerciseEntries.Add(exercise);
                }
            }
        }

        // Triggered when user changes the day in the ComboBox
        private void ExerciseDayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;

            if (ExerciseDayComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedDay = selectedItem.Content.ToString();
                LoadExercisesForDay(selectedDay);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var newExercise = new ExerciseEntry
            {
                ExerciseName = "New Exercise",
                Reps = 0,
                Sets = 0
            };

            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                string sql = @"
                    INSERT INTO Exercises (DayOfWeek, ExerciseName, Reps, Sets)
                    VALUES (@DayOfWeek, @ExerciseName, @Reps, @Sets);
                    SELECT last_insert_rowid();";

                // Insert into DB and grab the newly generated Id
                int newId = connection.QuerySingle<int>(sql, new
                {
                    DayOfWeek = _currentDay,
                    ExerciseName = newExercise.ExerciseName,
                    Reps = newExercise.Reps,
                    Sets = newExercise.Sets
                });

                // Update the object and add it to UI
                newExercise.Id = newId;
                ExerciseEntries.Add(newExercise);
            }
        }

        private void SubtractButton_Click(object sender, RoutedEventArgs e)
        {
           if (ExerciseDataGrid.SelectedItem is ExerciseEntry selectedEntry)
           {
                using (var connection = new SQLiteConnection(_dbConnectionString))
                {
                    // Delete item from DB using the unique ID
                    connection.Execute(
                        "DELETE FROM Exercises WHERE Id = @Id",
                        new { Id = selectedEntry.Id }
                        );
                }

                // Remove from UI
                ExerciseEntries.Remove(selectedEntry);

           }
           else
           {
                MessageBox.Show("Please select a row to delete.");
           }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = new SQLiteConnection(_dbConnectionString))
            {
                // Update every row for the current day in the database
                foreach (var entry in ExerciseEntries)
                {
                    connection.Execute(@"
                        UPDATE Exercises
                        SET ExerciseName = @ExerciseName, Reps = @Reps, Sets = @Sets
                        WHERE Id = @Id", entry);
                }
            }
            MessageBox.Show("Workouts saved successfully!");
        }
    }
}
