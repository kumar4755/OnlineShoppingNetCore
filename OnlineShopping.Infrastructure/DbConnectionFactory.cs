using OnlineShopping.Infrastructure;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace OnlineShopping.Infrastructure
{
    public class DbConnectionFactory
    {
        public static IDbConnection GetDbConnection(EDbConnectionTypes dbType, string connectionString)
        {
            IDbConnection connection = null;

            switch (dbType)
            {
                case EDbConnectionTypes.SQL:
                    connection = new SqlConnection(connectionString);
                    break;
                case EDbConnectionTypes.XML:
                    // TODO: Implement XML Connection (path name)
                    break;
                case EDbConnectionTypes.DOCUMENT:
                    // TODO: Implement Document DB connection
                    break;
                default:
                    connection = null;
                    break;
            }

            connection.Open();
            return connection;
        }
    }

    public enum EDbConnectionTypes
    {
        SQL,
        DOCUMENT,
        XML
    }
}


/// <summary>
/// The concrete implementation of a SQL repository
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public abstract class SqlRepository : IGenericRepository
{
    private string _connectionString;
    private EDbConnectionTypes _dbType;

    public SqlRepository(string connectionString)
    {
        _dbType = EDbConnectionTypes.SQL;
        _connectionString = connectionString;
    }

    public IDbConnection GetOpenConnection()
    {
        return DbConnectionFactory.GetDbConnection(_dbType, _connectionString);
    }
}


