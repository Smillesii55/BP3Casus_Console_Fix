using BP3Casus_Console_Fix.Event.Service;
using BP3Casus_Console_Fix.Users;
using BP3Casus_Console_Fix.Users.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP3Casus_Console_Fix.Event
{
    public class Event
    {
        EventDataAccesLayer EventDataAccesLayer = EventDataAccesLayer.Instance;
        UserDataAccesLayer UserDataAccesLayer = UserDataAccesLayer.Instance;

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int MaxParticipants { get; set; }
        public bool IsOpen { get; set; }
        public List<Participant> Participants { get; set; }
        public int? CoachId { get; set; }
        public Coach? Coach
        {
            get
            {
                Coach Coach = UserDataAccesLayer.GetCoachById((int)CoachId);
                return Coach;
            }
        }
        public int? EventTypeId { get; set; }
        public EventType EventType
        {
            get
            {
                EventType eventType = EventDataAccesLayer.GetEventTypeById((int)EventTypeId);
                return eventType;
            }
        }

        public Event(string name, DateTime date, int maxParticipants)
        {
            Name = name;
            Date = date;
            MaxParticipants = maxParticipants;
            Participants = new List<Participant>();
            IsOpen = true;
        }

        public void CloseEvent()
        {
            IsOpen = false;
            EventDataAccesLayer.UpdateEvent(this);
        }
        public void OpenEvent()
        {
            IsOpen = true;
            EventDataAccesLayer.UpdateEvent(this);
        }

        public void AddParticipant(Participant participant)
        {
            if (Participants.Count < MaxParticipants)
            {
                Participants.Add(participant);
            } else
            {
                CloseEvent();
            }
            EventDataAccesLayer.UpdateEventParticipants(this);
        }
        public void RemoveParticipant(Participant participant)
        {
            Console.WriteLine("Attempting to remove participant with ID: " + participant.Id);
            Console.WriteLine("Current participants:");
            foreach (var p in Participants)
            {
                Console.WriteLine("Participant ID: " + p.Id);
            }

            if (Participants.Contains(participant))
            {
                Participants.Remove(participant);
                EventDataAccesLayer.UpdateEventParticipants(this);
            }
            else
            {
                Console.WriteLine("Participant not found in the list.");
            }
        }


        public void SetCoach(Coach coach)
        {
            CoachId = coach.Id;
            EventDataAccesLayer.UpdateEvent(this);
        }

        public void ChangeName(string newName)
        {
            Name = newName;
            EventDataAccesLayer.UpdateEvent(this);
        }

        public void ChangeDate(DateTime newDate)
        {
            Date = newDate;
            EventDataAccesLayer.UpdateEvent(this);
        }

        public void ChangeMaxParticipants(int newMaxParticipants)
        {
            MaxParticipants = newMaxParticipants;
            EventDataAccesLayer.UpdateEvent(this);
        }

        public void ChangeEventType(EventType newEventType)
        {
            EventTypeId = newEventType.ID;
            EventDataAccesLayer.UpdateEvent(this);
        }

        public void RemoveCoach()
        {
            CoachId = null;
            EventDataAccesLayer.UpdateEvent(this);
        }
    }
}
