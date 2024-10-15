using EmployeeCrudApi.Data;
using EmployeeCrudApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace EmployeeCrudApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<List<Employee>> GetAll()
        {
            return await _context.Employees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is required");
            }

            if (string.IsNullOrWhiteSpace(employee.Name))
            {
                return BadRequest("El nombre no puede estar vacío o compuesto solo de espacios.");
            }

            if (employee.Name.Length > 100)
            {
                return BadRequest("El nombre y apellido deben tener una longitud máxima de 100 caracteres.");
            }

            if (employee.Name.Length < 2)
            {
                return BadRequest("El nombre debe tener al menos dos caracteres.");
            }

            if (Regex.IsMatch(employee.Name, @"\d"))
            {
                return BadRequest("El nombre no debe contener números.");
            }

            var nameParts = employee.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in nameParts)
            {
                if (part.Length < 2)
                {
                    return BadRequest("Cada parte del nombre debe tener al menos dos caracteres.");
                }
            }

            employee.CreatedDate = DateTime.Now;
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is required");
            }

            var employeeToUpdate = await _context.Employees.FindAsync(employee.Id);
            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            employeeToUpdate.Name = employee.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employeeToDelete = await _context.Employees.FindAsync(id);
            if (employeeToDelete == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employeeToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
