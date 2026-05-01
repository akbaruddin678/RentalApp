namespace RentalApp.Services;
public interface ILocationService
{
    Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync();
    double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
}
