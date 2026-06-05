using Npgsql;
using Dapper;
using Models;
using System;
using StringToConnect;
 
namespace Repositories;
public class PaymentRepository{
    private readonly string _cs=DBconf.dbConnectionString;
    public Payment? Get(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Payment WHERE id=@id";
            return connection.QueryFirstOrDefault<Payment>(sql, new {id});
        }
        catch(Exception ex){Console.WriteLine($"PaymentRepository.Get: {ex.Message}"); return null;}
    }
    public Payment? GetByOrder(int idOrder){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Payment WHERE id_order=@idOrder";
            return connection.QueryFirstOrDefault<Payment>(sql, new {idOrder});
        }
        catch(Exception ex){Console.WriteLine($"PaymentRepository.GetByOrder: {ex.Message}"); return null;}
    }
    public bool Add(int idOrder, decimal total, string method="Готівка"){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql=@"
                INSERT INTO Payment (id_order, total, payment_method) 
                VALUES (@idOrder, @total, @method)";
            return connection.Execute(sql, new {idOrder, total, method})>0;
        }
        catch(Exception ex){Console.WriteLine($"PaymentRepository.Add: {ex.Message}"); return false;}
    }
    public bool UpdateTotal(int idPayment, decimal total){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="UPDATE Payment SET total=@total WHERE id=@idPayment";
            return connection.Execute(sql, new {total, idPayment})>0;
        }
        catch(Exception ex){Console.WriteLine($"PaymentRepository.UpdateTotal: {ex.Message}"); return false;}
    }
    public bool UpdateMethod(int idPayment, string method){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="UPDATE Payment SET payment_method=@method WHERE id=@idPayment";
            return connection.Execute(sql, new {method, idPayment})>0;
        }
        catch(Exception ex){Console.WriteLine($"PaymentRepository.UpdateMethod: {ex.Message}"); return false;}
    }   
}