using Npgsql;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using StringToConnect;
 
namespace Repositories;
public class TableRepository{
    private readonly string _cs=DBconf.dbConnectionString;
    public Table? Get(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Table_ WHERE id=@id";
            return connection.QueryFirstOrDefault<Table>(sql, new {id});
        }
        catch(Exception ex){Console.WriteLine($"TableRepository.Get: {ex.Message}"); return null;}
    }
    public List<Table> GetAll(){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Table_";
            return connection.Query<Table>(sql).ToList();
        }
        catch(Exception ex){Console.WriteLine($"TableRepository.GetAll: {ex.Message}"); return new List<Table>();}
    }
    public List<Table> GetByOrder(int idOrder){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT t.* FROM Table_ t JOIN TableShedule ts ON t.id=ts.id_table WHERE ts.id_order=@idOrder";
            return connection.Query<Table>(sql, new {idOrder}).ToList();
        }
        catch(Exception ex){Console.WriteLine($"TableRepository.GetByOrder: {ex.Message}"); return new List<Table>();}
    }
    public bool UpdateStatus(List<int> ids, string status="Вільний"){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="UPDATE Table_ SET status=@status WHERE id=ANY(@ids)";
            return connection.Execute(sql, new {status, ids=ids.ToArray()})>0;
        }
        catch(Exception ex){Console.WriteLine($"TableRepository.UpdateStatus: {ex.Message}"); return false;}
    }
}