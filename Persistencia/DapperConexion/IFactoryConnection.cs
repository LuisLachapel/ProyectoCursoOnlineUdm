using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Persistencia.DapperConexion
{
    public interface IFactoryConnection
    {
        void CloseConnection();
        IDbConnection GetConnection();
    }
}
