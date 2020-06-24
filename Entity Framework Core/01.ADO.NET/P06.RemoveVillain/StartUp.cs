using System;
using System.Text;
using System.Threading.Channels;
using Microsoft.Data.SqlClient;

namespace P06.RemoveVillain
{
    public class StartUp
    {
        private const string ConnectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";
        static void Main(string[] args)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();

            int villainId = int.Parse(Console.ReadLine());

            string result = RemoveVillainById(sqlConnection, villainId);

            Console.WriteLine(result);
        }

        private static string RemoveVillainById(SqlConnection sqlConnection, int villainId)
        {
            StringBuilder sb = new StringBuilder();

            using SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            string getVillainIdQueryText = @"SELECT [Name] FROM Villains
                                             WHERE Id = @villainId";
            using SqlCommand getVillainNameCmd = new SqlCommand(getVillainIdQueryText, sqlConnection);
            getVillainNameCmd.Parameters.AddWithValue("@villainId", villainId);
            getVillainNameCmd.Transaction = sqlTransaction;

            string villainName = getVillainNameCmd.ExecuteScalar()?.ToString();

            if (villainName == null)
            {
                sb.AppendLine("No such villain was found.");
            }
            else
            {
                try
                {
                    string releaseMinionsQueryText = @"DELETE FROM MinionsVillains
                                                       WHERE MinionId = @villainId";
                    using SqlCommand releaseMinionsCmd = new SqlCommand(releaseMinionsQueryText, sqlConnection);
                    releaseMinionsCmd.Parameters.AddWithValue("@villainId", villainId);
                    releaseMinionsCmd.Transaction = sqlTransaction;

                    int releasedMinionsCount = releaseMinionsCmd.ExecuteNonQuery();

                    string deleteVillainQueryText = @"DELETE FROM Villains
                                                      WHERE Id = @villainId";
                    using SqlCommand deleteVillain = new SqlCommand(deleteVillainQueryText,sqlConnection);
                    deleteVillain.Parameters.AddWithValue("@villainId", villainId);
                    deleteVillain.Transaction = sqlTransaction;
                    deleteVillain.ExecuteNonQuery();

                    sqlTransaction.Commit();

                    sb.AppendLine($"{villainName} was deleted.")
                        .AppendLine($"{releasedMinionsCount} minions were released.");
                }
                catch (Exception ex)
                {
                    sb.AppendLine(ex.Message);

                    try
                    {
                        sqlTransaction.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        sb.AppendLine(rollbackEx.Message);
                    }
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}
