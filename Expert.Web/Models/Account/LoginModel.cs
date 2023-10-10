using System.ComponentModel.DataAnnotations;

namespace Expert.Web.Models.Account;

public class LoginModel
{
    public LoginModel()
    {
    }

    public LoginModel(string? password, string? userName)
    {
        Password = password;
        UserName = userName;
    }
    public string? Password { get; set; }

    public string? UserName { get; set; }

    public string ErrorMessage { get; set; }
}