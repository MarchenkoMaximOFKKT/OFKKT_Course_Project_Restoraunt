using Input;
using Models;
using Repositories;
using Validators;
 
namespace Services;
public class CustomerService{
    private readonly CustomerRepository _customerRepo;
    private readonly PhoneValidator _phoneVal;
    private readonly FullNameValidator _nameVal;
    public CustomerService(CustomerRepository customerRepo, PhoneValidator phoneVal, FullNameValidator nameVal){
        this._customerRepo=customerRepo;
        this._phoneVal=phoneVal;
        this._nameVal=nameVal;
    }
    public (bool Success, Customer? Customer, string Message) Login(string phone){
        if(!this._phoneVal.CheckPhone(phone)) return (false, null, "Невірний формат телефону");
        var customer=this._customerRepo.GetByPhone(phone);
        if(customer==null) return (false, null, "Клієнта з таким телефоном не знайдено. Спочатку зареєструйтесь.");
        return (true, customer, $"Ласкаво просимо, {customer.FullName}");
    }
    public (bool Success, Customer? Customer, string Message) Register(InputCustomer input){
        if(string.IsNullOrWhiteSpace(input.FullName) || input.FullName.Length<2) return (false, null, "Введіть повне ім'я, мінімум 2 символи");
        if(!this._nameVal.CheckFullName(input.FullName)) return (false, null, "ПІБ може містити лише літери та пробіли");
        if(!this._phoneVal.CheckPhone(input.Phone)) return (false, null, "Невірний формат телефону");
        if(this._customerRepo.GetByPhone(input.Phone)!=null) return (false, null, "Клієнт з таким телефоном вже існує. Спробуйте увійти.");
        var newCustomer=new Customer(){FullName=input.FullName, Phone=input.Phone};
        if(!this._customerRepo.Add(newCustomer)) return (false, null, "Помилка при реєстрації. Спробуйте ще раз.");
        var created=this._customerRepo.GetByPhone(input.Phone);
        return (true, created, "Реєстрацію успішно завершено");
    }
    public (bool Success, string Message) Remove(string phone){
        if(!this._phoneVal.CheckPhone(phone)) return (false, "Невірний формат телефону");
        var customer=this._customerRepo.GetByPhone(phone);
        if(customer==null) return (false, "Користувача не існує");
        if(!this._customerRepo.Remove(customer.Id)) return (false, "Не вдалося видалити користувача");
        return (true, "Користувача видалено");
    }
}
