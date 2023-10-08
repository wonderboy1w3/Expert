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
			FirtsName = getter.FirtsName,
			LastName = getter.LastName
		};
		var mappedSetter = new UserResultDto
		{
			Id = getter.Id,
			FirtsName = getter.FirtsName,
			LastName = getter.LastName
		};

		return new GradeResultDto
		{
			Score = createdGrade.Score,
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
		var grades = this.appDbContext.Grades.GroupBy(grade => grade.GetterId);

		var result = new List<GradeResultDto>();
        foreach (var grade in grades)
        {
			var getter = await this.appDbContext.Users.FirstOrDefaultAsync(user => user.Id.Equals(grade.Key));
			var mappedGetter = new UserResultDto
			{
				Id = getter.Id,
				FirtsName = getter.FirtsName,
				LastName = getter.LastName
			};

			result.Add(new GradeResultDto
			{
				Getter = mappedGetter,
				Score = grade.Average(g => g.Score)
			});
        }
		
		return result;
    }

	public async ValueTask<GradeResultDto> GetAsync(long userId)
	{
		var grades = this.appDbContext.Grades.Where(grade => grade.GetterId.Equals(userId));
		var getter = await this.appDbContext.Users.FirstOrDefaultAsync(user => user.Id.Equals(userId));
		var mappedGetter = new UserResultDto
		{
			Id = getter.Id,
			FirtsName = getter.FirtsName,
			LastName = getter.LastName
		};

		return new GradeResultDto
		{
			Getter = mappedGetter,
			Score = grades.Average(g => g.Score)
		};
    }
}