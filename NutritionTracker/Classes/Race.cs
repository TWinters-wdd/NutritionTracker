using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionTracker.Classes
{
    public class Race
    {
        // 1. Code generates on room creation, User sets the Race Length
        public string RoomCode { get; private set; }
        public string RaceLength { get; set; }

        // 2. Private data
        private List<Player> _racers = new List<Player>();
        private string _raceStatus;

        // 3. Public Properties
        public IReadOnlyList<Player> Racers
        {
            get { return _racers.AsReadOnly(); }
        }

        public string RaceStatus
        {
            get { return _raceStatus; }
            set { _raceStatus = value; }
        }

        // 4. Race Constructor
        public Race(string raceLength)
        {
            RaceLength = raceLength;
            RaceStatus = "Waiting for Players: ";

            // Generate a GUID, convert to string, only take first 6 characters
            RoomCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }

        // 5. Race Methods
        public void AddRacer(Player newPlayer)
        {
            // Only add the player if they aren't null, max of 4 racers in each room
            if (newPlayer != null && _racers.Count < 4)
            {
                _racers.Add(newPlayer);
            }
        }
    }
}
