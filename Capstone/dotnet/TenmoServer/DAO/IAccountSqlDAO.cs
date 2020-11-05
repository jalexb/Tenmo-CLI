using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountSqlDAO
    {
        decimal GetBalance(int? userId);
        int MakeTransfer(Transfer transfer);
        List<ReturnUser> GetListOfUsers();
        List<Transfer> GetTransferList(int? userId);

    }
}
