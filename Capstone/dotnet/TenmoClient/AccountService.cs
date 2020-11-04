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



        public decimal? GetBalance(string userToken)
        {
            //should print the balance in ConsoleService, and check if decimal is null or not
            client.Authenticator = new JwtAuthenticator(userToken);
            RestRequest request = new RestRequest(API_BASE_URL + "balance");
            IRestResponse<decimal?> response = client.Get<decimal?>(request);
            return response.Data;
            //  response = client get balance
            //  return response.data
        }

        public List<API_User> GetListOfUsers(string userToken)
        {
            client.Authenticator = new JwtAuthenticator(userToken);
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
            IRestRequest request = new RestRequest(API_BASE_URL + "transfer");
            request.AddJsonBody(toUser.UserId);
            request.AddJsonBody(transferAmount);
            IRestResponse response = client.Post(request);
        }
    }
}
