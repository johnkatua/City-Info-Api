using CityInfoAPI.Entities;
using CityInfoAPI.Models;
using CityInfoAPI.Db;
using Microsoft.EntityFrameworkCore;

namespace CityInfoAPI.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;
        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<City>> GetCitiesAsync() 
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }
        public async Task<City?> GetCityAsync(int cityId,  bool includePointOfInterest)
        {
            if (includePointOfInterest) 
            {
                return await _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointsOfInterest.Where(p => p.CityId == cityId).ToListAsync();
        }
        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }
        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointsOfInterest.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
        }

        public  void AddPointOfInterestForCityAsync(PointOfInterest pointOfInterest) {
          _context.PointsOfInterest.Add(pointOfInterest);
        }
        public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }
        public void DeletePointOfInterest(PointOfInterest pointOfInterest) {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }
        public async Task<bool> SaveChangesAsync() 
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}