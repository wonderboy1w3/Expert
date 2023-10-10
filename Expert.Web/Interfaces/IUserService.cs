using Expert.Web.DTOs;

namespace Expert.Web.Interfaces;

public interface IUserService
{
	ValueTask<UserResultDto> CreateAsync(UserCreationDto dto);
	ValueTask<bool> DeleteAsync(long id);
	ValueTask<UserResultDto> GetAsync(long id);
	ValueTask<IEnumerable<UserResultDto>> GetAllAsync();
	ValueTask<UserResultDto> CheckAsync(string login, string password);
}