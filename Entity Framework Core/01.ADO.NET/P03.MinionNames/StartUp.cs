using System;
using System.Text;
using Microsoft.Data.SqlClient;

namespace P03.MinionNames
{
    public class Program
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";
        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            int villainId = int.Parse(Console.ReadLine());

            string result = GetMinionsInfoAboutVillains(sqlConnection, villainId);

            Console.WriteLine(result);
        }

        private static string GetMinionsInfoAboutVillains(SqlConnection sqlConnection, int villainId)
        {
            StringBuilder sb = new StringBuilder();

            string getVillainNameQueryText = @"SELECT [Name] FROM Villains
                                              WHERE Id = @villainId";
            using SqlCommand getVillainNameCmd = new SqlCommand(getVillainNameQueryText, sqlConnection);
            getVillainNameCmd.Parameters.AddWithValue("@villainId", villainId);

            string villainName = getVillainNameCmd.ExecuteScalar()?.ToString();

            if (villainName == null)
            {
                sb.AppendLine($"No villain with ID {villainId} exists in the database.");
            }
            else
            {
                sb.AppendLine($"Villain: {villainName}");

                string getMinionsInfoQueryText = @"SELECT m.[Name], m.Age
                                                   FROM Villains AS v
                                                   LEFT OUTER JOIN MinionsVillains AS mv ON v.Id = mv.VillainId
                                                   LEFT OUTER JOIN Minions AS m ON mv.MinionId = m.Id
                                                   WHERE v.[Name] = @villainName
                                                   ORDER BY m.[Name]";

                SqlCommand getMinionsInfoCmd = new SqlCommand(getMinionsInfoQueryText, sqlConnection);
                getMinionsInfoCmd.Parameters.AddWithValue("@villainName", villainName);

                using SqlDataReader reader = getMinionsInfoCmd.ExecuteReader();

                int rowNum = 1;

                while (reader.Read())
                {
                    string minionName = reader["Name"]?.ToString();
                    string minionAge = reader["Age"]?.ToString();

                    if (minionAge == "" && minionName == "")
                    {
                        sb.AppendLine("(no minions)");
                        break;
                    }

                    sb.AppendLine($"{rowNum}. {minionName} {minionAge}");

                    rowNum++;
                }
            }
            return sb.ToString().TrimEnd();
        }
    }
}
