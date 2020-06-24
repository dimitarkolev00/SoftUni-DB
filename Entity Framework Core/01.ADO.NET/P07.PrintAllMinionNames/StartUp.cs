using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace P07.PrintAllMinionNames
{
    public class StartUp
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";
        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            var minions = new List<string>();

            using (sqlConnection)
            {
                string cmdText = "SELECT Name FROM Minions";
                var command = new SqlCommand(cmdText, sqlConnection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string name = (string)reader["Name"];
                    minions.Add(name);
                }

                reader.Close();

                int count = minions.Count;
                int loopEnd = count / 2;

                for (int i = 0; i < loopEnd; i++)
                {
                    Console.WriteLine(minions[i]);
                    Console.WriteLine(minions[count - 1 - i]);
                }

                if (count % 2 == 1)
                {
                    Console.WriteLine(minions[count / 2]);
                }
            }
        }
    }
}
