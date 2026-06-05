using System; 

namespace Validators;
public class DateTimeValidator{
    public bool CheckDateTime(DateTime startTime, DateTime endTime){
        if(startTime<DateTime.Now.AddMinutes(-5)) return false;
        if(endTime<=startTime) return false;
        bool correctStart=startTime.Hour>=10 && startTime.Hour<22;
        bool correctEnd=endTime.Hour>=10 && endTime.Hour<22 || (endTime.Hour==22 && endTime.Minute==0);
        return correctStart && correctEnd;
    }
}