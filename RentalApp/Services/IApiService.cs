namespace RentalApp.Services;
public interface IApiService
{
    int? GetCurrentUserId();
    string? GetCurrentUserEmail();
    void SetCurrentUser(int userId, string email);
    void ClearCurrentUser();
    bool IsLoggedIn();
}
