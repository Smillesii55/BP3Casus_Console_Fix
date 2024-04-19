using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BP3Casus_Console_Fix.Event;
using BP3Casus_Console_Fix.Users;
using BP3Casus_Console_Fix.Users.Service;


namespace BP3Casus_Console_Fix.Event.Service
{
    public class EventDataAccesLayer
    {
        private string connectionString = "Server=.;Database=BP3Casus_Fix;Trusted_Connection=True;";
        UserDataAccesLayer UserDataAccesLayer = UserDataAccesLayer.Instance;
        private EventDataAccesLayer()
        {
        }
        private static EventDataAccesLayer? instance = null;
        public static EventDataAccesLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventDataAccesLayer();
                }
                return instance;
            }
        }


        // Event
        public List<Event> GetAllEvents()
        {
            List<Event> events = new List<Event>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to fetch all events and their associated CoachId and EventTypeId
                string query = @"
            SELECT Id, Name, Date, MaxParticipants, IsOpen, CoachId, EventTypeId
            FROM Events";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Event eventItem = new Event(
                                reader["Name"].ToString(),
                                reader.GetDateTime(reader.GetOrdinal("Date")),
                                reader.GetInt32(reader.GetOrdinal("MaxParticipants"))
                            )
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                IsOpen = reader.GetBoolean(reader.GetOrdinal("IsOpen")),
                                CoachId = reader.IsDBNull(reader.GetOrdinal("CoachId")) ? null : reader.GetInt32(reader.GetOrdinal("CoachId")),
                                EventTypeId = reader.IsDBNull(reader.GetOrdinal("EventTypeId")) ? null : reader.GetInt32(reader.GetOrdinal("EventTypeId"))
                            };
                            events.Add(eventItem);
                        }
                    }
                }

                // Populate the Participants for each event
                foreach (Event eventItem in events)
                {
                    string participantQuery = @"
                SELECT ParticipantId
                FROM ParticipantsEvents
                WHERE EventId = @EventId";

                    using (SqlCommand participantCommand = new SqlCommand(participantQuery, connection))
                    {
                        participantCommand.Parameters.AddWithValue("@EventId", eventItem.Id);

                        using (SqlDataReader participantReader = participantCommand.ExecuteReader())
                        {
                            while (participantReader.Read())
                            {
                                int participantId = participantReader.GetInt32(participantReader.GetOrdinal("ParticipantId"));
                                Participant participant = UserDataAccesLayer.GetParticipantById(participantId);
                                if (participant != null)
                                {
                                    eventItem.Participants.Add(participant);
                                }
                            }
                        }
                    }
                }
            }
            return events;
        }

        public void AddEvent(Event @event)
        {
            if (@event == null)
            {
                throw new ArgumentException("Event cannot be null.");
            }

            if (string.IsNullOrEmpty(@event.Name))
            {
                throw new ArgumentException("Event must have a name.");
            }

            if (@event.Date == DateTime.MinValue)
            {
                throw new ArgumentException("Event must have a valid date.");
            }

            if (@event.EventTypeId == null)
            {
                throw new ArgumentException("Event must have a valid EventType.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert into Events table
                    string insertEventQuery = @"
                INSERT INTO Events (Name, Date, MaxParticipants, IsOpen, CoachId, EventTypeId) 
                VALUES (@Name, @Date, @MaxParticipants, @IsOpen, @CoachId, @EventTypeId);
                SELECT SCOPE_IDENTITY();"; // This returns the ID of the newly inserted event

                    int eventId;

                    using (SqlCommand eventCommand = new SqlCommand(insertEventQuery, connection, transaction))
                    {
                        eventCommand.Parameters.AddWithValue("@Name", @event.Name);
                        eventCommand.Parameters.AddWithValue("@Date", @event.Date);
                        eventCommand.Parameters.AddWithValue("@MaxParticipants", @event.MaxParticipants);
                        eventCommand.Parameters.AddWithValue("@IsOpen", @event.IsOpen);
                        eventCommand.Parameters.AddWithValue("@CoachId", @event.CoachId.HasValue ? (object)@event.CoachId : DBNull.Value);
                        eventCommand.Parameters.AddWithValue("@EventTypeId", @event.EventTypeId);

                        eventId = Convert.ToInt32(eventCommand.ExecuteScalar());
                    }

                    // Insert into ParticipantsEvents if there are any participants
                    if (@event.Participants != null && @event.Participants.Count > 0)
                    {
                        string insertParticipantEventQuery = @"
                    INSERT INTO ParticipantsEvents (EventId, ParticipantId)
                    VALUES (@EventId, @ParticipantId)";

                        foreach (Participant participant in @event.Participants)
                        {
                            using (SqlCommand participantEventCommand = new SqlCommand(insertParticipantEventQuery, connection, transaction))
                            {
                                participantEventCommand.Parameters.AddWithValue("@EventId", eventId);
                                participantEventCommand.Parameters.AddWithValue("@ParticipantId", participant.Id);
                                participantEventCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // If something goes wrong, rollback the transaction
                    transaction.Rollback();
                    Console.WriteLine("An error occurred: " + ex.Message);
                    throw;
                }
            }
        }

        public void UpdateEvent(Event @event)
        {
            if (@event == null)
            {
                throw new ArgumentException("Event cannot be null.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Adding EventTypeId and CoachId to the update statement.
                using (SqlCommand command = new SqlCommand("UPDATE Events SET Name = @Name, Date = @Date, MaxParticipants = @MaxParticipants, IsOpen = @IsOpen, CoachId = @CoachId, EventTypeId = @EventTypeId WHERE Id = @EventId", connection))
                {
                    command.Parameters.AddWithValue("@Name", @event.Name);
                    command.Parameters.AddWithValue("@Date", @event.Date);
                    command.Parameters.AddWithValue("@MaxParticipants", @event.MaxParticipants);
                    command.Parameters.AddWithValue("@IsOpen", @event.IsOpen);
                    command.Parameters.AddWithValue("@CoachId", @event.CoachId.HasValue ? (object)@event.CoachId : DBNull.Value); // Handle nullable CoachId
                    command.Parameters.AddWithValue("@EventTypeId", @event.EventTypeId.HasValue ? (object)@event.EventTypeId : DBNull.Value); // Handle nullable EventTypeId
                    command.Parameters.AddWithValue("@EventId", @event.Id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException("No event was updated, check the event ID.");
                    }
                }
            }
        }

        public void UpdateEventParticipants(Event @event)
        {
            if (@event == null)
            {
                throw new ArgumentException("Event cannot be null.");
            }

            if (@event.Id <= 0)
            {
                throw new ArgumentException("Event must have a valid ID.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // First, delete all current participants for this event
                    string deleteQuery = "DELETE FROM ParticipantsEvents WHERE EventId = @EventId";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection, transaction))
                    {
                        deleteCommand.Parameters.AddWithValue("@EventId", @event.Id);
                        deleteCommand.ExecuteNonQuery();
                    }

                    // Then, add all current participants from the event's participant list
                    string insertQuery = "INSERT INTO ParticipantsEvents (EventId, ParticipantId) VALUES (@EventId, @ParticipantId)";
                    foreach (Participant participant in @event.Participants)
                    {
                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@EventId", @event.Id);
                            insertCommand.Parameters.AddWithValue("@ParticipantId", participant.Id);
                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // If something goes wrong, roll back the transaction
                    transaction.Rollback();
                    Console.WriteLine("An error occurred: " + ex.Message);
                    throw;
                }
            }
        }

        public void RemoveEvent(Event @event)
        {
            if (@event == null)
            {
                throw new ArgumentException("Event cannot be null.");
            }

            if (@event.Id <= 0)
            {
                throw new ArgumentException("Event must have a valid ID.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // First, delete any associated records in the ParticipantsEvents table
                    string deleteParticipantsQuery = "DELETE FROM ParticipantsEvents WHERE EventId = @EventId";
                    using (SqlCommand deleteParticipantsCommand = new SqlCommand(deleteParticipantsQuery, connection, transaction))
                    {
                        deleteParticipantsCommand.Parameters.AddWithValue("@EventId", @event.Id);
                        deleteParticipantsCommand.ExecuteNonQuery();
                    }

                    // Then, delete the event record from the Events table
                    string deleteEventQuery = "DELETE FROM Events WHERE Id = @EventId";
                    using (SqlCommand deleteEventCommand = new SqlCommand(deleteEventQuery, connection, transaction))
                    {
                        deleteEventCommand.Parameters.AddWithValue("@EventId", @event.Id);
                        int rowsAffected = deleteEventCommand.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new InvalidOperationException("No event found with the provided ID, or it could not be deleted.");
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // If something goes wrong, roll back the transaction
                    transaction.Rollback();
                    Console.WriteLine("An error occurred: " + ex.Message);
                    throw; // Rethrow the exception to handle it further up the call stack
                }
            }
        }

        public Event? GetEventById(int eventId)
        {
            if (eventId <= 0)
            {
                throw new ArgumentException("Invalid Event ID provided.");
            }

            Event? eventItem = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to fetch the event details
                string eventQuery = @"
            SELECT Id, Name, Date, MaxParticipants, IsOpen, CoachId
            FROM Events
            WHERE Id = @EventId";

                using (SqlCommand eventCommand = new SqlCommand(eventQuery, connection))
                {
                    eventCommand.Parameters.AddWithValue("@EventId", eventId);

                    using (SqlDataReader reader = eventCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            eventItem = new Event(
                                reader["Name"].ToString(),
                                reader.GetDateTime(reader.GetOrdinal("Date")),
                                reader.GetInt32(reader.GetOrdinal("MaxParticipants"))
                            )
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                IsOpen = reader.GetBoolean(reader.GetOrdinal("IsOpen")),
                                CoachId = reader.IsDBNull(reader.GetOrdinal("CoachId")) ? null : reader.GetInt32(reader.GetOrdinal("CoachId"))
                            };
                        }
                    }
                }

                // Fetch associated participants if the event was found
                if (eventItem != null)
                {
                    eventItem.Participants = new List<Participant>();

                    string participantQuery = @"
                SELECT p.UserId
                FROM ParticipantsEvents pe
                JOIN Participants p ON pe.ParticipantId = p.UserId
                WHERE pe.EventId = @EventId";

                    using (SqlCommand participantCommand = new SqlCommand(participantQuery, connection))
                    {
                        participantCommand.Parameters.AddWithValue("@EventId", eventId);

                        using (SqlDataReader participantReader = participantCommand.ExecuteReader())
                        {
                            while (participantReader.Read())
                            {
                                Participant participant = UserDataAccesLayer.GetParticipantById(participantReader.GetInt32(participantReader.GetOrdinal("UserId")));
                                eventItem.Participants.Add(participant);
                            }
                        }
                    }
                }
            }

            return eventItem;
        }



        // EventType
        public List<EventType>? GetAllEventTypes()
        {
            List<EventType> eventTypes = new List<EventType>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to fetch all event types and their tags
                string query = @"
            SELECT et.ID, et.Name, et.Description, et.ExpPerParticipant, t.ID as TagID, t.TagName
            FROM EventTypes et
            LEFT JOIN EventTypeTags ett ON et.ID = ett.EventTypeId
            LEFT JOIN Tags t ON ett.TagId = t.ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var eventTypeDict = new Dictionary<int, EventType>();

                        while (reader.Read())
                        {
                            int eventTypeId = reader.GetInt32(reader.GetOrdinal("ID"));
                            EventType eventType;
                            if (!eventTypeDict.TryGetValue(eventTypeId, out eventType))
                            {
                                eventType = new EventType(
                                    reader["Name"].ToString(),
                                    reader["Description"].ToString(),
                                    Convert.ToDouble(reader["ExpPerParticipant"])
                                )
                                {
                                    ID = eventTypeId
                                };
                                eventTypeDict.Add(eventTypeId, eventType);
                                eventTypes.Add(eventType);
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("TagID")))
                            {
                                Tag tag = new Tag(
                                    reader.GetInt32(reader.GetOrdinal("TagID")),
                                    reader["TagName"].ToString()
                                );
                                eventType.Tags.Add(tag.TagName);
                            }
                        }
                    }
                }
            }
            return eventTypes;
        }

        public void AddEventType(EventType eventType)
        {
            if (eventType == null)
            {
                throw new ArgumentException("Provided eventType is null");
            }

            if (string.IsNullOrEmpty(eventType.Name))
            {
                throw new ArgumentException("EventType must have a name");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert into EventTypes table
                    string insertEventTypeQuery = @"
                INSERT INTO EventTypes (Name, Description, ExpPerParticipant) 
                VALUES (@Name, @Description, @ExpPerParticipant);
                SELECT SCOPE_IDENTITY();";

                    int eventTypeId;

                    using (SqlCommand command = new SqlCommand(insertEventTypeQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Name", eventType.Name);
                        command.Parameters.AddWithValue("@Description", eventType.Description ?? (object)DBNull.Value);  // Handle null Description
                        command.Parameters.AddWithValue("@ExpPerParticipant", eventType.ExpPerParticipant);

                        // Execute the query and get the new EventType ID
                        eventTypeId = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // Insert any associated tags into EventTypeTags
                    if (eventType.Tags != null && eventType.Tags.Any())
                    {
                        string insertTagQuery = @"
                    INSERT INTO EventTypeTags (EventTypeId, TagId) 
                    VALUES (@EventTypeId, @TagId)";

                        foreach (string tag in eventType.Tags)
                        {
                            int tagId = GetTagIdByName(tag);
                            using (SqlCommand tagCommand = new SqlCommand(insertTagQuery, connection, transaction))
                            {
                                tagCommand.Parameters.AddWithValue("@EventTypeId", eventTypeId);
                                tagCommand.Parameters.AddWithValue("@TagId", tagId);
                                tagCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch
                {
                    // If anything goes wrong, roll back the transaction
                    transaction.Rollback();
                    throw;
                }
            }
        }


        public void UpdateEventType(EventType eventType)
        {
            if (eventType == null)
            {
                throw new ArgumentException("EventType cannot be null.");
            }

            if (eventType.ID <= 0)
            {
                throw new ArgumentException("EventType must have a valid ID.");
            }

            if (string.IsNullOrEmpty(eventType.Name))
            {
                throw new ArgumentException("EventType must have a name.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Update the EventType table with new details
                    string updateEventTypeQuery = @"
                UPDATE EventTypes 
                SET Name = @Name, 
                    Description = @Description, 
                    ExpPerParticipant = @ExpPerParticipant
                WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(updateEventTypeQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Name", eventType.Name);
                        command.Parameters.AddWithValue("@Description", eventType.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ExpPerParticipant", eventType.ExpPerParticipant);
                        command.Parameters.AddWithValue("@ID", eventType.ID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new InvalidOperationException("EventType not found or no data updated.");
                        }
                    }

                    // Assuming the Tags are managed with another method such as UpdateTags which we will need to implement
                    UpdateTags(eventType, transaction);

                    // Commit transaction
                    transaction.Commit();
                }
                catch
                {
                    // Roll back the transaction if anything goes wrong
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void RemoveEventType(EventType eventType)
        {
            if (eventType == null)
            {
                throw new ArgumentException("EventType cannot be null.");
            }

            if (eventType.ID <= 0)
            {
                throw new ArgumentException("EventType must have a valid ID.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Call to remove the associated tags from the EventType
                    RemoveTagsFromEvent(eventType, transaction);

                    // Then, delete the EventType
                    string deleteEventTypeQuery = "DELETE FROM EventTypes WHERE ID = @ID";
                    using (SqlCommand deleteEventTypeCommand = new SqlCommand(deleteEventTypeQuery, connection, transaction))
                    {
                        deleteEventTypeCommand.Parameters.AddWithValue("@ID", eventType.ID);
                        int rowsAffected = deleteEventTypeCommand.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new InvalidOperationException("EventType not found or could not be deleted.");
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch
                {
                    // If anything goes wrong, roll back the transaction
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public EventType? GetEventTypeById(int eventTypeId)
        {
            if (eventTypeId <= 0)
            {
                throw new ArgumentException("Invalid EventType ID provided.");
            }

            EventType? eventType = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to fetch the event type and its associated tags
                string eventTypeQuery = @"
            SELECT ID, Name, Description, ExpPerParticipant
            FROM EventTypes
            WHERE ID = @EventTypeId";

                string eventTagQuery = @"
            SELECT t.TagName
            FROM Tags t
            JOIN EventTypeTags ett ON t.ID = ett.TagId
            WHERE ett.EventTypeId = @EventTypeId";

                using (SqlCommand eventTypeCommand = new SqlCommand(eventTypeQuery, connection))
                {
                    eventTypeCommand.Parameters.AddWithValue("@EventTypeId", eventTypeId);

                    using (SqlDataReader reader = eventTypeCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            eventType = new EventType(
                                reader["Name"].ToString(),
                                reader["Description"].ToString(),
                                Convert.ToDouble(reader["ExpPerParticipant"])
                            )
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                Tags = new List<string>()
                            };
                        }
                    }
                }

                // Only fetch tags if an EventType was found
                if (eventType != null)
                {
                    using (SqlCommand eventTagCommand = new SqlCommand(eventTagQuery, connection))
                    {
                        eventTagCommand.Parameters.AddWithValue("@EventTypeId", eventTypeId);

                        using (SqlDataReader tagReader = eventTagCommand.ExecuteReader())
                        {
                            while (tagReader.Read())
                            {
                                eventType.Tags.Add(tagReader["TagName"].ToString());
                            }
                        }
                    }
                }
            }

            return eventType;
        }



        // Progress
        public List<Progress> GetProgresses(Participant participant)
        {
            if (participant == null)
            {
                throw new ArgumentException("Participant cannot be null.");
            }

            List<Progress> progresses = new List<Progress>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query to select all progress entries for a given participant
                string query = @"
            SELECT ID, EventTypeID, UserID, Level, Experience
            FROM Progress
            WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", participant.Id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Progress progress = new Progress(
                                reader.GetInt32(reader.GetOrdinal("EventTypeID")),
                                reader.GetInt32(reader.GetOrdinal("UserID"))
                            )
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                Level = reader.GetInt32(reader.GetOrdinal("Level")),
                                Experience = reader.GetDouble(reader.GetOrdinal("Experience"))
                            };
                            progresses.Add(progress);
                        }
                    }
                }
            }

            return progresses;
        }

        public void AddProgress(Progress progress)
        {
            if (progress == null)
            {
                throw new ArgumentException("Progress cannot be null.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = @"
            INSERT INTO Progress (EventTypeID, UserID, Level, Experience)
            VALUES (@EventTypeID, @UserID, @Level, @Experience);
            SELECT SCOPE_IDENTITY();";  // To get the ID of the newly inserted progress

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@EventTypeID", progress.EventTypeID);
                    command.Parameters.AddWithValue("@UserID", progress.UserID);
                    command.Parameters.AddWithValue("@Level", progress.Level);
                    command.Parameters.AddWithValue("@Experience", progress.Experience);

                    // Execute the query and get the ID of the newly inserted progress
                    int newProgressId = Convert.ToInt32(command.ExecuteScalar());
                    progress.ID = newProgressId;  // Set the ID of the progress object
                }
            }
        }


        public void UpdateProgress(Progress progress)
        {
        }

        public void RemoveProgress(Progress progress)
        {
        }

        public Progress GetProgressById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid ID provided.");
            }

            Progress progress = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
            SELECT ID, EventTypeID, UserID, Level, Experience
            FROM Progress
            WHERE ID = @ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            progress = new Progress(
                                reader.GetInt32(reader.GetOrdinal("EventTypeID")),
                                reader.GetInt32(reader.GetOrdinal("UserID"))
                            )
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                Level = reader.GetInt32(reader.GetOrdinal("Level")),
                                Experience = reader.GetDouble(reader.GetOrdinal("Experience"))
                            };
                        }
                    }
                }
            }

            return progress;
        }


        // Tag
        public int GetTagIdByName(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                throw new ArgumentException("Tag name must be provided");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to fetch the ID of the tag by name
                string query = "SELECT ID FROM Tags WHERE TagName = @TagName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TagName", tagName);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return (int)result;
                    }
                    else
                    {
                        // Handle the case where the tag does not exist. 
                        // Options: throw an exception, create a new tag, or return a default value like 0.
                        throw new InvalidOperationException("Tag not found");
                    }
                }
            }
        }

        private void UpdateTags(EventType eventType, SqlTransaction transaction)
        {
            // Delete existing tags for this EventType
            string deleteTagsQuery = "DELETE FROM EventTypeTags WHERE EventTypeId = @EventTypeId";

            using (SqlCommand deleteCommand = new SqlCommand(deleteTagsQuery, transaction.Connection, transaction))
            {
                deleteCommand.Parameters.AddWithValue("@EventTypeId", eventType.ID);
                deleteCommand.ExecuteNonQuery();
            }

            // Insert new tags for this EventType
            string insertTagQuery = @"
        INSERT INTO EventTypeTags (EventTypeId, TagId) 
        VALUES (@EventTypeId, @TagId)";

            foreach (string tagName in eventType.Tags)
            {
                // Assuming a method exists to ensure a tag exists and get its ID: EnsureTagExistsAndGetId
                int tagId = EnsureTagExistsAndGetId(tagName, transaction);

                using (SqlCommand insertCommand = new SqlCommand(insertTagQuery, transaction.Connection, transaction))
                {
                    insertCommand.Parameters.AddWithValue("@EventTypeId", eventType.ID);
                    insertCommand.Parameters.AddWithValue("@TagId", tagId);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        public int EnsureTagExistsAndGetId(string tagName, SqlTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                throw new ArgumentException("Tag name cannot be null or whitespace.");
            }

            // Attempt to find the tag ID from the database
            string selectQuery = "SELECT ID FROM Tags WHERE TagName = @TagName";
            using (SqlCommand selectCommand = new SqlCommand(selectQuery, transaction.Connection, transaction))
            {
                selectCommand.Parameters.AddWithValue("@TagName", tagName);
                object result = selectCommand.ExecuteScalar();
                if (result != null)
                {
                    return (int)result;
                }
            }

            // If not found, insert the new tag
            string insertQuery = "INSERT INTO Tags (TagName) VALUES (@TagName); SELECT SCOPE_IDENTITY();";
            using (SqlCommand insertCommand = new SqlCommand(insertQuery, transaction.Connection, transaction))
            {
                insertCommand.Parameters.AddWithValue("@TagName", tagName);
                int newTagId = Convert.ToInt32(insertCommand.ExecuteScalar());
                return newTagId;
            }
        }


        private void RemoveTagsFromEvent(EventType eventType, SqlTransaction transaction)
        {
            // Delete entries from the EventTypeTags junction table for this EventType
            string deleteTagsQuery = "DELETE FROM EventTypeTags WHERE EventTypeId = @EventTypeId";
            using (SqlCommand deleteTagsCommand = new SqlCommand(deleteTagsQuery, transaction.Connection, transaction))
            {
                deleteTagsCommand.Parameters.AddWithValue("@EventTypeId", eventType.ID);
                deleteTagsCommand.ExecuteNonQuery();
            }
        }

    }
}
