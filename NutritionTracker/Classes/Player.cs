using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionTracker.Classes
{
    public class Player
    {
        // 1. Gets UserID, Username, and initializes a list of user goals
        public string UserID { get; private set; }
        public string Username { get; set; }
        private List<Goal> _userGoals = new List<Goal>();

        // 2. Create a read only version of the list to the UI
        public IReadOnlyList<Goal> UserGoals
        {
            get { return _userGoals.AsReadOnly(); }
        }

        // 3. Calculate the percentage of goals completed (0.0 to 100.0)
        public double CurrentProgress
        {
            get
            {
                // Edge case: no goals yet, progress = 0%
                if (_userGoals.Count == 0)
                {
                    return 0;
                }
                // Count the total goals and the goals that are marked complete using LINQ
                int completedCount = _userGoals.Count(g => g.IsCompleted == true);
                int totalCount = _userGoals.Count;

                // Calculation: (Completed / Total) * 100
                return ((double)completedCount / totalCount) * 100;
            }
        }

        // Player constructor
        public Player(string username)
        {
            Username = username;
            UserID = Guid.NewGuid().ToString();
        }

        public void AddGoal(Goal newGoal)
        {
            if (newGoal != null)
            {
                _userGoals.Add(newGoal);
            }
        }
    }
}
