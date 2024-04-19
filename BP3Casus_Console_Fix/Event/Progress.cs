using BP3Casus_Console_Fix.Event.Service;
using BP3Casus_Console_Fix.Users.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP3Casus_Console_Fix.Event
{
    public class Progress
    {
        EventDataAccesLayer EventDataAccesLayer = EventDataAccesLayer.Instance;

        public int ID { get; set; }
        public int EventTypeID { get; set; }
        public int UserID { get; set; }

        public double ExperienceNeededForLevel(int level)
        {
            // Baseline experience (30 XP)
            int baselineXP = 10;
            // Increment for the first three levels (3 XP each)
            int incrementalXP = 3 * (level - 1);
            // Exponential growth starting from level 8
            int exponentialXP = 10 * (int)Math.Pow(2, level - 8);
            // Total max XP for the given level
            int maxXP = baselineXP + incrementalXP + exponentialXP;

            return maxXP;
        }
        public int Level { get; set; }
        public double Experience { get; set; } = 0;

        public EventType? EventType
        {
            get
            {
                EventType eventType = EventDataAccesLayer.GetEventTypeById(EventTypeID);
                return eventType;
            }
        }

        public Progress(int eventTypeID, int userID)
        {
            EventTypeID = eventTypeID;
            UserID = userID;
        }

        public void GainExperience(float grade)
        {
            this.Experience += grade;
            while (this.Experience >= ExperienceNeededForLevel(this.Level + 1))
            {
                this.Level++;
            }
            EventDataAccesLayer.UpdateProgress(this);
        }
    }
}
