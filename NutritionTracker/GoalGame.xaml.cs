using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NutritionTracker.Classes;

namespace NutritionTracker
{
    /* 
        Uses the Goal class, Player class, and the Race class
        This page will display a racetrack with horses on it. Each horse represents
        a separate online player.
        Every time a user completes a goal, their horse moves closer to the finish line.
     */
    public partial class GoalGame : Page
    {
        public GoalGame()
        {
            InitializeComponent();
        }
    }
}
