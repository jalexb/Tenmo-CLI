using RestSharp;
using RestSharp.Authenticators;
using System;
using TenmoClient.Data;

namespace TenmoClient
{

    class AccountService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();



        //decimal GetBalance(int userId)
        //  response = client get balance
        //  return response.data

        //List<API_User> GetListOfUsers()
        //  response = client get users
        //  return response.data
    }
}
