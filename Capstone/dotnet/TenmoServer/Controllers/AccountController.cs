using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class AccountController : ControllerBase
    {
        //dependancy injection
        //make a dao for users
        //make a dao 

        //httpget["{id}"]
        //public actionresult<decimal> currentbalance(int id)
        //account_dao.GetBalance(id)
        //was it able to get the balance?
        //return status code(decimal)



        //send te bucks






        //view your past transfers
        //view your pending requests

        //log in as different user


        //send back responses
        //return status codes(with the object)

        //verify with postman
    }
}
