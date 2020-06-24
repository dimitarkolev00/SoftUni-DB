using System;
using System.Linq;
using System.Threading;
using Microsoft.Data.SqlClient;

namespace P08.IncreaseMinionAge
{
    public class StartUp
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";
        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            int[] ids = Console.ReadLine().Split().Select(int.Parse).ToArray();


            using (sqlConnection)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    string cmdText = $"SELECT * FROM Minions WHERE Id = @id";
                    var command = new SqlCommand(cmdText, sqlConnection);
                    command.Parameters.AddWithValue("@id", ids[i]);
                    var reader = command.ExecuteReader();
                    reader.Read();
                    string name = Convert.ToString(reader["Name"]);
                    reader.Close();

                    var cultureInfo = Thread.CurrentThread.CurrentCulture;
                    var textInfo = cultureInfo.TextInfo;
                    string convertedName = textInfo.ToTitleCase(name);

                    var updateCmd = $"UPDATE Minions SET Name = '{convertedName}', Age += 1 WHERE Id = {ids[i]}";
                    var updateCommand = new SqlCommand(updateCmd, sqlConnection);
                    updateCommand.ExecuteNonQuery();
                }

                string printQuery = "SELECT Name, Age FROM Minions";
                var printCommand = new SqlCommand(printQuery, sqlConnection);
                var printer = printCommand.ExecuteReader();
                while (printer.Read())
                {
                    string minionName = (string)printer["Name"];
                    int age = (int)printer["Age"];

                    Console.WriteLine($"{minionName} {age}");
                }
            }
        }
    }
}
