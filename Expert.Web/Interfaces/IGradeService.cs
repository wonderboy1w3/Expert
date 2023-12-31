﻿using Expert.Web.DTOs;

namespace Expert.Web.Interfaces;

public interface IGradeService
{
	ValueTask<GradeResultDto> CreateAsync(GradeCreationDto dto);
	ValueTask<bool> DeleteAsync(long id);
	ValueTask<GradeResultDto> GetAsync(long userId);
	ValueTask<IEnumerable<GradeResultDto>> GetAllAsync();
	ValueTask<IEnumerable<GradeResultDto>> GetAllAsync(long userId);
	ValueTask<int> GetAverageAsync(long userId);
	ValueTask<IEnumerable<(long UserId, int Average)>> GetAllAverageAsync();
}
