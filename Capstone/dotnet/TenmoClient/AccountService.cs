using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using TenmoClient.Data;

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

        public List<ReturnUser> GetListOfUsers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "list");
            IRestResponse<List<ReturnUser>> response = client.Get<List<ReturnUser>>(request);
            
            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            return response.Data;
        }
        //  response = client get users
        //  return response.data

        public void MakeTransfer(ReturnUser toUser, decimal transferAmount)
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
            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }
            return response.Data;
        }
    }
}
