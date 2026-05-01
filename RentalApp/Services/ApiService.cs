namespace RentalApp.Services;
public class ApiService : IApiService
{
    private int? _userId;
    private string? _email;
    public int? GetCurrentUserId() => _userId;
    public string? GetCurrentUserEmail() => _email;
    public void SetCurrentUser(int userId, string email) { _userId = userId; _email = email; }
    public void ClearCurrentUser() { _userId = null; _email = null; }
    public bool IsLoggedIn() => _userId.HasValue;
}
