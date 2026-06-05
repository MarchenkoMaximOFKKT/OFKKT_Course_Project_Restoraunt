using Npgsql;
using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using StringToConnect;
 
namespace Repositories;
public class TableSheduleRepository{
    private readonly string _cs=DBconf.dbConnectionString;
    public TableShedule? Get(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM TableShedule WHERE id=@id";
            return connection.QueryFirstOrDefault<TableShedule>(sql, new {id});
        }
        catch(Exception ex){Console.WriteLine($"TableSheduleRepository.Get: {ex.Message}"); return null;}
    }
    public List<TableShedule> GetByOrder(int idOrder){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM TableShedule WHERE id_order=@idOrder";
            return connection.Query<TableShedule>(sql, new {idOrder}).ToList();
        }
        catch(Exception ex){Console.WriteLine($"TableSheduleRepository.GetByOrder: {ex.Message}"); return new List<TableShedule>();}
    }
    public List<TableShedule> GetByBooking(int idBooking){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM TableShedule WHERE id_booking=@idBooking";
            return connection.Query<TableShedule>(sql, new {idBooking}).ToList();
        }
        catch(Exception ex){Console.WriteLine($"TableSheduleRepository.GetByBooking: {ex.Message}"); return new List<TableShedule>();}
    }
    public List<TableShedule> GetOverlaps(List<int> idTable, DateTime startTime, DateTime endTime){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM TableShedule WHERE id_table=ANY(@ids) AND start_time<@endTime AND end_time>@startTime";
            return connection.Query<TableShedule>(sql, new {ids=idTable.ToArray(), startTime, endTime}).ToList();
        }
        catch(Exception ex){Console.WriteLine($"TableSheduleRepository.GetOverlaps: {ex.Message}"); return new List<TableShedule>();}
    }
    public bool Add(List<int> idTable, DateTime startTime, DateTime endTime, int? idOrder=null, int? idBooking=null, string eventType="Обслуговування"){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql=@"
                INSERT INTO TableShedule (id_table, id_order, id_booking, start_time, end_time, event_type)
                SELECT UNNEST(@ids), @idOrder, @idBooking, @startTime, @endTime, @eventType";
            return connection.Execute(sql, new {ids=idTable.ToArray(), idOrder, idBooking, startTime, endTime, eventType})>0;
        }
        catch(Exception ex){Console.WriteLine($"TableSheduleRepository.Add: {ex.Message}"); return false;}
    }
    public bool RemoveByOrder(int idOrder){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="DELETE FROM TableShedule WHERE id_order=@idOrder";
            return connection.Execute(sql, new {idOrder})>0;
        }
        catch(Exception ex){Console.WriteLine($"TableSheduleRepository.RemoveByOrder: {ex.Message}"); return false;}
    }
    public bool RemoveByEvent(string eventType){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="DELETE FROM TableShedule WHERE event_type=@eventType";
            return connection.Execute(sql, new {eventType})>0;
        }
        catch(Exception ex){Console.WriteLine($"TableSheduleRepository.RemoveByEvent: {ex.Message}"); return false;}
    }
}