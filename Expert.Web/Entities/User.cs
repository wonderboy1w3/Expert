namespace Expert.Web.Entities;

public class User
{
	public long Id { get; set; }
	public string FirtsName { get; set; }
	public string LastName { get; set; }
	public UserType Type { get; set; }
}
