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

        public decimal GetBalance(int? userId)
        {
            decimal balance = 0;

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

        public int UpdateRequest(Transfer transfer)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE transfers SET transfer_status_id = (SELECT transfer_status_id FROM transfer_statuses " +
                                                "WHERE transfer_status_desc = @transferStatus) WHERE transfer_id = @transferId;", conn);
                cmd.Parameters.AddWithValue("@transferStatus", transfer.transfer_status);
                cmd.Parameters.AddWithValue("@transferId", transfer.transferId);

                rowsAffected = cmd.ExecuteNonQuery();
            }
            //pass this into a method in accountcontroller
            if (transfer.transfer_status == "Approved")// && userId == transfer.from_user;
            {
                UpdateBalances(transfer.from_account, transfer.to_account, transfer.amount);
            }
            
            return rowsAffected;
        }

        public int MakeTransfer(Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO transfers(transfer_type_id, " +
                    "transfer_status_id, account_from, account_to, amount) VALUES ((SELECT " +
                    "transfer_type_id FROM transfer_types WHERE transfer_type_desc = @transferType)," +
                    "(SELECT transfer_status_id FROM transfer_statuses WHERE transfer_status_desc = @transferStatus)," +
                    "@accountFrom, @accountTo, @amount);", conn);
                cmd.Parameters.AddWithValue("@transferType", transfer.transfer_type);
                cmd.Parameters.AddWithValue("@transferStatus", transfer.transfer_status);
                cmd.Parameters.AddWithValue("@accountFrom", transfer.from_account);
                cmd.Parameters.AddWithValue("@accountTo", transfer.to_account);
                cmd.Parameters.AddWithValue("@amount", transfer.amount);
                
                int rowsAffected = cmd.ExecuteNonQuery();
                if(transfer.transfer_status == "Approved" )//&& userId = transfer.from_account) 
                {
                    UpdateBalances(transfer.from_account, transfer.to_account, transfer.amount);
                }                              

                return rowsAffected;
            }
        }

        public List<Transfer> GetTransferList(int? userId)
        {
            List<Transfer> transferList = new List<Transfer>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT  transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount " +
                                                "FROM transfers WHERE account_from = @account_from OR account_to = @account_to", conn);
                cmd.Parameters.AddWithValue("@account_from", userId);
                cmd.Parameters.AddWithValue("@account_to", userId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Transfer transfer = new Transfer();

                    transfer.transferId = Convert.ToInt32(reader["transfer_id"]);
                    transfer.from_account = Convert.ToInt32(reader["account_from"]);
                    transfer.to_account = Convert.ToInt32(reader["account_to"]);
                    transfer.amount = Convert.ToDecimal(reader["amount"]);
                    transfer.transfer_type = (Convert.ToInt32(reader["transfer_type_id"]) == 1) ? "Request" : "Send";
                    transfer.transfer_status = (Convert.ToInt32(reader["transfer_status_id"]) == 1) ? "Pending" :
                                               (Convert.ToInt32(reader["transfer_status_id"]) == 2) ? "Approved" : "Rejected";

                    transferList.Add(transfer);
                }
                return transferList;
            }
        }

        public void UpdateBalances(int senderId, int receiverId, decimal transferAmount)
        {


            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();


                //verify transfer amount with currUser Balance
                //verify that the from_user is currUser

                //transaction
                //executeNonQuery
                SqlCommand cmd = new SqlCommand("UPDATE accounts SET balance = balance - @transferAmount WHERE user_id = @senderId;" +
                                                "UPDATE accounts SET balance = balance + @transferAmount WHERE user_id = @receiverId;", conn);
                //commit
                //executeNonQuery

                cmd.Parameters.AddWithValue("@senderId", senderId);
                cmd.Parameters.AddWithValue("@receiverId", receiverId);
                cmd.Parameters.AddWithValue("@transferAmount", transferAmount);
                


                cmd.ExecuteNonQuery();
            }

            
        }

        public List<ReturnUser> GetListOfUsers()
        {
            List<ReturnUser> listOfUsers = new List<ReturnUser>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT username, user_id FROM users;", conn);

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
