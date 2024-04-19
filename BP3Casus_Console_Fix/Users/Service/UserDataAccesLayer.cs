using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BP3Casus_Console_Fix.Users;
using BP3Casus_Console_Fix.Event;

namespace BP3Casus_Console_Fix.Users.Service
{
    public class UserDataAccesLayer
    {
        private UserDataAccesLayer()
        {
        }

        private string connectionString = "Data Source=.;Initial Catalog=BP3Casus_Fix;Integrated Security=True;Encrypt=False";

        private static UserDataAccesLayer? instance = null;
        public static UserDataAccesLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserDataAccesLayer();
                }
                return instance;
            }
        }

        public User? GetUserByCredentials(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Updated query to select only from Users since passwords are stored there
                string query = "SELECT Id, Email, FirstName, LastName, DateOfBirth, UserType FROM Users WHERE Username = @Username AND Password = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(reader.GetOrdinal("Id"));
                            string email = reader.GetString(reader.GetOrdinal("Email"));
                            string firstName = reader.GetString(reader.GetOrdinal("FirstName"));
                            string lastName = reader.GetString(reader.GetOrdinal("LastName"));
                            DateTime dateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth"));
                            User.UserType userType = (User.UserType)Enum.Parse(typeof(User.UserType), reader.GetString(reader.GetOrdinal("UserType")));

                            // Close the reader before making another query
                            reader.Close();

                            if (userType == User.UserType.Participant)
                            {
                                // Fetch additional participant details
                                string participantQuery = "SELECT GeneralLevel, GeneralExperience FROM Participants WHERE UserId = @UserId";
                                using (SqlCommand participantCommand = new SqlCommand(participantQuery, connection))
                                {
                                    participantCommand.Parameters.AddWithValue("@UserId", userId);
                                    using (SqlDataReader participantReader = participantCommand.ExecuteReader())
                                    {
                                        if (participantReader.Read())
                                        {
                                            int generalLevel = participantReader.GetInt32(participantReader.GetOrdinal("GeneralLevel"));
                                            double generalExperience = participantReader.GetDouble(participantReader.GetOrdinal("GeneralExperience"));

                                            Participant participant = new Participant(username, password, email, firstName, lastName, dateOfBirth);
                                            participant.Id = userId;
                                            participant.GeneralLevel = generalLevel;
                                            participant.GeneralExperience = generalExperience;
                                            return participant;
                                        }
                                    }
                                }
                            }

                            else if (userType == User.UserType.Coach)
                            {
                                // Fetch additional coach details
                                string coachQuery = "SELECT Expertise FROM Coaches WHERE UserId = @UserId";
                                using (SqlCommand coachCommand = new SqlCommand(coachQuery, connection))
                                {
                                    coachCommand.Parameters.AddWithValue("@UserId", userId);
                                    using (SqlDataReader coachReader = coachCommand.ExecuteReader())
                                    {
                                        if (coachReader.Read())
                                        {
                                            Coach.AreaOfExpertise expertise = (Coach.AreaOfExpertise)Enum.Parse(typeof(Coach.AreaOfExpertise), coachReader.GetString(coachReader.GetOrdinal("Expertise")));

                                            Coach coach = new Coach(username, password, email, firstName, lastName, dateOfBirth, expertise);
                                            coach.Id = userId;
                                            return coach;
                                        }
                                    }
                                }
                            }

                            // IF WE EVER ADD AN ADMIN USER TYPE, ADD IT HERE!
                        }
                    }
                }
            }
            return null;
        }

        public Participant? GetParticipantById(int userId)
        {
            Participant? participant = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT u.*, p.GeneralLevel, p.GeneralExperience
            FROM Users u
            INNER JOIN Participants p ON u.Id = p.UserId
            WHERE u.Id = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            participant = new Participant(
                                reader["Username"].ToString(),
                                reader["Password"].ToString(), // Password should not actually be handled like this
                                reader["Email"].ToString(),
                                reader["FirstName"].ToString(),
                                reader["LastName"].ToString(),
                                (DateTime)reader["DateOfBirth"]
                            )
                            {
                                Id = userId,
                                GeneralLevel = (int)reader["GeneralLevel"],
                                GeneralExperience = (double)reader["GeneralExperience"]
                            };
                        }
                    }
                }
            }

            return participant;
        }
        public Participant? GetParticipantByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.");
            }

            Participant? participant = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT u.*, p.GeneralLevel, p.GeneralExperience
            FROM Users u
            INNER JOIN Participants p ON u.Id = p.UserId
            WHERE u.Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            participant = new Participant(
                                reader["Username"].ToString(),
                                reader["Password"].ToString(), // Note: Handle passwords securely
                                reader["Email"].ToString(),
                                reader["FirstName"].ToString(),
                                reader["LastName"].ToString(),
                                (DateTime)reader["DateOfBirth"]
                            )
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                GeneralLevel = reader.GetInt32(reader.GetOrdinal("GeneralLevel")),
                                GeneralExperience = reader.GetDouble(reader.GetOrdinal("GeneralExperience"))
                            };
                        }
                    }
                }
            }

            return participant;
        }


        public Coach? GetCoachById(int userId)
        {
            Coach? coach = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT u.*, c.Expertise
            FROM Users u
            INNER JOIN Coaches c ON u.Id = c.UserId
            WHERE u.Id = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            coach = new Coach(
                                reader["Username"].ToString(),
                                reader["Password"].ToString(), // Again, handling passwords should be done securely
                                reader["Email"].ToString(),
                                reader["FirstName"].ToString(),
                                reader["LastName"].ToString(),
                                (DateTime)reader["DateOfBirth"],
                                (Coach.AreaOfExpertise)Enum.Parse(typeof(Coach.AreaOfExpertise), reader["Expertise"].ToString())
                            )
                            {
                                Id = userId
                            };
                        }
                    }
                }
            }

            return coach;
        }
        public Coach? GetCoachByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.");
            }

            Coach? coach = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT u.*, c.Expertise
            FROM Users u
            INNER JOIN Coaches c ON u.Id = c.UserId
            WHERE u.Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            coach = new Coach(
                                reader["Username"].ToString(),
                                reader["Password"].ToString(), // Handling passwords should be secure
                                reader["Email"].ToString(),
                                reader["FirstName"].ToString(),
                                reader["LastName"].ToString(),
                                (DateTime)reader["DateOfBirth"],
                                (Coach.AreaOfExpertise)Enum.Parse(typeof(Coach.AreaOfExpertise), reader["Expertise"].ToString())
                            )
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id"))
                            };
                        }
                    }
                }
            }

            return coach;
        }


        public User CreateUser(string username, string password, string email, string firstName, string lastName, DateTime dateOfBirth, User.UserType userType)
        {
            User user = new User(username, password, email, firstName, lastName, dateOfBirth);
            if (userType == User.UserType.Participant)
            {
                user = new Participant(username, password, email, firstName, lastName, dateOfBirth);
            }
            else if (userType == User.UserType.Coach)
            {
                user = new Coach(username, password, email, firstName, lastName, dateOfBirth, Coach.AreaOfExpertise.VariabeleSport);
            }
            AddUser(user);
            return user;
        }

        public void AddUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if username is already taken
                string checkUsernameQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (SqlCommand checkCommand = new SqlCommand(checkUsernameQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@Username", user.Username);
                    int exists = (int)checkCommand.ExecuteScalar();

                    if (exists > 0)
                    {
                        throw new ArgumentException("Username is already taken.");
                    }
                }

                // Start a transaction
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert into Users table
                    string insertUserQuery = @"
                INSERT INTO Users (Username, Password, Email, FirstName, LastName, DateOfBirth, UserType) 
                VALUES (@Username, @Password, @Email, @FirstName, @LastName, @DateOfBirth, @UserType);
                SELECT SCOPE_IDENTITY();"; // To get the last inserted Id

                    int userId = 0;
                    using (SqlCommand insertCommand = new SqlCommand(insertUserQuery, connection, transaction))
                    {
                        insertCommand.Parameters.AddWithValue("@Username", user.Username);
                        insertCommand.Parameters.AddWithValue("@Password", user.Password); // Should be hashed and salted
                        insertCommand.Parameters.AddWithValue("@Email", user.Email);
                        insertCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                        insertCommand.Parameters.AddWithValue("@LastName", user.LastName);
                        insertCommand.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                        insertCommand.Parameters.AddWithValue("@UserType", user.Type.ToString());

                        userId = Convert.ToInt32(insertCommand.ExecuteScalar());
                    }

                    if (user is Participant participant)
                    {
                        // Insert into Participants table
                        string insertParticipantQuery = @"
                    INSERT INTO Participants (UserId, GeneralLevel, GeneralExperience)
                    VALUES (@UserId, @GeneralLevel, @GeneralExperience);";

                        using (SqlCommand participantCommand = new SqlCommand(insertParticipantQuery, connection, transaction))
                        {
                            participantCommand.Parameters.AddWithValue("@UserId", userId);
                            participantCommand.Parameters.AddWithValue("@GeneralLevel", participant.GeneralLevel);
                            participantCommand.Parameters.AddWithValue("@GeneralExperience", participant.GeneralExperience);
                            participantCommand.ExecuteNonQuery();
                        }
                    }
                    else if (user is Coach coach)
                    {
                        // Insert into Coaches table
                        string insertCoachQuery = @"
                    INSERT INTO Coaches (UserId, Expertise)
                    VALUES (@UserId, @Expertise);";

                        using (SqlCommand coachCommand = new SqlCommand(insertCoachQuery, connection, transaction))
                        {
                            coachCommand.Parameters.AddWithValue("@UserId", userId);
                            coachCommand.Parameters.AddWithValue("@Expertise", coach.Expertise.ToString());
                            coachCommand.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void UpdateUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Update the Users table
                    string updateUserQuery = @"
                UPDATE Users
                SET
                    Username = @Username,
                    Password = @Password,
                    Email = @Email,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    DateOfBirth = @DateOfBirth,
                    UserType = @UserType
                WHERE Id = @Id";

                    using (SqlCommand updateUserCommand = new SqlCommand(updateUserQuery, connection, transaction))
                    {
                        updateUserCommand.Parameters.AddWithValue("@Username", user.Username);
                        updateUserCommand.Parameters.AddWithValue("@Password", user.Password); // Use a hashed password instead
                        updateUserCommand.Parameters.AddWithValue("@Email", user.Email);
                        updateUserCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                        updateUserCommand.Parameters.AddWithValue("@LastName", user.LastName);
                        updateUserCommand.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                        updateUserCommand.Parameters.AddWithValue("@UserType", user.Type.ToString());
                        updateUserCommand.Parameters.AddWithValue("@Id", user.Id);

                        updateUserCommand.ExecuteNonQuery();
                    }

                    // Update the Participants or Coaches table if necessary
                    if (user is Participant participant)
                    {
                        string updateParticipantQuery = @"
                    UPDATE Participants
                    SET
                        GeneralLevel = @GeneralLevel,
                        GeneralExperience = @GeneralExperience
                    WHERE UserId = @UserId";

                        using (SqlCommand updateParticipantCommand = new SqlCommand(updateParticipantQuery, connection, transaction))
                        {
                            updateParticipantCommand.Parameters.AddWithValue("@GeneralLevel", participant.GeneralLevel);
                            updateParticipantCommand.Parameters.AddWithValue("@GeneralExperience", participant.GeneralExperience);
                            updateParticipantCommand.Parameters.AddWithValue("@UserId", participant.Id);

                            updateParticipantCommand.ExecuteNonQuery();
                        }
                    }
                    else if (user is Coach coach)
                    {
                        string updateCoachQuery = @"
                    UPDATE Coaches
                    SET
                        Expertise = @Expertise
                    WHERE UserId = @UserId";

                        using (SqlCommand updateCoachCommand = new SqlCommand(updateCoachQuery, connection, transaction))
                        {
                            updateCoachCommand.Parameters.AddWithValue("@Expertise", coach.Expertise.ToString());
                            updateCoachCommand.Parameters.AddWithValue("@UserId", coach.Id);

                            updateCoachCommand.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // If anything goes wrong, roll back the transaction
                    transaction.Rollback();
                    throw; // Re-throw the exception to be handled by the calling code
                }
            }
        }

        public void RemoveUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Delete from CoachesParticipants if user is a coach or participant
                    string deleteFromCoachesParticipantsQuery = @"
                DELETE FROM CoachesParticipants WHERE CoachId = @UserId OR ParticipantId = @UserId";
                    using (SqlCommand cmd = new SqlCommand(deleteFromCoachesParticipantsQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@UserId", user.Id);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete from ParticipantsEvents if user is a participant
                    if (user.Type == User.UserType.Participant)
                    {
                        string deleteParticipantEventsQuery = "DELETE FROM ParticipantsEvents WHERE ParticipantId = @UserId";
                        using (SqlCommand cmd = new SqlCommand(deleteParticipantEventsQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@UserId", user.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Delete from Friends where the user is either UserId1 or UserId2
                    string deleteFriendsQuery = "DELETE FROM Friends WHERE UserId1 = @UserId OR UserId2 = @UserId";
                    using (SqlCommand cmd = new SqlCommand(deleteFriendsQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@UserId", user.Id);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete from FriendRequests where the user is either the sender or receiver
                    string deleteFriendRequestsQuery = "DELETE FROM FriendRequests WHERE SenderUserId = @UserId OR ReceiverUserId = @UserId";
                    using (SqlCommand cmd = new SqlCommand(deleteFriendRequestsQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@UserId", user.Id);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete from Progress where the user is referenced
                    string deleteProgressQuery = "DELETE FROM Progress WHERE UserID = @UserId";
                    using (SqlCommand cmd = new SqlCommand(deleteProgressQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@UserId", user.Id);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete from Coaches or Participants if applicable
                    if (user.Type == User.UserType.Coach)
                    {
                        string deleteCoachQuery = "DELETE FROM Coaches WHERE UserId = @UserId";
                        using (SqlCommand cmd = new SqlCommand(deleteCoachQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@UserId", user.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else if (user.Type == User.UserType.Participant)
                    {
                        string deleteParticipantQuery = "DELETE FROM Participants WHERE UserId = @UserId";
                        using (SqlCommand cmd = new SqlCommand(deleteParticipantQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@UserId", user.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Finally, delete the user from the Users table
                    string deleteUserQuery = "DELETE FROM Users WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(deleteUserQuery, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Id", user.Id);
                        cmd.ExecuteNonQuery();
                    }

                    // Commit the transaction if all deletions were successful
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Roll back the transaction if any deletions failed
                    transaction.Rollback();
                    Console.WriteLine("An error occurred: " + ex.Message);
                    throw; // Propagate the exception to handle it outside this method
                }
            }
        }



        public List<Participant> GetParticipantsToEvaluate(Coach coach)
        {
            List<Participant> participants = new List<Participant>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    // SQL query to retrieve participants assigned to the coach
                    string query = @"
            SELECT u.Id, u.Username, u.Password, u.Email, u.FirstName, u.LastName, u.DateOfBirth
            FROM Users u
            INNER JOIN Participants p ON u.Id = p.UserId
            INNER JOIN CoachesParticipants cp ON p.UserId = cp.ParticipantId
            WHERE cp.CoachId = @CoachId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CoachId", coach.Id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create Participant instance from each record
                                Participant participant = new Participant(
                                    reader["Username"].ToString(),
                                    reader["Password"].ToString(), // Again, we should not handle passwords like this!
                                    reader["Email"].ToString(),
                                    reader["FirstName"].ToString(),
                                    reader["LastName"].ToString(),
                                    Convert.ToDateTime(reader["DateOfBirth"])
                                )
                                {
                                    Id = Convert.ToInt32(reader["Id"])
                                };
                                participants.Add(participant);
                            }
                        }
                    }
                    return participants;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                    throw;
                }
            }
        }

        public void UpdateParticipantsToEvaluate(Coach coach)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Remove all current participants for this coach
                    string deleteQuery = @"
                DELETE FROM CoachesParticipants 
                WHERE CoachId = @CoachId";

                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection, transaction))
                    {
                        deleteCommand.Parameters.AddWithValue("@CoachId", coach.Id);
                        deleteCommand.ExecuteNonQuery();
                    }

                    // Add all current participants from ParticipantsToEvaluate
                    string insertQuery = @"
                INSERT INTO CoachesParticipants (CoachId, ParticipantId)
                VALUES (@CoachId, @ParticipantId)";

                    foreach (Participant participant in coach.ParticipantsToEvaluate)
                    {
                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@CoachId", coach.Id);
                            insertCommand.Parameters.AddWithValue("@ParticipantId", participant.Id);
                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Roll back the transaction if anything fails
                    transaction.Rollback();
                    Console.WriteLine("An error occurred: " + ex.Message);
                    throw;
                }
            }
        }

    }
}
