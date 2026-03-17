using DPSinfra.ConnectionCache;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_JeeSale.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly IConnectionCache connectionCache;
        private readonly IConfiguration confiuration;

        public ConnectionService(IConnectionCache connectionCache, IConfiguration confiuration)
            => (this.connectionCache, this.confiuration) = (connectionCache, confiuration);

        public string getConnectionString(long customerID)
        {
            try
            {
                var isDev = confiuration.GetValue<string>("AppConfig:IsDev");
                if (isDev == "true")
                    return confiuration.GetValue<string>("AppConfig:ConnectionString") ?? "";
                else
                {
                    return connectionCache.GetConnectionString(customerID);
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
