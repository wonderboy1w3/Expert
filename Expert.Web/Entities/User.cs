namespace Expert.Web.Entities;

public class User
{
	public long Id { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
	public UserType Type { get; set; }
}
