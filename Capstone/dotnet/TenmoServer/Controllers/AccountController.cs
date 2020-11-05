using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountSqlDAO accountSqlDAO;
        private readonly IUserDAO UserDAO;
        //dependancy injection
        public AccountController(IAccountSqlDAO _accountDAO, IUserDAO _userDAO)
        {
            accountSqlDAO = _accountDAO;
            UserDAO = _userDAO;
        }

        [HttpGet("balance")]
        public IActionResult currentBalance()
        {
            int? userId = GetCurrentUserId();
            decimal? balance = accountSqlDAO.GetBalance(userId);

            return Ok(balance);
        }

        private int? GetCurrentUserId()
        {
            string userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return null;
            int.TryParse(userId, out int userIdInt);
            return userIdInt;
        }

        [HttpGet("list")]
        public IActionResult ListUsers()
        {
            List<ReturnUser> listOfUsers = null;
            listOfUsers = accountSqlDAO.GetListOfUsers();

            if(listOfUsers == null)
            {
                return StatusCode(500);
            }
            else
            {
                return Ok(listOfUsers);
            }
        }

        //send te bucks
        [HttpPost("transfer")]
        public ActionResult MakeTransfer(Transfer transfer)
        {
            int rowsAffected = accountSqlDAO.MakeTransfer(transfer);

            if(rowsAffected > 0)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("transfer/list")]
        public ActionResult<List<Transfer>> GetTransferList()
        {
            List<Transfer> transferList = null;

            transferList = accountSqlDAO.GetTransferList(GetCurrentUserId());

            if(transferList.Count == 0)
            {
                return StatusCode(500);
            }
            else
            {
                return Ok(transferList);
            }
        }

        //view your past transfers
        //view your pending requests

        //log in as different user


        //send back responses
        //return status codes(with the object)

        //verify with postman
    }
}
