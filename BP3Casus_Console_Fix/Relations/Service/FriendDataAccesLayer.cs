using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BP3Casus_Console_Fix.Users;

namespace BP3Casus_Console_Fix.Relations.Service
{
    public class FriendDataAccesLayer
    {
        private FriendDataAccesLayer()
        {
        }

        private string connectionString = "Data Source=.;Initial Catalog=BP3Casus_Fix;Integrated Security=True;Encrypt=False";

        private static FriendDataAccesLayer? instance = null;
        public static FriendDataAccesLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FriendDataAccesLayer();
                }
                return instance;
            }
        }

        public List<User>? GetFriends(Participant participant)
        {
            List<User> friends = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to fetch friends. We need to check both UserId1 and UserId2.
                string query = @"
            SELECT u.Id, u.Username, u.Password, u.Email, u.FirstName, u.LastName, u.DateOfBirth, u.UserType
            FROM Users u
            JOIN Friends f ON (f.UserId1 = @ParticipantId AND u.Id = f.UserId2) OR (f.UserId2 = @ParticipantId AND u.Id = f.UserId1)
            WHERE u.Id != @ParticipantId";  // Ensure we don't include the participant as their own friend

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ParticipantId", participant.Id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User friend = new User(
                                reader["Username"].ToString(),
                                reader["Password"].ToString(),
                                reader["Email"].ToString(),
                                reader["FirstName"].ToString(),
                                reader["LastName"].ToString(),
                                Convert.ToDateTime(reader["DateOfBirth"])
                            )
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Type = (User.UserType)Enum.Parse(typeof(User.UserType), reader["UserType"].ToString())
                            };
                            friends.Add(friend);
                        }
                    }
                }
            }
            return friends;
        }

        public List<FriendRequest> GetFriendRequests(Participant participant)
        {
            List<FriendRequest> friendRequests = new List<FriendRequest>();
            if (participant == null || participant.Id <= 0)
            {
                throw new ArgumentException("Invalid participant provided.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to fetch friend requests where the participant is the sender or receiver
                string query = @"
            SELECT fr.Id, fr.SenderUserId, fr.ReceiverUserId, fr.RequestDate, fr.Status
            FROM FriendRequests fr
            WHERE fr.SenderUserId = @ParticipantId OR fr.ReceiverUserId = @ParticipantId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ParticipantId", participant.Id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var friendRequest = new FriendRequest(
                                Convert.ToInt32(reader["SenderUserId"]),
                                Convert.ToInt32(reader["ReceiverUserId"]),
                                reader["RequestDate"] as DateTime? ?? default(DateTime)
                            )
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                            };
                            friendRequests.Add(friendRequest);
                        }
                    }
                }
            }
            return friendRequests;
        }



        public void CreateFriendRequest(Participant sender, Participant receiver)
        {
            if (sender == null || receiver == null)
            {
                throw new ArgumentException("Sender and receiver must not be null.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL INSERT statement to create a new friend request
                string insertQuery = @"
            INSERT INTO FriendRequests (SenderUserId, ReceiverUserId, RequestDate, Status)
            VALUES (@SenderUserId, @ReceiverUserId, @RequestDate, 'Pending')";

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@SenderUserId", sender.Id);
                    command.Parameters.AddWithValue("@ReceiverUserId", receiver.Id);
                    command.Parameters.AddWithValue("@RequestDate", DateTime.UtcNow);  // Using UTC time to standardize the time zone

                    // Execute the command
                    int result = command.ExecuteNonQuery();
                    if (result < 1)
                    {
                        throw new InvalidOperationException("Failed to insert the friend request.");
                    }
                }
            }
        }



        public void UpdateFriendRequest(FriendRequest friendRequest)
        {
            if (friendRequest == null)
            {
                throw new ArgumentException("Friend request must not be null.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                switch (friendRequest.Status)
                {
                    case FriendRequest.FriendRequestStatus.Accepted:
                        DeleteFriendRequest(friendRequest, connection);
                        break;
                    case FriendRequest.FriendRequestStatus.Declined:
                        DeleteFriendRequest(friendRequest, connection);  // Pass the connection to use the same open connection
                        break;
                    case FriendRequest.FriendRequestStatus.Pending:
                        // Update the friend request to keep it in the database
                        string updateQuery = @"
                    UPDATE FriendRequests
                    SET RequestDate = @RequestDate  -- Potentially updating the request date or any other fields needed
                    WHERE Id = @Id";
                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@RequestDate", friendRequest.RequestDate);
                            command.Parameters.AddWithValue("@Id", friendRequest.Id);
                            command.ExecuteNonQuery();
                        }
                        break;
                }
            }
        }

        private void DeleteFriendRequest(FriendRequest friendRequest, SqlConnection connection)
        {
            // SQL DELETE statement to delete the friend request
            string deleteQuery = "DELETE FROM FriendRequests WHERE Id = @Id";
            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", friendRequest.Id);
                int result = command.ExecuteNonQuery();
                if (result < 1)
                {
                    throw new InvalidOperationException("Failed to delete the friend request.");
                }
            }
        }



        public void AddFriend(Participant participant, User friend)
        {
            if (participant == null || friend == null)
            {
                throw new ArgumentException("Participant and friend must not be null.");
            }

            if (participant.Id == friend.Id)
            {
                throw new ArgumentException("A participant cannot befriend themselves.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if the friendship already exists to avoid duplicates
                string checkQuery = @"
            SELECT COUNT(*) FROM Friends
            WHERE (UserId1 = @ParticipantId AND UserId2 = @FriendId) OR
                  (UserId1 = @FriendId AND UserId2 = @ParticipantId)";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@ParticipantId", participant.Id);
                    checkCommand.Parameters.AddWithValue("@FriendId", friend.Id);
                    int exists = (int)checkCommand.ExecuteScalar();
                    if (exists > 0)
                    {
                        throw new InvalidOperationException("This friendship already exists.");
                    }
                }

                // Insert the new friendship
                string insertQuery = @"
            INSERT INTO Friends (UserId1, UserId2)
            VALUES (@UserId1, @UserId2)";
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId1", participant.Id);
                    command.Parameters.AddWithValue("@UserId2", friend.Id);
                    int result = command.ExecuteNonQuery();
                    if (result < 1)
                    {
                        throw new InvalidOperationException("Failed to add friend.");
                    }
                }
            }
        }

        public void RemoveFriend(Participant participant, User friend)
        {
            if (participant == null || friend == null)
            {
                throw new ArgumentException("Participant and friend must not be null.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Remove the friendship
                string deleteQuery = @"
            DELETE FROM Friends
            WHERE (UserId1 = @ParticipantId AND UserId2 = @FriendId) OR
                  (UserId1 = @FriendId AND UserId2 = @ParticipantId)";
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ParticipantId", participant.Id);
                    command.Parameters.AddWithValue("@FriendId", friend.Id);
                    int result = command.ExecuteNonQuery();
                    if (result < 1)
                    {
                        throw new InvalidOperationException("Failed to remove friend or friendship was not found.");
                    }
                }
            }
        }

    }
}
