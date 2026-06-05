using Npgsql;
using Dapper;
using Models;
using StringToConnect;
using System;
 
namespace Repositories;
public class BookingRepository{
    private readonly string _cs=DBconf.dbConnectionString;
    public Booking? Get(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Booking WHERE id=@id";
            return connection.QueryFirstOrDefault<Booking>(sql, new {id});
        }
        catch(Exception ex){Console.WriteLine($"BookingRepository.Get: {ex.Message}"); return null;}
    }
    public Booking? GetByCustomer(int idCustomer, DateTime bookingStartTime){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="SELECT * FROM Booking WHERE id_customer=@idCustomer AND DATE(booking_start_time)=DATE(@bookingStartTime)";
            return connection.QueryFirstOrDefault<Booking>(sql, new {idCustomer, bookingStartTime});
        }
        catch(Exception ex){Console.WriteLine($"BookingRepository.GetByCustomer: {ex.Message}"); return null;}
    }
    public bool Add(int idCustomer, DateTime bookingStartTime, DateTime bookingEndTime){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql=@"
                INSERT INTO Booking (id_customer, booking_start_time, booking_end_time) 
                VALUES (@idCustomer, @bookingStartTime, @bookingEndTime)";
            return connection.Execute(sql, new {idCustomer, bookingStartTime, bookingEndTime})>0;
        }
        catch(Exception ex){Console.WriteLine($"BookingRepository.Add: {ex.Message}"); return false;}
    }
    public bool Remove(int id){
        try{
            using var connection=new NpgsqlConnection(_cs);
            connection.Open();
            string sql="DELETE FROM Booking WHERE id=@id";
            return connection.Execute(sql, new {id})>0;
        }
        catch(Exception ex){Console.WriteLine($"BookingRepository.Remove: {ex.Message}"); return false;}
    }
}