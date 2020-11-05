using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    public class ConsoleService
    {
        /// <summary>
        /// Prompts for transfer ID to view, approve, or reject
        /// </summary>
        /// <param name="action">String to print in prompt. Expected values are "Approve" or "Reject" or "View"</param>
        /// <returns>ID of transfers to view, approve, or reject</returns>
        public int PromptForTransferID(string action)
        {
            Console.WriteLine("");
            Console.Write("Please enter transfer ID to " + action + " (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int auctionId))
            {
                Console.WriteLine("Invalid input. Only input a number.");
                return 0;
            }
            else
            {
                return auctionId;
            }
        }

        public LoginUser PromptForLogin()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            string password = GetPasswordFromConsole("Password: ");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        private string GetPasswordFromConsole(string displayMessage)
        {
            string pass = "";
            Console.Write(displayMessage);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
            return pass;
        }

        public ReturnUser GetValidUserFromList(List<ReturnUser> list)
        {
            int choice;
            Dictionary<int, ReturnUser> userIdAndObject = new Dictionary<int, ReturnUser>();
            while (true)
            {
                Console.WriteLine($"-------------------------------------------\nUsers\n{"ID":0,7}Name\n-------------------------------------------\n");
                foreach(ReturnUser user in list)
                {
                    if(user.UserId == UserService.GetUserId())
                    {
                        continue;
                    }
                    Console.WriteLine($"{user.UserId, -7}{user.Username}");
                    userIdAndObject[user.UserId] = user;
                }

                Console.WriteLine("-------------------------------------------");

                Console.WriteLine("Select ID of user you are transferring to (0 to cancel):");
                if (int.TryParse(Console.ReadLine(), out int userChoice))
                {
                    if (userIdAndObject.ContainsKey(userChoice) || userChoice == 0)
                    {
                        if(userChoice == 0)
                        {
                            Console.WriteLine("Exiting...");
                            Console.Clear();
                            return null;
                        }
                        else
                        {
                            choice = userChoice;
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("*** Not a valid username. ***\n");
                    }
                }
            }
            return userIdAndObject[choice];
        }

        public decimal GetValidTransferAmount(decimal? balance)
        {
            decimal transferAmount;

            while (true)
            {
                Console.WriteLine("Please enter a transfer amount (0 to cancel):\n");
                if(!decimal.TryParse(Console.ReadLine(), out decimal userTransferAmount) || userTransferAmount > balance || userTransferAmount < 0)
                {
                    Console.WriteLine("*** Not a valid transfer amount. ***\n");
                }
                else
                {
                    if (userTransferAmount == 0)
                    {
                        Console.WriteLine("Exiting...");
                        return 0;
                    }
                    transferAmount = userTransferAmount;
                    break;
                }
            }

            return transferAmount;
        }

        public void PrintTransferDetails(Transfer selectedTransfer, List<ReturnUser> list)
        {
            Dictionary<int, string> userIdAndObject = new Dictionary<int, string>();

            foreach (ReturnUser user in list)
            {
                userIdAndObject[user.UserId] = user.Username;
            }

            string from_user = userIdAndObject[selectedTransfer.from_account];
            string to_user = userIdAndObject[selectedTransfer.to_account];
            Console.WriteLine("--------------------------------------------\n" 
                              + "Transfer Details\n" +
                              "--------------------------------------------");

            Console.WriteLine($"{"Id:", -10}{selectedTransfer.transferId}\n" +
                                $"{"From:", -10}{from_user}\n" +
                                $"{"To:", -10}{to_user}\n" +
                                $"{"Type:", -10}{selectedTransfer.transfer_type}\n" +
                                $"{"Status:", -10}{selectedTransfer.transfer_status}\n" +
                                $"{"Amount:", -10}{selectedTransfer.amount:c}\n");
        }

        public Transfer ValidateTransferDetailsChoice(List<Transfer> transferList)
        {
            Dictionary<int, Transfer> transferIdAndObject = new Dictionary<int, Transfer>();

            foreach(Transfer transfer in transferList)
            {
                transferIdAndObject[transfer.transferId] = transfer;
            }

            while (true)
            {
                Console.WriteLine("Please enter transfer ID to view details (0 to cancel): ");
                if(int.TryParse(Console.ReadLine(), out int transferId))
                {
                    if (transferId == 0)
                    {
                        return null;
                    }
                    if (transferIdAndObject.ContainsKey(transferId))
                    {
                        return transferIdAndObject[transferId];
                    }
                }
            }
        }

        public void PrintPreviousTransfers(List<Transfer> transferList, List<ReturnUser> userList)
        {
            Dictionary<int, string> userIdAndUsername = new Dictionary<int, string>();
            foreach(ReturnUser user in userList)
            {
                userIdAndUsername[user.UserId] = user.Username;
            }

            if(transferList != null)
            {
                Console.WriteLine($"-------------------------------------------\nTransfers\n{"ID", -7}{"From/To", -17}{"Amount":c}\n-------------------------------------------\n");

                int id;
                string username;
                decimal amount;

                foreach (Transfer transfer in transferList)
                {
                    if(transfer.transfer_type != "Requested")
                    {
                        username = "From: " + userIdAndUsername[transfer.from_account];
                    }
                    else
                    {
                        username = "To: " + userIdAndUsername[transfer.to_account];
                    }

                    id = transfer.transferId;
                    amount = transfer.amount;

                    Console.WriteLine($"{id, -7}{username, -17}{amount:c}");
                }

                Console.WriteLine("---------");
                //print tansfer.Id
                //if it's not requested print from username
                //print the transfer amount
            }
        }

        //method for error handling
    }
}
