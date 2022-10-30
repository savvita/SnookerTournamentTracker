using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TournamentLibrary;

namespace SnookerTournamentTracker.Model
{
    internal static class ConnectionClientModel
    {
        public static bool SignIn(PersonModel user, out string error)
        {
            // TODO - Add request to the server
            error = "Error";
            if (!File.Exists("users.json"))
            {
                return false;
            }

            string json = File.ReadAllText("users.json");

            List<PersonModel>? players = JsonSerializer.Deserialize<List<PersonModel>>(json);
            if(players == null)
            {
                return false;
            }

            return players.FindIndex(player => {
                if(player == null || player.Login == null)
                {
                    return false;
                }
                return player.Login.Equals(user.Login);
            }) != -1;
        }

        public static bool SignUp(PersonModel user, out string error)
        {
            // TODO - Add request to the server
            error = "Error";
            if (!File.Exists("users.json"))
            {
                File.Create("users.json");
            }

            string json = File.ReadAllText("users.json");


            List<PersonModel>? players = new List<PersonModel>();

            try
            {
                players = JsonSerializer.Deserialize<List<PersonModel>>(json);
            }
            catch
            {
            }

            if(players == null)
            {
                players = new List<PersonModel>();
            }

            players.Add(user);

            json = JsonSerializer.Serialize<List<PersonModel>>(players);
            File.WriteAllText("users.json", json);

            //TODO - Add checks
            return true;
        }

        private static string? SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public static List<string> GetAllPlaces()
        {
            //TODO - Load from DB
            List<string> places = new List<string>();

            places.Add("Winner");
            places.Add("Round Up");
            places.Add("Semi final");
            places.Add("Quarter final");
            return places;
        }

        public static List<PersonModel> GetAllPlayers()
        {
            //TODO - Load from DB
            List<PersonModel> players = new List<PersonModel>();

            players.Add(new PersonModel()
            {
                FirstName = "John",
                SecondName = "J.",
                LastName = "Smith",
                EmailAddress = "smith@mail.com"
            });

            players.Add(new PersonModel()
            {
                FirstName = "Ivan",
                SecondName = "Ivanovich",
                LastName = "Petrov",
                EmailAddress = "petrov@mail.com"
            });

            players.Add(new PersonModel()
            {
                FirstName = "John",
                LastName = "Doe"
            });
            return players;
        }

        public static bool CreateTournament(TournamentModel tournament)
        {
            //TODO add creation and creation rounds
            return true;
        }
    }
}
