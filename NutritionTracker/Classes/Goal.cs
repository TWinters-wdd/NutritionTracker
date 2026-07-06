using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionTracker.Classes
{
    public class Goal
    {
        public string GoalID { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public string OwnerID { get; set; }

        public Goal(string title, string description, string ownerID)
        {
            GoalID = Guid.NewGuid().ToString();
            Title = title;
            Description = description;
            IsCompleted = false;
            OwnerID = ownerID;
        }
    }
}
