using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace SqlActSim
{
    internal class SqlMgr
    {
        private string? ConnectionString;


        public SqlMgr()
        {
            ConnectionString = null;
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
                Console.WriteLine("DB:ConnectionString: " + ConnectionString);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }


        public void InsertActivity(SqlConnection Connection, IActivity Activity, long index)
        {
            try
            {

                Activity.FillLine(index);

                SqlCommand cmd = Activity.CreateCommand(Connection);
                int rows = cmd.ExecuteNonQuery();
                if (rows != 1)
                {
                    Console.WriteLine($"Error. InsertActivity {Activity.GetType().Name} failed to insert line.");
                }
                else
                {
                    Console.WriteLine($"{GlobalStatic.Counter}>> Inserted Activity line: {Activity.Name}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Cannot insert activity {Activity.GetType().Name}.{Activity.Name}.\n\tException: {ex.Message}");
            }
        }


        public void UpdateDB(IActivity Activity, long index)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    InsertActivity(connection, Activity, index);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

        }
  

        public bool TestAccess()
        {
            bool Status = false;
            Console.WriteLine("** Test Access: Read OUs");
            try
            {

                string query = "SELECT * FROM OU ORDER BY Name";
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                { 
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        const int max = 3;
                        int i = 0;
                        while (reader.Read() )
                        {
                            if(i++ < max)
                            {
                                Console.WriteLine($"{i} > {reader[0]}");
                            }
                        }
                        Console.WriteLine("...");
                        Console.WriteLine($"Total: Rows = {i}; Cols={reader.FieldCount}");
                        Status = i > 0 && reader.FieldCount > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return Status;
        }

    }
}
