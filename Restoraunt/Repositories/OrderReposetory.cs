using Npgsql;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using StringToConnect;
 
namespace Repositories;
public class OrderRepository{
    private readonly string _cs=DBconf.dbConnectionString;
    private readonly TableRepository _tableRepo;
    public OrderRepository(TableRepository tableRepo)=>this._tableRepo=tableRepo;
    public Order? Get(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Order_ WHERE id=@id";
            return connection.QueryFirstOrDefault<Order>(sql, new {id});
        }
        catch(Exception ex){Console.WriteLine($"OrderRepository.Get: {ex.Message}"); return null;}
    }
    public Order? GetActive(int idCustomer){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Order_ WHERE id_customer=@idCustomer AND status NOT IN ('Оплачено','Скасовано') LIMIT 1";
            return connection.QueryFirstOrDefault<Order>(sql, new {idCustomer});
        }
        catch(Exception ex){Console.WriteLine($"OrderRepository.GetActive: {ex.Message}"); return null;}
    }
    public List<Order> GetAll(int idCustomer){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Order_ WHERE id_customer=@idCustomer";
            return connection.Query<Order>(sql, new {idCustomer}).ToList();
        }
        catch(Exception ex){Console.WriteLine($"OrderRepository.GetAll: {ex.Message}"); return new List<Order>();}
    }
    public bool Add(int idCustomer, int? idPersonnel, DateTime startTime, DateTime endTime, string type="Замовлення", string? comment=null){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql=@"
                INSERT INTO Order_ (id_customer, id_personnel, type_, start_time, end_time, comment, status, total_price)
                VALUES (@idCustomer, @idPersonnel, @type, @startTime, @endTime, @comment, 'Нове', 0.0)";
            return connection.Execute(sql, new {idCustomer, idPersonnel, type, startTime, endTime, comment})>0;
        }
        catch(Exception ex){Console.WriteLine($"OrderRepository.Add: {ex.Message}"); return false;}
    }
    public bool UpdateStatus(int idOrder, string status){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="UPDATE Order_ SET status=@status WHERE id=@idOrder";
            return connection.Execute(sql, new {status, idOrder})>0;
        }
        catch(Exception ex){Console.WriteLine($"OrderRepository.UpdateStatus: {ex.Message}"); return false;}
    }
    public bool UpdateTotal(int idOrder, decimal total){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="UPDATE Order_ SET total_price=@total WHERE id=@idOrder";
            return connection.Execute(sql, new {total, idOrder})>0;
        }
        catch(Exception ex){Console.WriteLine($"OrderRepository.UpdateTotal: {ex.Message}"); return false;}
    }
}
