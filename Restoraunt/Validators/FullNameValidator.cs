using System.Linq;

namespace Validators;
public class FullNameValidator{
    public bool CheckFullName(string fullName){
        if(string.IsNullOrWhiteSpace(fullName)) return false;
        fullName=fullName.Trim();
        return fullName.All(c=>char.IsLetter(c) || char.IsWhiteSpace(c));
    }
}