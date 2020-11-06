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
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "balance");
            IRestResponse<decimal> response = client.Get<decimal>(request);

            return response;
        }

        public List<ReturnUser> GetListOfUsers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + "list");
            IRestResponse<List<ReturnUser>> response = client.Get<List<ReturnUser>>(request);
            
            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Failed to get user list: " + response.StatusCode);
                return null;
            }

            return response.Data;
        }
        //  response = client get users
        //  return response.data

        public void MakeTransfer(Transfer transfer)
        {
            //client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestRequest request = new RestRequest(API_BASE_URL + "transfer");
            request.AddJsonBody(transfer);
            IRestResponse<Transfer> response = client.Post<Transfer>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"{response.Data.transfer_type} has successfully been made");
            }
            else
            {
                Console.WriteLine("Server couldn't be accessed\n\n");
            }
        }
                
        public List<Transfer> GetPreviousTransfers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestRequest request = new RestRequest(API_BASE_URL + "transfer/list");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Server couldn't be accessed");
                return null;
            }
            return response.Data;
        }

        public void UpdateTransfer(Transfer selectedTransfer, bool approved)
        {
            string transferStatus = approved ? "Approved" : "Rejected";

            selectedTransfer.transfer_status = transferStatus;

            IRestRequest request = new RestRequest(API_BASE_URL + "transfer/update");
            request.AddJsonBody(selectedTransfer);
            IRestResponse<Transfer> response = client.Put<Transfer>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"The transfer request has successfully been {response.Data.transfer_status}\n\n");
            }
            else
            {
                Console.WriteLine($"{response.StatusCode} while updating your transfer");
            }
        }
    }
}
