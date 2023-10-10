using Expert.Web.Data;
using Expert.Web.DTOs;
using Expert.Web.Entities;
using Expert.Web.Exceptions;
using Expert.Web.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Expert.Web.Services;

public class UserService : IUserService
{
	private readonly AppDbContext appDbContext;
	private readonly IGradeService gradeService;
	
    public UserService(AppDbContext appDbContext, IGradeService gradeService)
    {
	    this.appDbContext = appDbContext;
	    this.gradeService = gradeService;
    }

    public async ValueTask<UserResultDto> CreateAsync(UserCreationDto dto)
    {
	    var user = await appDbContext.Users.FirstOrDefaultAsync(x => x.UserName == dto.UserName.ToLower().Trim());

	    if (user is not null)
		    throw new CustomException(401, "This user already exist");
	    
		var createdUser = (await this.appDbContext.Users.AddAsync(new User
		{
			FirstName = dto.FirstName,
			LastName = dto.LastName,
			UserName = dto.UserName.ToLower().Trim(),
			Password = dto.Password
		})).Entity;

		await this.appDbContext.SaveChangesAsync();

		return new UserResultDto
		{
			Id = createdUser.Id,
			LastName = createdUser.LastName,
			FirstName = createdUser.FirstName,
			UserName = dto.UserName,
			Score = await gradeService.GetAverageAsync(createdUser.Id)
		};
	}

	public async ValueTask<bool> DeleteAsync(long id)
	{
		var user = await this.appDbContext.Users.FirstOrDefaultAsync(user => user.Id.Equals(id))
			?? throw new CustomException(404, "User is not found");

		this.appDbContext.Users.Remove(user);
		await this.appDbContext.SaveChangesAsync();
		return true;
	}

	public async ValueTask<IEnumerable<UserResultDto>> GetAllAsync()
	{
		var users = await this.appDbContext.Users
			.ToListAsync();

		var result = new List<UserResultDto>();
        foreach (var user in users)
        {
			result.Add(new UserResultDto
			{
				Id = user.Id,
				FirstName = user.FirstName,
				LastName =  user.LastName,
				UserName = user.UserName,
				Score = await gradeService.GetAverageAsync(user.Id)
			});
        }

		return result;
    }

	public async ValueTask<UserResultDto> GetAsync(long id)
	{
		var user = await this.appDbContext.Users.FirstOrDefaultAsync(user => user.Id.Equals(id))
			?? throw new CustomException(404, "User is not found");

		return new UserResultDto
		{
			Id = user.Id,
			LastName = user.LastName,
			FirstName = user.FirstName,
			UserName = user.UserName,
			Score = await gradeService.GetAverageAsync(user.Id)
		};
	}

	public async ValueTask<UserResultDto> CheckAsync(string login, string password)
	{
		if (appDbContext.Users.Count() > 0)
		{
			var user = await this.appDbContext.Users.FirstOrDefaultAsync(user => user.UserName.Equals(login.ToLower()))
				       ?? null;
			
			if(user.Password == password)
				return new UserResultDto
				{
					Id = user.Id,
					LastName = user.LastName,
					FirstName = user.FirstName,
					UserName = user.UserName,
					Score = await gradeService.GetAverageAsync(user.Id)
				};
		}
		return null;
	}
}