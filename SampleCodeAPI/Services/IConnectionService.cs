using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_JeeSale.Services
{
    public interface IConnectionService
    {
        /// <summary>
        /// Get connect string (env product: connect cache)
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        string getConnectionString(long customerID);
    }
}
