using FlightPlanner.Context;
using FlightPlanner.Models;
using FlightPlanner.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner.Controllers;

[Route("admin-api/flights")]
[ApiController, Authorize]
public class AdminApiController : ControllerBase
{
    private readonly FlightPlannerDbContext _context;

    public AdminApiController(FlightPlannerDbContext context)
    {
        _context = context;
    }
    
    [Route("{id:int}")]
    [HttpGet]
    public IActionResult GetFlights(int id)
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
    
    [HttpPut]
    public IActionResult PutFlights(Flight flight)
    {
        if (AdminApiValidator.HasInvalidValues(flight) || 
            AdminApiValidator.IsWrongDate(flight) || 
            AdminApiValidator.IsSameAirport(flight))
        {
            return  BadRequest();
        }

        if (_context.Flights.Any(f => f.From.City == flight.From.City &&
                                      f.From.Country == flight.From.Country &&
                                      f.From.AirportCode == flight.From.AirportCode &&
                                      f.To.City == flight.To.City &&
                                      f.To.Country == flight.To.Country &&
                                      f.To.AirportCode == flight.To.AirportCode &&
                                      f.Carrier == flight.Carrier &&
                                      f.ArrivalTime == flight.ArrivalTime &&
                                      f.DepartureTime == flight.DepartureTime))
        {
            return Conflict();
        }

        _context.Flights.Add(flight);
        _context.SaveChanges();
        return Created( "", flight);
    }
    
    [Route("{id:int}")]
    [HttpDelete]
    public IActionResult DeleteFlights(int id)
    {
        var flight = _context.Flights.FirstOrDefault(flight => flight.Id == id);

        if (flight == null)
        {
            return Ok();
        }

        _context.Flights.Remove(flight);
        _context.SaveChanges();

        if (flight == null)
        {
            return NotFound();
        }

        return Ok(flight);
    }
}