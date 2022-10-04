using FlightPlanner.Context;
using FlightPlanner.Models;
using FlightPlanner.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers;

[Route("api")]
[ApiController]
public class CustomerApiController : ControllerBase
{
    private readonly FlightPlannerDbContext _context;

    public CustomerApiController(FlightPlannerDbContext context)
    {
        _context = context;
    }
    
    [Route("airports")]
    [HttpGet]
    public IActionResult GetAirport(string search)
    {
        var airports = new List<Airport>();
        var formattedPhrase = search.ToLower().Trim();
        
        foreach (var airport in _context.Airports)
        {
            if (airport.City.ToLower().Contains(formattedPhrase) ||
                airport.Country.ToLower().Contains(formattedPhrase) ||
                airport.AirportCode.ToLower().Contains(formattedPhrase))
            {
                airports.Add(airport);
                return Ok(airports.ToArray());
            }
            if (airport.City.ToLower().Contains(formattedPhrase) ||
                airport.Country.ToLower().Contains(formattedPhrase) ||
                airport.AirportCode.ToLower().Contains(formattedPhrase))
            {
                airports.Add(airport);
                return Ok(airports.ToArray());
            }
        }
        
        return Ok(airports.ToArray());
    }
    
    [Route("flights/search")]
    [HttpPost]
    public IActionResult SearchFlight(FlightSearch flightSearch)
    {
        if (CustomerApiValidator.HasInvalidValues(flightSearch) ||
            CustomerApiValidator.IsSameAirport(flightSearch))
        {
            return BadRequest();
        }
        
        var page = 0;
        var totalItems = 0;
        var items = new List<Flight>();
        
        var flight = _context.Flights
            .Include(flight => flight.From)
            .Include(flight => flight.To)
            .FirstOrDefault(flight => flight.DepartureTime.Substring(0, 10) == flightSearch.DepartureDate &&
                                      flight.From.AirportCode == flightSearch.From &&
                                      flight.To.AirportCode == flightSearch.To);

        if (flight != null)
        {
            items.Add(flight);
            totalItems++;
            page++;
        }

        var pageResult = new PageResult(page, totalItems, items.ToArray());
        return Ok(pageResult);
    }
    
    [Route("flights/{id:int}")]
    [HttpGet]
    public IActionResult GetFlight(int id)
    {
        var flight = _context.Flights
            .Include(flight => flight.From)
            .Include(flight => flight.To)
            .FirstOrDefault(flight => flight.Id == id);
        
        if (flight == null)
        {
            return NotFound();
        }
        
        return Ok(flight);
    }
}