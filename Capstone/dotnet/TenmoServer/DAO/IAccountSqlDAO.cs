using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountSqlDAO
    {
        decimal? GetBalance(int? userId);
        int MakeTransfer(int? fromUserId, int toUserId, decimal transferAmount);
        List<ReturnUser> GetListOfUsers(int? userId);

    }
}
