using RestSharp;
using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly AccountService accountService = new AccountService();

        static void Main(string[] args)
        {
            Run();
        }
        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                    menuSelection = -1;
                }
                else if (menuSelection == 1)
                {
                    string output;
                    IRestResponse<decimal> response = accountService.GetBalance();
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        output = $"Balance: {response.Data:c}";
                    }
                    else
                    {
                        output = "Unable to reach server.";
                    }
                    Console.WriteLine(output);
                }
                else if (menuSelection == 2)
                {
                    //view your past transfers
                    List<Transfer> transferList = accountService.GetPreviousTransfers();
                    if(transferList != null)
                    {
                        List<ReturnUser> userList = accountService.GetListOfUsers(); //get username
                        if (userList != null)
                        {
                            bool pending = false;
                            consoleService.PrintPreviousTransfers(transferList, userList, pending);
                            Transfer selectedTransfer = consoleService.ValidateTransferDetailsChoice(transferList, pending);
                            if (selectedTransfer != null)
                            {
                                consoleService.PrintTransferDetails(selectedTransfer, userList);
                            }
                            else
                            {
                                Console.WriteLine("Couldn't get transfer details.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Couldn't retreive User List while getting past transfers.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Couldn't get list of past transfers.");
                    }
                }
                else if (menuSelection == 3)
                {
                    //view your pending requests
                    List<Transfer> transferList = accountService.GetPreviousTransfers();
                    if (transferList != null)
                    {
                        List<ReturnUser> userList = accountService.GetListOfUsers();
                        if (userList != null)
                        {
                            bool pending = true;
                            consoleService.PrintPreviousTransfers(transferList, userList, pending);
                            Transfer selectedTransfer = consoleService.ValidateTransferDetailsChoice(transferList, pending);
                            if (selectedTransfer != null)
                            {
                                IRestResponse<decimal> response = accountService.GetBalance();
                                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    Console.WriteLine("Unable to reach server.");
                                }
                                decimal balance = response.Data;
                                int userChoice = consoleService.ValidateApproveOrReject(selectedTransfer.amount, balance);

                                bool approved = userChoice == 1 ? true : false;

                                if (userChoice == 1 || userChoice == 2)
                                {
                                    //approve
                                    accountService.UpdateTransfer(selectedTransfer, approved);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Couldn't get transfer details.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Couldn't retreive User List while getting past transfers.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Couldn't get list of past transfers.");
                    }
                }
                else if (menuSelection == 4)
                {
                    //sending TE Bucks
                    IRestResponse<decimal> response = accountService.GetBalance();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        decimal balance = response.Data;
                        //GetUserFromListOfUsers(list of users)
                        List<ReturnUser> userList = accountService.GetListOfUsers();
                        if (userList.Count != 0)
                        {
                            //pass the user list to Console Service(listOfUsers)  => This displays the list of users, prompts of a selection, returns the selected user
                            ReturnUser transferToThisUser = consoleService.GetValidUserFromList(userList, true);

                            if (transferToThisUser != null)
                            {
                                //verifytransferamount(fromUser)
                                decimal transferAmount = consoleService.GetValidTransferAmount(balance);
                                if (transferAmount != 0)
                                {
                                    //send te bucks to specified user
                                    Transfer transfer = consoleService.PopulateTransfer("Send", "Approved", transferToThisUser.UserId, UserService.GetUserId(), transferAmount);
                                    accountService.MakeTransfer(transfer);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Unable to retreive User from List of Users while making a transfer");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Unable to get User List while making a transfer.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unable to get balance.");
                    }
                }
                else if (menuSelection == 5)
                {
                    //request TE bucks
                    IRestResponse<decimal> response = accountService.GetBalance();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //GetUserFromListOfUsers(list of users)
                        List<ReturnUser> userList = accountService.GetListOfUsers();
                        if (userList.Count != 0)
                        {
                            //pass the user list to Console Service(listOfUsers)  => This displays the list of users, prompts of a selection, returns the selected user
                            ReturnUser requestFromThisUser = consoleService.GetValidUserFromList(userList, false);

                            if (requestFromThisUser != null)
                            {
                                //verifytransferamount(fromUser)
                                decimal transferAmount = consoleService.GetValidTransferAmount();
                                if (transferAmount != 0)
                                {
                                    //send te bucks to specified user
                                    Transfer transfer = consoleService.PopulateTransfer("Request", "Pending", UserService.GetUserId(), requestFromThisUser.UserId, transferAmount);
                                    accountService.MakeTransfer(transfer);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Unable to retreive User from List of Users while making a request");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Unable to get User List while making a request.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unable to get balance.");
                    }
                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
