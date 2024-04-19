using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP3Casus_Console_Fix.Event;
using BP3Casus_Console_Fix.Users;
using BP3Casus_Console_Fix.Users.Service;

namespace BP3Casus_Console_Fix.Users
{
    public class Coach : User
    {
        UserDataAccesLayer UserDataAccesLayer = UserDataAccesLayer.Instance;

        public List<Participant> ParticipantsToEvaluate { get; set; } = new List<Participant>();

        public AreaOfExpertise Expertise { get; set; }
        public enum AreaOfExpertise
        {
            VariabeleSport,
            VrijeKeuze,
            Kickboxen,
            KungFu,
            Bodytraining,
            Yoga,
            Fitness
        }

        public Coach(string username, string password, string email, string firstName, string lastName, DateTime dateOfBirth, AreaOfExpertise expertise) : base(username, password, email, firstName, lastName, dateOfBirth)
        {
            Type = UserType.Coach;
            Expertise = expertise;
            UserDataAccesLayer.GetParticipantsToEvaluate(this);
        }

        public void ChangeExpertise(AreaOfExpertise newExpertise)
        {
            Expertise = newExpertise;
            UserDataAccesLayer.UpdateUser(this);
        }

        public void AddParticipantToEvaluate(Participant participant)
        {
            ParticipantsToEvaluate.Add(participant);
            UserDataAccesLayer.UpdateParticipantsToEvaluate(this);
        }
        public void RemoveParticipantToEvaluate(Participant participant)
        {
            ParticipantsToEvaluate.Remove(participant);
            UserDataAccesLayer.UpdateParticipantsToEvaluate(this);
        }
        public void EvaluateParticipants(int Grade, Event.Event @event)
        {
            foreach (Participant participant in ParticipantsToEvaluate)
            {
                participant.GainGeneralExperience(Grade);

                var targetProgress = participant.progresses.FirstOrDefault(p => p.EventType == @event.EventType);
                targetProgress?.GainExperience(Grade);

                RemoveParticipantToEvaluate(participant);
            }
        }
    }
}
