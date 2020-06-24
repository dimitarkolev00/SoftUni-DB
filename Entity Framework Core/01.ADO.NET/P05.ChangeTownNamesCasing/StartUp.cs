using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace P05.ChangeTownNamesCasing
{
    public class StartUp
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";
        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            string countryName = Console.ReadLine();
            int countryId = 0;
            var townsInCountry = new List<string>();

            using (sqlConnection)
            {
                string cmdText = @"SELECT Id FROM Countries WHERE Name = @countryName";
                var command = new SqlCommand(cmdText, sqlConnection);
                command.Parameters.AddWithValue("@countryName", countryName);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    countryId = (int)reader["Id"];
                }

                reader.Close();

                cmdText = @"UPDATE Towns SET Name = UPPER(Name) WHERE CountryCode = @countryId";
                command = new SqlCommand(cmdText, sqlConnection);
                command.Parameters.AddWithValue("@countryId", countryId);
                int rowsAffected = command.ExecuteNonQuery();

                cmdText = @"SELECT Name FROM Towns WHERE CountryCode = @countryId";
                command = new SqlCommand(cmdText, sqlConnection);
                command.Parameters.AddWithValue("@countryId", countryId);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string town = (string)reader["Name"];
                    townsInCountry.Add(town);
                }

                reader.Close();

                if (townsInCountry.Count > 0)
                {
                    Console.WriteLine($"{rowsAffected} town names were affected.");
                    Console.WriteLine($"[{String.Join(", ", townsInCountry)}]");
                }
                else
                {
                    Console.WriteLine("No town names were affected.");
                }
            }
        }
    }
}
