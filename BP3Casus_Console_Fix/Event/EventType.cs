using BP3Casus_Console_Fix.Event.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP3Casus_Console_Fix.Event
{
    public class EventType
    {
        EventDataAccesLayer EventDataAccesLayer = EventDataAccesLayer.Instance;

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public double ExpPerParticipant { get; set; }


        public List<String> Tags { get; set; } = new List<string>();

        public EventType(string name, string description, double expPerParticipant)
        {
            Name = name;
            Description = description;
            ExpPerParticipant = expPerParticipant;
        }

        public void AddTag(string tag)
        {
            Tags.Add(tag);
            EventDataAccesLayer.UpdateEventType(this);
        }
        public void RemoveTag(string tag)
        {
            Tags.Remove(tag);
            EventDataAccesLayer.UpdateEventType(this);
        }

        public void Rename(string newName)
        {
            Name = newName;
            EventDataAccesLayer.UpdateEventType(this);
        }
        public void ChangeDescription(string newDescription)
        {
            Description = newDescription;
            EventDataAccesLayer.UpdateEventType(this);
        }
        public void ChangeExpPerParticipant(double newExpPerParticipant)
        {
            ExpPerParticipant = newExpPerParticipant;
            EventDataAccesLayer.UpdateEventType(this);
        }
    }
}
