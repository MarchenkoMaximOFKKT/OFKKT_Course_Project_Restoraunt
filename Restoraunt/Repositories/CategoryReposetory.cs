using Npgsql;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using StringToConnect;
 
namespace Repositories;
public class CategoryRepository{
    private readonly string _cs=DBconf.dbConnectionString;
    public Category? Get(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            return connection.QueryFirstOrDefault<Category>("SELECT * FROM Category WHERE id=@id", new {id});
        }
        catch(Exception ex){Console.WriteLine($"CategoryRepository.Get: {ex.Message}"); return null;}
    }
    public List<Category> GetAll(){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Category";
            return connection.Query<Category>(sql).ToList();
        }
        catch(Exception ex){Console.WriteLine($"CategoryRepository.GetAll: {ex.Message}"); return new List<Category>();}
    }
}