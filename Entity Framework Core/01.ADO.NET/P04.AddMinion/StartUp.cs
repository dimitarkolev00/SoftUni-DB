using System;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;

namespace P04.AddMinion
{
    public class StartUp
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";
        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            string[] minionsInput = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
            string[] minionsInfo = minionsInput[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();


            string[] villainInput = Console.ReadLine()
                .Split(": ", StringSplitOptions.RemoveEmptyEntries)
                .ToArray();
            string[] villainInfo = villainInput[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            string result = AddMinionsToDatabase(sqlConnection, minionsInfo, villainInfo);

            Console.WriteLine(result);
        }

        private static string AddMinionsToDatabase(SqlConnection sqlConnection, string[] minionsInfo, string[] villainInfo)
        {
            StringBuilder output = new StringBuilder();

            string minionName = minionsInfo[0];
            string minionAge = minionsInfo[1];
            string minionTown = minionsInfo[2];
            
            string villainName = villainInfo[0];

            string townId = EnsureTownExists(sqlConnection, minionTown,output);
            string villainId = EnsureVillainExists(sqlConnection, villainName, output);

            string insertMinionQueryText = @"INSERT INTO Minions([Name],Age,TownId)
                                             VALUES (@minionName,@minionAge,@townId)";
            using SqlCommand insertMinionCmd = new SqlCommand(insertMinionQueryText,sqlConnection);
            insertMinionCmd.Parameters.AddWithValue("@minionName", minionName);
            insertMinionCmd.Parameters.AddWithValue("@minionAge", minionAge);
            insertMinionCmd.Parameters.AddWithValue("@townId", townId);

            insertMinionCmd.ExecuteNonQuery();

            string getMinionIdQueryText = @"SELECT Id FROM Minions
                                            WHERE [Name] = @minionName";
            using SqlCommand getMinionIdCmd = new SqlCommand(getMinionIdQueryText,sqlConnection);
            getMinionIdCmd.Parameters.AddWithValue("@minionName", minionName);

            string minionId = getMinionIdCmd.ExecuteScalar().ToString();

            string insertIntoMappingQueryText = @"INSERT INTO MinionsVillains(MinionId,VillainId)
                                                  VALUES(@minionId,@villainId)";
            using SqlCommand insertIntoMappingCmd = new SqlCommand(insertIntoMappingQueryText,sqlConnection);
            insertIntoMappingCmd.Parameters.AddWithValue("minionId", minionId);
            insertIntoMappingCmd.Parameters.AddWithValue("@villainId", villainId);

            insertIntoMappingCmd.ExecuteNonQuery();

            output.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");
            return output.ToString().TrimEnd();
        }

        private static string EnsureVillainExists(SqlConnection sqlConnection, string villainName, StringBuilder output)
        {
            string getVillainIdQueryText = @"SELECT Id FROM Villains
                                             WHERE [Name]=@villainName";
            using SqlCommand getVillainIdCmd = new SqlCommand(getVillainIdQueryText,sqlConnection);
            getVillainIdCmd.Parameters.AddWithValue("@villainName", villainName);

            string villainId = getVillainIdCmd.ExecuteScalar()?.ToString();

            if (villainId==null)
            {
                string getFactorIdQueryText = @"SELECT Id FROM EvilnessFactors
                                                WHERE [Name] = 'Evil'";
                using SqlCommand getFactorIdCmd = new SqlCommand(getFactorIdQueryText,sqlConnection);
                string factorId = getFactorIdCmd.ExecuteScalar()?.ToString();


                string insertVillainQueryText = @"INSERT INTO Villains([Name],EvilnessFactorId)
                                                  VALUES(@villainName,@factorId)";
                using SqlCommand insertVillainCmd = new SqlCommand(insertVillainQueryText,sqlConnection);
                insertVillainCmd.Parameters.AddWithValue("@villainName", villainName);
                insertVillainCmd.Parameters.AddWithValue("@factorId", factorId);
                insertVillainCmd.ExecuteNonQuery();

                villainId = getVillainIdCmd.ExecuteScalar().ToString();

                output.AppendLine($"Villain {villainName} was added to the database.");
            }

            return villainId;
        }

        private static string EnsureTownExists(SqlConnection sqlConnection, string minionTown,StringBuilder output)
        {
            string getTownIdQueryText = @"SELECT Id FROM Towns
                                          WHERE [Name] = @townName";
            using SqlCommand getTownIdCmd = new SqlCommand(getTownIdQueryText,sqlConnection);
            getTownIdCmd.Parameters.AddWithValue("@townName", minionTown);

            string townId = getTownIdCmd.ExecuteScalar()?.ToString();

            if (townId==null)
            {
                string insertTownQueryText = @"INSERT INTO Towns([Name],CountryCode)
                                               VALUES(@townName,1)";
                using SqlCommand insertTownCmd= new SqlCommand(insertTownQueryText,sqlConnection);
                insertTownCmd.Parameters.AddWithValue("@townName", minionTown);

                insertTownCmd.ExecuteNonQuery();

                townId = getTownIdCmd.ExecuteScalar().ToString();

                output.AppendLine($"Town {minionTown} was added to the database.");
            }

            return townId;
        }
    }
}
