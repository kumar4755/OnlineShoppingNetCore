using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OnlineShopping.Infrastructure
{
    public interface IGenericRepository
    {
        IDbConnection GetOpenConnection();
    }
}
