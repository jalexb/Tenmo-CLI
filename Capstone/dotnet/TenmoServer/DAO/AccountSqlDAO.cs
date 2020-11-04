using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO : IAccountSqlDAO
    {

        private readonly string ConnectionString;
        public AccountSqlDAO(string _connectionString)
        {
            ConnectionString = _connectionString;
        }

        public decimal? GetBalance(int? userId)
        {
            //  subquery that SELECT account balance FROM account WHERE (SELECT user id FROM user WHERE userid = @userid )
            //return executescalar
            decimal? balance = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT balance FROM accounts WHERE user_id = @userId;", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    balance = (decimal)cmd.ExecuteScalar();
                }                
            }
            catch (SqlException e)
            {
                Console.WriteLine("Failed to Get Balance: " + e);
            }
            return balance;
        }

        public int MakeTransfer(int? fromUserId, int toUserId, decimal transferAmount)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO transfers(transfer_type_id, " +
                    "transfer_status_id, account_from, account_to, amount) VALUES ((SELECT " +
                    "transfer_type_id FROM transfer_types WHERE transfer_type_desc = @transferType)," +
                    "(SELECT transfer_status_id FROM transfer_statuses WHERE transfer_status_desc = @transferStatus)," +
                    "@accountFrom, @accountTo, @amount);", conn);
                cmd.Parameters.AddWithValue("@transferType", "Send");
                cmd.Parameters.AddWithValue("@transferStatus", "Approved");
                cmd.Parameters.AddWithValue("@accountFrom", fromUserId);
                cmd.Parameters.AddWithValue("@accountTo", toUserId);
                cmd.Parameters.AddWithValue("@amount", transferAmount);
                
                int rowsAffected = cmd.ExecuteNonQuery();
                if(rowsAffected > 0)
                {
                    UpdateBalances(fromUserId, toUserId, transferAmount);
                }                              

                return rowsAffected;
            }
        }

        public void UpdateBalances(int? senderId, int receiverId, decimal transferAmount)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE accounts SET balance = balance - @transferAmount WHERE user_id = @senderId;" +
                                                "UPDATE accounts SET balance = balance + @transferAmount WHERE user_id = @receiverId;", conn);

                cmd.Parameters.AddWithValue("@senderId", senderId);
                cmd.Parameters.AddWithValue("@receiverId", receiverId);
                cmd.Parameters.AddWithValue("@transferAmount", transferAmount);
                
                cmd.ExecuteNonQuery();
            }

            
        }

        public List<ReturnUser> GetListOfUsers(int? userId)
        {
            List<ReturnUser> listOfUsers = new List<ReturnUser>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT username, user_id FROM users WHERE user_id != @userId;", conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ReturnUser user = new ReturnUser();
                    user.Username = Convert.ToString(reader["username"]);
                    user.UserId = Convert.ToInt32(reader["user_id"]);

                    listOfUsers.Add(user);
                }
            }
            return listOfUsers;
        }

        //public <list<users> transfer(fromuser, touser, sendAmount)
        //  sends the sendAmount fromUser toUser
        //  create a new row in the transfer table
        //  populate the transfer table with ()
        //  initialize 
        //



    }
}
