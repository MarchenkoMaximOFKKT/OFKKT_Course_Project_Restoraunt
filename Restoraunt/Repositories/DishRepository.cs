using Npgsql;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using StringToConnect;
 
namespace Repositories;
public class DishRepository{
    private readonly string _cs=DBconf.dbConnectionString;
    public Dish? Get(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql=@"
                SELECT d.*, c.id, c.name_ FROM Dish d
                LEFT JOIN Category c ON d.id_category=c.id
                WHERE d.id=@id";
            return connection.Query<Dish, Category, Dish>(sql, (dish, cat)=>{dish.Category=cat!; return dish;}, new {id}, splitOn: "id").FirstOrDefault();
        }
        catch(Exception ex){Console.WriteLine($"DishRepository.Get: {ex.Message}"); return null;}
    }
    public List<Dish> GetAll(){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql=@"
                SELECT d.*, c.id, c.name_ FROM Dish d
                LEFT JOIN Category c ON d.id_category=c.id";
            return connection.Query<Dish, Category, Dish>(sql, (dish, cat)=>{dish.Category=cat!; return dish;}, splitOn:"id").ToList();
        }
        catch(Exception ex){Console.WriteLine($"DishRepository.GetAll: {ex.Message}"); return new List<Dish>();}
    }
}