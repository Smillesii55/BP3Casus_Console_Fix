using BP3Casus_Console_Fix.Relations.Service;
using BP3Casus_Console_Fix.Users;
using BP3Casus_Console_Fix.Users.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP3Casus_Console_Fix.Relations
{
    public class FriendRequest
    {
        FriendDataAccesLayer FriendDataAccesLayer = FriendDataAccesLayer.Instance;
        UserDataAccesLayer UserDataAccesLayer = UserDataAccesLayer.Instance;

        public int Id { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public DateTime RequestDate { get; set; }
        public FriendRequestStatus Status { get; set; }
        public enum FriendRequestStatus
        {
            Pending,
            Accepted,
            Declined
        }

        public Participant? Sender
        {
            get
            {
                Participant participant = UserDataAccesLayer.GetParticipantById(SenderUserId);
                return participant;
            }
        }
        public Participant Receiver
        {
            get
            {
                Participant participant = UserDataAccesLayer.GetParticipantById(ReceiverUserId);
                return participant;
            }
        }

        public FriendRequest(int senderUserId, int receiverUserId, DateTime requestDate)
        {
            SenderUserId = senderUserId;
            ReceiverUserId = receiverUserId;
            RequestDate = requestDate;
            Status = FriendRequestStatus.Pending;
        }

        public void Accept()
        {
            this.Status = FriendRequestStatus.Accepted;
            Receiver.AddFriend(Sender);
            FriendDataAccesLayer.UpdateFriendRequest(this);
        }
        public void Decline()
        {
            Status = FriendRequestStatus.Declined;
            FriendDataAccesLayer.UpdateFriendRequest(this);
        }
    }
}
