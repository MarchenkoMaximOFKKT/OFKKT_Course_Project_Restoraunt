using System;
using System.Collections.Generic;
using System.Linq;
using Repositories;
using Models;
using Validators;

namespace Services;
public class TableService{
    private readonly TableRepository _tableRepo;
    private readonly TableSheduleRepository _tableSheduleRepo;
    public TableService(TableRepository tableRepo, TableSheduleRepository tableSheduleRepo){
        this._tableRepo=tableRepo;
        this._tableSheduleRepo=tableSheduleRepo;
    }
    public List<Table> GetAll()=>this._tableRepo.GetAll();
    public List<Table> GetFreeForTime(DateTime startTime, DateTime endTime){
        var allTables=this._tableRepo.GetAll();
        var busyIds=this._tableSheduleRepo
            .GetOverlaps(allTables.Select(t=>t.Id).ToList(), startTime, endTime)
            .Select(ts=>ts.IdTable);
        return allTables.Where(t=>!busyIds.Contains(t.Id)).OrderBy(t=>t.Id).ToList();
    }
    public (bool Success, string Message) OcuppateTable(List<int> idTables, DateTime startTime, DateTime endTime, string eventType, int? idOrder=null, int? idBooking=null){
        if(idTables.Count()==0) return (false, "Оберіть хочаб один стіл");
        var conflicts=this._tableSheduleRepo.GetOverlaps(idTables, startTime, endTime);
        if(conflicts.Any()) return (false, "Обрані "+(idTables.Count>1 ? "столи вже зайняті" : "стіл вже зайнятий")+" на цей час");
        bool isAdded=this._tableSheduleRepo.Add(idTables, startTime, endTime, idOrder, idBooking, eventType);
        if(!isAdded){
            if(idOrder==null && idBooking==null) return (false, "Проблеми з додаванням обслуговування");
            return (false, "Проблеми з додаванням "+(idBooking==null ? "замовлення" : "броні"));
        }
        if(startTime<=DateTime.Now.AddMinutes(15) && endTime>=DateTime.Now){
            if(!this._tableRepo.UpdateStatus(idTables, "Зайнятий")) return (false, "Помилка при спробі зайняти "+(idTables.Count>1 ? "столи" : "стіл"));
        }
        return (true, (idTables.Count>1 ? "Столи" : "Стіл")+" успішно зайнято");
    }
    public (bool Success, string Message) FreeTable(string? eventType=null, int? idOrder=null){
        if(idOrder!=null){
            var tables=this._tableRepo.GetByOrder(idOrder.Value);
            if(tables.Any()) this._tableRepo.UpdateStatus(tables.Select(t=>t.Id).ToList(), "Вільний");
            if(!this._tableSheduleRepo.RemoveByOrder(idOrder.Value)) return (false, "Помилка при звільненні столів замовлення");
        }else{
            string target=eventType??"Обслуговування";
            if(!this._tableSheduleRepo.RemoveByEvent(target)) return (false, "Помилка при звільненні столів");
        }
        return (true, "Столи звільнено успішно");
    }
}