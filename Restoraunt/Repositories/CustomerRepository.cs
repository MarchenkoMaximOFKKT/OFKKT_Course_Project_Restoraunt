using Npgsql;
using Dapper;
using Models;
using System;
using StringToConnect;
 
namespace Repositories;
public class CustomerRepository{
    private readonly string _cs=DBconf.dbConnectionString;
    public Customer? Get(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Customer WHERE id=@id";
            return connection.QueryFirstOrDefault<Customer>(sql, new {id});
        }
        catch(Exception ex){Console.WriteLine($"CustomerRepository.Get: {ex.Message}"); return null;}
    }
    public Customer? GetByPhone(string phone){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Customer WHERE phone=@phone";
            return connection.QueryFirstOrDefault<Customer>(sql, new {phone});
        }
        catch(Exception ex){Console.WriteLine($"CustomerRepository.GetByPhone: {ex.Message}"); return null;}
    }
    public bool Add(Customer customer){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql=@"
                INSERT INTO Customer (fullname, phone) 
                VALUES (@Fullname, @Phone)";
            return connection.Execute(sql, customer)>0;
        }
        catch(Exception ex){Console.WriteLine($"CustomerRepository.Add: {ex.Message}"); return false;}
    }
    public bool Remove(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="DELETE FROM Customer CASCADE WHERE id=@id";
            return connection.Execute(sql, new {id})>0;
        }
        catch(Exception ex){Console.WriteLine($"CustomerRepository.Remove: {ex.Message}"); return false;}
    }
}