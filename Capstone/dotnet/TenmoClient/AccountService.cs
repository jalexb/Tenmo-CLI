using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using TenmoClient.Data;
using TenmoServer.Models;

namespace TenmoClient
{

    public class AccountService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/account/";
        private readonly IRestClient client = new RestClient();



        public IRestResponse<decimal> GetBalance()
        {
            //should print the balance in ConsoleService, and check if decimal is null or not
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "balance");
            IRestResponse<decimal> response = client.Get<decimal>(request);

            return response;
            //  response = client get balance
            //  return response.data
        }

        public List<API_User> GetListOfUsers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "list");
            List<API_User> userList = new List<API_User>();
            IRestResponse<List<ReturnUser>> response = client.Get<List<ReturnUser>>(request);
            
            foreach(ReturnUser user in response.Data)
            {
                API_User apiUser = new API_User();
                apiUser.UserId = user.UserId;
                apiUser.Username = user.Username;

                userList.Add(apiUser);
            }
            return userList;
        }
        //  response = client get users
        //  return response.data

        public void MakeTransfer(API_User toUser, decimal transferAmount)
        {
            Transfer transfer = new Transfer();
            transfer.transfer_type = "Send";
            transfer.transfer_status = "Approved";
            transfer.to_account = toUser.UserId;
            transfer.from_account = UserService.GetUserId();
            transfer.amount = transferAmount;

            //client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestRequest request = new RestRequest(API_BASE_URL + "transfer");
            request.AddJsonBody(transfer);
            IRestResponse response = client.Post(request);
        }

        public List<Transfer> GetPreviousTransfers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestRequest request = new RestRequest(API_BASE_URL + "transfer/list");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

            return response.Data;
        }
    }
}
