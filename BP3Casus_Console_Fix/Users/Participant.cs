using BP3Casus_Console_Fix.Users.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP3Casus_Console_Fix.Event;
using BP3Casus_Console_Fix.Users;
using BP3Casus_Console_Fix.Event.Service;
using BP3Casus_Console_Fix.Relations.Service;
using BP3Casus_Console_Fix.Relations;

namespace BP3Casus_Console_Fix.Users
{
    public class Participant : User
    {
        UserDataAccesLayer UserDataAccesLayer = UserDataAccesLayer.Instance;
        EventDataAccesLayer EventDataAccesLayer = EventDataAccesLayer.Instance;
        FriendDataAccesLayer FriendDataAccesLayer = FriendDataAccesLayer.Instance;

        public double ExperienceNeededForGeneralLevel(int level)
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
        public int GeneralLevel { get; set; } = 1;
        public double GeneralExperience { get; set; } = 0;

        public List<User> Friends { get; set; } = new List<User>();
        public List<FriendRequest> FriendRequests { get; set; } = new List<FriendRequest>();
        public List<Progress> progresses { get; set; } = new List<Progress>();

        public Participant(string username, string password, string email, string firstName, string lastName, DateTime dateOfBirth) : base(username, password, email, firstName, lastName, dateOfBirth)
        {
            Type = UserType.Participant;
        }

        public void GainGeneralExperience(double xp)
        {
            this.GeneralExperience += xp;
            while (this.GeneralExperience >= ExperienceNeededForGeneralLevel(this.GeneralLevel + 1))
            {
                this.GeneralLevel++;
                UserDataAccesLayer.UpdateUser(this);
            }
        }

        public void GainProgressExperience(float xp, Progress progress)
        {
            progress.GainExperience(xp);
        }

        public void ParticipateInEvent(Event.Event eventToParticipateIn)
        {
            eventToParticipateIn.AddParticipant(this);
        }

        public void LeaveEvent(Event.Event eventToLeave)
        {
            eventToLeave.RemoveParticipant(this);
        }

        public void SendFriendRequest(Participant friend)
        {
            FriendDataAccesLayer.CreateFriendRequest(this, friend);
        }

        public void AddFriend(User friend)
        {
            GetFriends();

            if (Friends.Any(f => f.Id == friend.Id))
            {
                return;
            }
            else
            {
                Friends.Add(friend);
                FriendDataAccesLayer.AddFriend(this, friend);
            }
        }

        public void RemoveFriend(User friend)
        {
            if (Friends.Contains(friend))
            {
                Friends.Remove(friend);
                FriendDataAccesLayer.RemoveFriend(this, friend);
            }
        }

        public void GetFriendRequests()
        {
            FriendRequests = FriendDataAccesLayer.GetFriendRequests(this);
        }

        public void GetFriends()
        {
            Friends = FriendDataAccesLayer.GetFriends(this);
        }

        public void GetProgresses()
        {
            progresses = EventDataAccesLayer.GetProgresses(this);
        }




        // I DON'T GET WHAT THIS METHOD DOES BUT IT FIXES THE REMOVE PARTICIPANT METHOD IN EVENT!
        // DO NOT REMOVE THIS METHOD
        public override bool Equals(object obj)
        {
            return obj is Participant participant &&
                   Id == participant.Id;
        }
        // THIS METHOD IS ALSO NEEDED FOR THE REMOVE PARTICIPANT METHOD IN EVENT
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
