using Npgsql;
using Dapper;
using Models;
using System;
using StringToConnect;
 
namespace Repositories;
public class PersonnelRepository{
    private readonly string _cs=DBconf.dbConnectionString;
    public Personnel? Get(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Personnel WHERE id=@id";
            return connection.QueryFirstOrDefault<Personnel>(sql, new {id});
        }
        catch(Exception ex){Console.WriteLine($"PersonnelRepository.Get: {ex.Message}"); return null;}
    }
    public bool Add(Personnel person){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql=@"
                INSERT INTO Personnel (fullname, position_, phone, is_active, priority_, salary) 
                VALUES (@Fullname, @Position, @Phone, true, 0, @Salary)";
            return connection.Execute(sql, person)>0;
        }
        catch(Exception ex){Console.WriteLine($"PersonnelRepository.Add: {ex.Message}"); return false;}
    }
    public Personnel? GetFirstAvailible(){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Personnel WHERE is_active=true AND position_='Офіціант' ORDER BY priority_ ASC, id ASC LIMIT 1";
            return connection.QueryFirstOrDefault<Personnel>(sql);
        }
        catch(Exception ex){Console.WriteLine($"PersonnelRepository.GetFirstAvailible: {ex.Message}"); return null;}
    }
    public bool ChangeStatus(int id, bool isActive, int priority){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="UPDATE Personnel SET is_active=@isActive, priority_=@priority WHERE id=@id";
            return connection.Execute(sql, new {id, isActive, priority})>0;
        }
        catch(Exception ex){Console.WriteLine($"PersonnelRepository.ChangeStatus: {ex.Message}"); return false;}
    }
    public bool Remove(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="DELETE FROM Personnel WHERE id=@id";
            return connection.Execute(sql, new {id})>0;
        }
        catch(Exception ex){Console.WriteLine($"PersonnelRepository.Remove: {ex.Message}"); return false;}
    }
}