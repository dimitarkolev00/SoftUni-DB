using System;
using Microsoft.Data.SqlClient;

namespace P02.VillainNames
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using SqlConnection connection = new SqlConnection("Server=.;Database=MinionsDB;Integrated Security=true");
            connection.Open();

            string cmdText = "SELECT v.Name, COUNT(mv.MinionId) [Minions Count] " +
                             "FROM Villains v " +
                             "JOIN MinionsVillains mv ON mv.VillainId = v.Id " +
                             "GROUP BY v.Name " +
                             "HAVING COUNT(mv.MinionId) > 3 " +
                             "ORDER BY [Minions Count] DESC";

            SqlCommand command = new SqlCommand(cmdText, connection);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string villainName = (string)reader["Name"];
                int minionsCount = (int)reader["Minions Count"];

                Console.WriteLine($"{villainName} - {minionsCount}");
            }

        }
    }
}
