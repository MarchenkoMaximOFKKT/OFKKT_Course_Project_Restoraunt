using Npgsql;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using StringToConnect;
using System.Threading.Tasks;

namespace Repositories;
public class MenuRepository{
    private readonly string _cs=DBconf.dbConnectionString;
    public Menu? Get(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Menu WHERE id=@id";
            var menu=connection.QueryFirstOrDefault<Menu>(sql, new {id});
            if(menu==null) return null;
            string menuSql=@"  
                SELECT d.*, c.id, c.name_ FROM Dish d
                LEFT JOIN Category c ON d.id_category=c.id
                JOIN MenuDish md ON md.id_dish=d.id
                WHERE md.id_menu=@id";
            menu.Dishes=connection.Query<Dish, Category, Dish>(menuSql, (dish, cat)=>{dish.Category=cat!; return dish;},new {id}, splitOn:"id").ToList();
            return menu;
        }
        catch(Exception ex){Console.WriteLine($"MenuRepository.Get: {ex.Message}"); return null;}
    }
    public Menu? GetFirst(){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Menu";
            return connection.QueryFirstOrDefault<Menu>(sql);
        }
        catch(Exception ex){Console.WriteLine($"MenuRepository.GetFirst: {ex.Message}"); return null;}
    }
}