using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class Transfer
    {
        public int transferId { get; set; }
        public string transfer_type { get; set; }
        public string transfer_status { get; set; }
        public int from_account { get; set; }
        public int to_account { get; set; }
        public decimal amount { get; set; }
    }
}
