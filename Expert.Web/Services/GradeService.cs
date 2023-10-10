using Expert.Web.Data;
using Expert.Web.DTOs;
using Expert.Web.Entities;
using Expert.Web.Exceptions;
using Expert.Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Expert.Web.Services;

public class GradeService : IGradeService
{
	private readonly AppDbContext appDbContext;
	public GradeService(AppDbContext appDbContext)
	{
		this.appDbContext = appDbContext;
	}

	public async ValueTask<GradeResultDto> CreateAsync(GradeCreationDto dto)
	{
		var grade = await appDbContext.Grades
			.FirstOrDefaultAsync(x => x.GetterId == dto.GetterId && x.SetterId == dto.SetterId);

		if (grade is not null)
			throw new CustomException(401, "You already set grade to this user");
		
		var createdGrade = (await this.appDbContext.Grades.AddAsync(new Grade
		{
			GetterId = dto.GetterId,
			SetterId = dto.SetterId,
			Score = dto.Score,
		})).Entity;

		await this.appDbContext.SaveChangesAsync();


		var getter = await this.appDbContext.Users.FirstOrDefaultAsync(user => user.Id.Equals(createdGrade.GetterId));
		var setter = await this.appDbContext.Users.FirstOrDefaultAsync(user => user.Id.Equals(createdGrade.SetterId));

		var mappedGetter = new UserResultDto
		{
			Id = getter.Id,
			FirstName = getter.FirstName,
			LastName = getter.LastName,
			UserName = getter.UserName
		};
		var mappedSetter = new UserResultDto
		{
			Id = setter.Id,
			FirstName = setter.FirstName,
			LastName = setter.LastName,
			UserName = setter.UserName
		};

		return new GradeResultDto
		{
			Score = createdGrade.Score,
			Setter = mappedSetter,
			Getter = mappedGetter
		};
	}

	public async ValueTask<bool> DeleteAsync(long id)
	{
		var grade = await this.appDbContext.Grades.FirstOrDefaultAsync(grade => grade.Id.Equals(id))
			?? throw new CustomException(404, "Grade is not found");

		this.appDbContext.Grades.Remove(grade);
		await this.appDbContext.SaveChangesAsync();
		return true;
	}

	public async ValueTask<IEnumerable<GradeResultDto>> GetAllAsync()
	{
		var grades = await this.appDbContext.Grades.ToListAsync();
	
		var result = new List<GradeResultDto>();
        foreach (var grade in grades)
        {
			var getter = await this.appDbContext.Users.FirstOrDefaultAsync(x => x.Id == grade.GetterId);
			var setter = await this.appDbContext.Users.FirstOrDefaultAsync(x => x.Id == grade.SetterId);
			var mappedGetter = new UserResultDto
			{
				Id = getter.Id,
				FirstName = getter.FirstName,
				LastName = getter.LastName,
				UserName = getter.UserName
			};
			
			var mappedSetter = new UserResultDto
			{
				Id = setter.Id,
				FirstName = setter.FirstName,
				LastName = setter.LastName,
				UserName = setter.UserName
			};
			
			result.Add(new GradeResultDto
			{
				Getter = mappedGetter,
				Setter = mappedSetter,
				Score = grade.Score
			});
        }
		
		return result;
    }
	
	public async ValueTask<IEnumerable<GradeResultDto>> GetAllAsync(long userId)
	{
		var grades = this.appDbContext.Grades
			.Where(x => x.GetterId == userId).GroupBy(grade => grade.GetterId);
	
		var result = new List<GradeResultDto>();
		foreach (var grade in grades)
		{
			var getter = await this.appDbContext.Users.FirstOrDefaultAsync(user => user.Id.Equals(grade.Key));
			var mappedGetter = new UserResultDto
			{
				Id = getter.Id,
				FirstName = getter.FirstName,
				LastName = getter.LastName,
				UserName = getter.UserName
			};
			
			result.Add(new GradeResultDto
			{
				Getter = mappedGetter,
				Score = grade.Average(g => g.Score)
			});
		}
		
		return result;
	}

	public async ValueTask<IEnumerable<(long UserId, int Average)>> GetAllAverageAsync()
	{
		List<(long UserId, int Average)> result = new ();
		var users = await appDbContext.Users.ToListAsync();

		foreach (var user in users)
		{
			var grades = appDbContext.Grades.Where(x => x.GetterId == user.Id);
			if(grades.Count() > 0) 
				result.Add((user.Id, (int)Math.Round(grades.Average(x => x.Score))));
			else
				result.Add((user.Id, 0));
		}

		return result;
	}
	
	public async ValueTask<int> GetAverageAsync(long userId)
	{
		var grades = appDbContext.Grades.Where(x => x.GetterId == userId);
		if(grades.Count() > 0) 
			return (int)Math.Round(grades.Average(x => x.Score));
		else
			return 0;
	}
	
	public async ValueTask<GradeResultDto> GetAsync(long userId)
	{
		var grades = this.appDbContext.Grades.Where(grade => grade.GetterId.Equals(userId));
		var getter = await this.appDbContext.Users.FirstOrDefaultAsync(user => user.Id.Equals(userId));
		var mappedGetter = new UserResultDto
		{
			Id = getter.Id,
			FirstName = getter.FirstName,
			LastName = getter.LastName,
			UserName = getter.UserName
		};
		
		return new GradeResultDto
		{
			Getter = mappedGetter,
			Score = grades.Average(g => g.Score)
		};
    }
}