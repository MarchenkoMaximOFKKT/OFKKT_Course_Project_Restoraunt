using System.Linq;

namespace Validators;
public class PhoneValidator{
    public bool CheckPhone(string phone){
        if(string.IsNullOrWhiteSpace(phone)) return false;
        phone=phone.Trim();
        if(phone.StartsWith("+")) phone=phone.Substring(1);
        return phone.Length>=7 && phone.Length<=15 && !phone.Any(
            c=>char.IsWhiteSpace(c) || char.IsLetter(c) || char.IsSymbol(c));
    }
}