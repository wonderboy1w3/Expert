using Expert.Web.Entities;

namespace Expert.Web.DTOs;

public class UserResultDto
{
	public long Id { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string UserName { get; set; }
	public List<GradeResultDto> Grades { get; set; }
}
