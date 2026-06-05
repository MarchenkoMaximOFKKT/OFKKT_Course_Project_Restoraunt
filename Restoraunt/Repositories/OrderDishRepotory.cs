using Npgsql;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using StringToConnect;
 
namespace Repositories;
public class OrderDishRepository{
    private readonly string _cs=DBconf.dbConnectionString;
    public OrderDish? Get(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql=@"
                SELECT od.id, od.id_order, od.id_dish, od.quantity, d.id, d.id_category, d.name_, d.price
                FROM OrderDish od JOIN Dish d ON od.id_dish=d.id WHERE od.id=@id";
            return connection.Query<OrderDish, Dish, OrderDish>(sql, (od, dish)=>{od.Dish=dish; return od;}, new {id}, splitOn: "id").FirstOrDefault();
        }
        catch(Exception ex){Console.WriteLine($"OrderDishRepository.Get: {ex.Message}"); return null;}
    }
    public List<OrderDish> GetAll(int idOrder){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql=@"
                SELECT od.id, od.id_order, od.id_dish, od.quantity, d.id, d.id_category, d.name_, d.price
                FROM OrderDish od JOIN Dish d ON od.id_dish=d.id WHERE od.id_order=@idOrder";
            return connection.Query<OrderDish, Dish, OrderDish>(sql, (od, dish)=>{ od.Dish=dish; return od;}, new {idOrder}, splitOn:"id").ToList();
        }
        catch(Exception ex){Console.WriteLine($"OrderDishRepository.GetAll: {ex.Message}"); return new List<OrderDish>();}
    }
    public bool Add(int idOrder, int idDish, int quantity){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql=@"
                INSERT INTO OrderDish (id_order, id_dish, quantity)
                VALUES (@idOrder, @idDish, @quantity)
                ON CONFLICT (id_order, id_dish) DO UPDATE SET quantity=OrderDish.quantity+EXCLUDED.quantity";
            return connection.Execute(sql, new {idOrder, idDish, quantity})>0;
        }
        catch(Exception ex){Console.WriteLine($"OrderDishRepository.Add: {ex.Message}"); return false;}
    }
    public bool UpdateQuantity(int idOrder, int idDish, int quantity){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="UPDATE OrderDish SET quantity=@quantity WHERE id_order=@idOrder AND id_dish=@idDish";
            return connection.Execute(sql, new {quantity, idOrder, idDish})>0;
        }
        catch(Exception ex){Console.WriteLine($"OrderDishRepository.UpdateQuantity: {ex.Message}"); return false;}
    }
    public bool Remove(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="DELETE FROM OrderDish WHERE id=@id";
            return connection.Execute(sql, new {id})>0;
        }
        catch(Exception ex){Console.WriteLine($"OrderDishRepository.Remove: {ex.Message}"); return false;}
    }
}