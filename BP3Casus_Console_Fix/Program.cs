// Create two users, add each other as friends.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP3Casus_Console_Fix.Users;
using BP3Casus_Console_Fix.Users.Service;
using BP3Casus_Console_Fix.Event;
using BP3Casus_Console_Fix.Event.Service;
using BP3Casus_Console_Fix.Relations;
using BP3Casus_Console_Fix.Relations.Service;

namespace BP3Casus_Console_Fix
{
    class Program
    {
        static void Main(string[] args)
        {
            UserDataAccesLayer userDataAccesLayer = UserDataAccesLayer.Instance;
            EventDataAccesLayer eventDataAccesLayer = EventDataAccesLayer.Instance;

            // User Creation and retrieval tests
            /*                  Add users and retrieve Test, SUCCESS!
            // Define test data
            var testParticipant = new Participant("testusername", "testpassword", "testemail@example.com", "TestFirstName", "TestLastName", new DateTime(1990, 1, 1));
            var testCoach = new Coach("coachusername", "coachpassword", "coachemail@example.com", "CoachFirstName", "CoachLastName", new DateTime(1985, 1, 1), Coach.AreaOfExpertise.Fitness);

            // Attempt to add a Participant and a Coach
            try
            {
                userDataAccesLayer.AddUser(testParticipant);
                Console.WriteLine("Participant added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding participant: {ex.Message}");
            }

            try
            {
                userDataAccesLayer.AddUser(testCoach);
                Console.WriteLine("Coach added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding coach: {ex.Message}");
            }

            // Attempt to retrieve the added users using GetUserByCredentials
            try
            {
                var retrievedParticipant = userDataAccesLayer.GetUserByCredentials("testusername", "testpassword");
                if (retrievedParticipant != null)
                {
                    Console.WriteLine("Participant retrieved successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to retrieve participant.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving participant: {ex.Message}");
            }

            try
            {
                var retrievedCoach = userDataAccesLayer.GetUserByCredentials("coachusername", "coachpassword");
                if (retrievedCoach != null)
                {
                    Console.WriteLine("Coach retrieved successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to retrieve coach.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving coach: {ex.Message}");
            }
            */
            /*          Finding participants and coaches Test, SUCCESS!
            try
            { 
                Participant participant = userDataAccesLayer.GetParticipantById(1);
                Console.WriteLine("Participant retrieved successfully.");
                Console.WriteLine("Id: " + participant.Id);
                Console.WriteLine("Username: " + participant.Username);
                Console.WriteLine("Password: " + participant.Password);
                Console.WriteLine("Email: " + participant.Email);
                Console.WriteLine("First name: " + participant.FirstName);
                Console.WriteLine("Last name: " + participant.LastName);
                Console.WriteLine("Date of birth: " + participant.DateOfBirth);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving participant: {ex.Message}");
            }
            
            try
            {
                Coach coach = userDataAccesLayer.GetCoachById(2);
                Console.WriteLine("Coach retrieved successfully.");
                Console.WriteLine("Id: " + coach.Id);
                Console.WriteLine("Username: " + coach.Username);
                Console.WriteLine("Password: " + coach.Password);
                Console.WriteLine("Email: " + coach.Email);
                Console.WriteLine("First name: " + coach.FirstName);
                Console.WriteLine("Last name: " + coach.LastName);
                Console.WriteLine("Date of birth: " + coach.DateOfBirth);
                Console.WriteLine("Area of expertise: " + coach.Expertise);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving coach: {ex.Message}");
            }
            */
            // User modification tests
            /*          Update user Test, SUCCESS!
            Participant p = userDataAccesLayer.GetParticipantById(1);
            try
            {
                p.ChangeFirstName("Bart");
                Console.WriteLine("Participant's first name updated!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating participant's first name: {ex.Message}");
            }

            try
            {
                p.ChangeLastName("Lammertsma");
                Console.WriteLine("Participant's last name updated!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating participant's last name: {ex.Message}");
            }

            try
            {
                p.ChangeEmail("Bart.lammertsma@live.nl");
                Console.WriteLine("Participant's email updated!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating participant's email: {ex.Message}");
            }

            try
            {
                p.ChangeDateOfBirth(new DateTime(1998, 08, 18));
                Console.WriteLine("Participant's date of birth updated!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating participant's date of birth: {ex.Message}");
            }

            try
            {
                p.ChangeUsername("Bartje");
                Console.WriteLine("Participant's username updated!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating participant's username: {ex.Message}");
            }

            try
            {
                p.ChangePassword("Pass123");
                Console.WriteLine("Participant's password updated!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating participant's password: {ex.Message}");
            }
            */
            /*          Delete user Test, SUCCESS!
            Coach q = userDataAccesLayer.GetCoachById(2);

            try
            {
                q.DELETEACCOUNT();
                Console.WriteLine("Coach deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting participant: {ex.Message}");
            }
            */

            // FriendRequest tests
            /*      Send friendRequest Test, SUCCESS!
            Participant p1 = userDataAccesLayer.GetParticipantById(3);
            Participant p2 = userDataAccesLayer.GetParticipantById(4);

            try
            {
                p3.SendFriendRequest(p4);
                Console.WriteLine("Friend request sent from p3 to p4.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending friend request: {ex.Message}");
            }
            */
            /*      Accept friendRequest Test, SUCCESS!
            Participant p1 = userDataAccesLayer.GetParticipantById(3);
            Participant p2 = userDataAccesLayer.GetParticipantById(4);

            p2.GetFriendRequests();

            foreach (FriendRequest fr in p2.FriendRequests)
            {
                Console.WriteLine(fr.Sender.Username);
                fr.Accept();
            }
            */
            /*      Retrieving friends Test, SUCCESS!
            Participant p1 = userDataAccesLayer.GetParticipantById(3);
            p1.GetFriends();
            foreach (User friend in p1.Friends)
            {
                Console.WriteLine(friend.Username);
            }
             */
            /*      Removing a Friend Test, SUCCESS!
            Participant p1 = userDataAccesLayer.GetParticipantById(3);
            try
            {
                p1.GetFriends();
                foreach (User friend in p1.Friends)
                {
                    Console.WriteLine(friend.Username);
                    p1.RemoveFriend(friend);
                    Console.WriteLine("Friend removed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing friend: {ex.Message}");
            }
             */

            // EventType Creation and retrieval tests
            /*          Add EventType Test, SUCCESS!
            EventType eventType = new EventType("Hardlopen", "Hardlopen is een sport waarbij je rent.",15);

            try
            {
                eventDataAccesLayer.AddEventType(eventType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding event type: {ex.Message}");
            }
             */
            /* Finding EventTypes, SUCCESS!
            try
            {
                EventType eventType = eventDataAccesLayer.GetEventTypeById(1);
                Console.WriteLine("Event type retrieved successfully.");
                Console.WriteLine("Id: " + eventType.ID);
                Console.WriteLine("Name: " + eventType.Name);
                Console.WriteLine("Description: " + eventType.Description);
                Console.WriteLine("Experience: " + eventType.ExpPerParticipant);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving event type: {ex.Message}");
            }

            List<EventType> eventTypes = eventDataAccesLayer.GetAllEventTypes();
            foreach (EventType eventType in eventTypes)
            {
                Console.WriteLine(eventType.Name);
            }
             */
            // EventType modification tests
            /*          Update EventType Test, SUCCESS!
            try
            {
                EventType eventType = eventDataAccesLayer.GetEventTypeById(1);
                eventType.Rename("Zwemmen");
                Console.WriteLine("Event type name updated!");
                eventType.ChangeDescription("Zwemmen is een sport waarbij je zwemt.");
                Console.WriteLine("Event type description updated!");
                eventType.ChangeExpPerParticipant(20);
                Console.WriteLine("Event type experience updated!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding tag to event type: {ex.Message}");
            }
             */
            /*          Add tag to EventType Test, SUCCESS!
            try
            {
                EventType eventType = eventDataAccesLayer.GetEventTypeById(1);
                eventType.AddTag("Sport");
                Console.WriteLine("Tag added to event type!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding tag to event type: {ex.Message}");
            }
             */
            /* Retrieve tags from EventType Test, SUCCESS!
            try
            {
                EventType eventType = eventDataAccesLayer.GetEventTypeById(1);
                Console.WriteLine("Tags retrieved from event type:");
                foreach (string tag in eventType.Tags)
                {
                    Console.WriteLine(tag);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving tags from event type: {ex.Message}");
            }
             */
            /*          Remove tag from EventType Test, SUCCESS!
            try
            {
                EventType eventType = eventDataAccesLayer.GetEventTypeById(1);
                eventType.RemoveTag("Sport");
                Console.WriteLine("Tag removed from event type!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing tag from event type: {ex.Message}");
            }
             */

            // Event Creation and retrieval tests
            /*          Add Event Test, SUCCESS!
            Event.Event @event = new Event.Event("Zwemmen", new DateTime(2021, 12, 31), 10);
            try
            {
                eventDataAccesLayer.AddEvent(@event);
                Console.WriteLine("Event added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding event: {ex.Message}");
            }
             */
            /*          Finding Events, SUCCESS!
            try
            {
                Event.Event @event = eventDataAccesLayer.GetEventById(1);
                Console.WriteLine("Event retrieved successfully.");
                Console.WriteLine("Id: " + @event.Id);
                Console.WriteLine("Name: " + @event.Name);
                Console.WriteLine("Date: " + @event.Date);
                Console.WriteLine("Max participants: " + @event.MaxParticipants);
                Console.WriteLine("Is open: " + @event.IsOpen);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving event: {ex.Message}");
            }
             */
            // Event modification tests
            /*      Add EventType, Coach and change name Test, SUCCESS!
            Event.Event @event = eventDataAccesLayer.GetEventById(1);
            try
            {

                EventType eventType = eventDataAccesLayer.GetEventTypeById(1);
                @event.ChangeEventType(eventType);
                Console.WriteLine("Event type added to event!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding event type to event: {ex.Message}");
            }

            try
            {
                Coach coach = userDataAccesLayer.GetCoachById(9);
                @event.SetCoach(coach);
                Console.WriteLine("Coach added to event!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating event name: {ex.Message}");
            }

            try
            {
                @event.ChangeName("Toch lopen");
                Console.WriteLine("Event name updated!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating event name: {ex.Message}");
            }
             */
            /*      Add and remove participant Test, SUCCESS!
            try
            {
                Event.Event @event = eventDataAccesLayer.GetEventById(1);
                Participant participant = userDataAccesLayer.GetParticipantById(3);
                @event.AddParticipant(participant);
                Console.WriteLine("Participant added to event!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding participant to event: {ex.Message}");
            }
            try
            {
                Event.Event @event = eventDataAccesLayer.GetEventById(1);
                Console.WriteLine("Participants in event:");
                foreach (Participant p in @event.Participants)
                {
                    Console.WriteLine(p.Username);
                }
                Participant participant = userDataAccesLayer.GetParticipantById(3);
                @event.RemoveParticipant(participant);
                Console.WriteLine("Participant removed from event!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing participant from event: {ex.Message}");
            }
             */

            // Progress Creation and retrieval tests
            /*          Add Progress Test, SUCCESS!
            Progress progress = new Progress(1, 3);
            try
            {
                eventDataAccesLayer.AddProgress(progress);
                Console.WriteLine("Progress added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding progress: {ex.Message}");
            }
             */
            /*          Finding Progress, SUCCESS!
            try
            {
                Progress progress = eventDataAccesLayer.GetProgressById(1);
                Console.WriteLine("Progress retrieved successfully.");
                Console.WriteLine("Id: " + progress.ID);
                Console.WriteLine("Event type ID: " + progress.EventType);
                Console.WriteLine("Participant ID: " + progress.UserID);
                Console.WriteLine("Level: " + progress.Level);
                Console.WriteLine("Experience: " + progress.Experience);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving progress: {ex.Message}");
            }
            Participant p1 = userDataAccesLayer.GetParticipantById(3);
            try
            {
                p1.GetProgresses();
                foreach (Progress progress in p1.progresses)
                {
                    Console.WriteLine("Progress retrieved successfully.");
                    Console.WriteLine("Id: " + progress.ID);
                    Console.WriteLine("Event type ID: " + progress.EventType);
                    Console.WriteLine("Participant ID: " + progress.UserID);
                    Console.WriteLine("Level: " + progress.Level);
                    Console.WriteLine("Experience: " + progress.Experience);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending friend request: {ex.Message}");
            }
             */
            // Progress modification tests
            /*          Gain experience Test, SUCCESS!
            Participant p1 = userDataAccesLayer.GetParticipantById(3);
            try
            {
                p1.GetProgresses();
                foreach (Progress progress in p1.progresses)
                {
                    progress.GainExperience(10);
                    Console.WriteLine("Id: " + progress.ID);
                    Console.WriteLine("Event type ID: " + progress.EventType);
                    Console.WriteLine("Participant ID: " + progress.UserID);
                    Console.WriteLine("Level: " + progress.Level);
                    Console.WriteLine("Experience: " + progress.Experience);
                }
  
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error gaining experience: {ex.Message}");
            }
             */


            // give the user the option to log in or register
            Console.WriteLine("Welcome to the Fitness App!");
            Console.WriteLine();
            Console.WriteLine("1. Log in");
            Console.WriteLine("2. Register");
            Console.WriteLine();
            Console.Write("Select an option: ");

            string loginOrRegister = Console.ReadLine();

            User CurrentUser = null;

            switch (loginOrRegister)
            {
                case "1":
                    CurrentUser = Login();
                    break;
                case "2":
                    CurrentUser = Register();
                    break;
                default:
                    Console.WriteLine("Invalid input. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }

            User? Login()
            {

                User? loggedInUser = null;

                while (loggedInUser == null)
                {
                    loggedInUser = null;

                    while (loggedInUser == null)
                    {
                        Console.Clear();
                        Console.WriteLine("Login.");
                        Console.WriteLine();
                        Console.Write("Username: ");
                        string username = Console.ReadLine();

                        Console.Write("password: ");
                        string password = Console.ReadLine();
                        loggedInUser = userDataAccesLayer.GetUserByCredentials(username, password);

                        if (loggedInUser == null)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Press any key to try again.");
                            Console.ReadKey();
                        }
                    }
                }

                return loggedInUser;
            }
            User Register()
            {
                User? RegisteredUser = null;
                while (RegisteredUser == null)
                {
                    Console.Clear();
                    Console.WriteLine("Register.");
                    Console.WriteLine();
                    Console.Write("Username: ");
                    string username = Console.ReadLine();
                    Console.Write("Password: ");
                    string password = Console.ReadLine();
                    Console.Write("Email: ");
                    string email = Console.ReadLine();
                    Console.Write("First name: ");
                    string firstName = Console.ReadLine();
                    Console.Write("Last name: ");
                    string lastName = Console.ReadLine();
                    Console.Write("Date of birth (yyyy-MM-dd): ");
                    DateTime dateOfBirth = DateTime.Parse(Console.ReadLine());

                    RegisteredUser = userDataAccesLayer.CreateUser(username, password, email, firstName, lastName, dateOfBirth, User.UserType.Participant);
                }

                return RegisteredUser;
            }

            Console.Clear();
            Console.WriteLine("Logged in as: " + CurrentUser.Username);
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();

        }
    }
}