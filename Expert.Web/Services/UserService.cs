using Expert.Web.Data;
using Expert.Web.DTOs;
using Expert.Web.Entities;
using Expert.Web.Exceptions;
using Expert.Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Expert.Web.Services;

public class UserService : IUserService
{
	private readonly AppDbContext appDbContext;
    public UserService(AppDbContext appDbContext)
    {
		this.appDbContext = appDbContext;
    }

    public async ValueTask<UserResultDto> CreateAsync(UserCreationDto dto)
	{
		var createdUser = (await this.appDbContext.Users.AddAsync(new User
		{
			FirtsName = dto.FirtsName,
			LastName = dto.LastName,
			Type = dto.Type,
		})).Entity;

		await this.appDbContext.SaveChangesAsync();

		return new UserResultDto
		{
			Id = createdUser.Id,
			LastName = createdUser.LastName,
			FirtsName = createdUser.FirtsName
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

	public async ValueTask<IEnumerable<UserResultDto>> GetAllAsync(UserType type)
	{
		var users = await this.appDbContext.Users
			.Where(user => user.Type.Equals(type))
			.ToListAsync();

		var result = new List<UserResultDto>();
        foreach (var user in users)
        {
			result.Add(new UserResultDto
			{
				Id = user.Id,
				FirtsName = user.FirtsName,
				LastName =  user.LastName,
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
			FirtsName = user.FirtsName
		};
	}
}