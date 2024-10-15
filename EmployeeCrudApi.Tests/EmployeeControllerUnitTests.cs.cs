using EmployeeCrudApi.Controllers;
using EmployeeCrudApi.Data;
using EmployeeCrudApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeCrudApi.Tests
{
    public class EmployeeControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_ForEmptyName()
        {
            var context = GetInMemoryDbContext();
            var controller = new EmployeeController(context);
            var newEmployee = new Employee { Id = 7, Name = "" };

            var result = await controller.Create(newEmployee);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El nombre no puede estar vacío o compuesto solo de espacios.", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_ForNameWithSpacesOnly()
        {
            var context = GetInMemoryDbContext();
            var controller = new EmployeeController(context);
            var newEmployee = new Employee { Id = 8, Name = "   " };

            var result = await controller.Create(newEmployee);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El nombre no puede estar vacío o compuesto solo de espacios.", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_ForNameLongerThan100Characters()
        {
            var context = GetInMemoryDbContext();
            var controller = new EmployeeController(context);
            var newEmployee = new Employee { Id = 9, Name = new string('A', 101) };

            var result = await controller.Create(newEmployee);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El nombre y apellido deben tener una longitud máxima de 100 caracteres.", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_ForNameShorterThan2Characters()
        {
            var context = GetInMemoryDbContext();
            var controller = new EmployeeController(context);
            var newEmployee = new Employee { Id = 10, Name = "J" };

            var result = await controller.Create(newEmployee);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El nombre debe tener al menos dos caracteres.", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_ForNameContainingNumbers()
        {
            var context = GetInMemoryDbContext();
            var controller = new EmployeeController(context);
            var newEmployee = new Employee { Id = 11, Name = "John123 Doe" };

            var result = await controller.Create(newEmployee);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("El nombre no debe contener números.", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_ForNamePartShorterThan2Characters()
        {
            var context = GetInMemoryDbContext();
            var controller = new EmployeeController(context);
            var newEmployee = new Employee { Id = 12, Name = "J Doe" };

            var result = await controller.Create(newEmployee);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cada parte del nombre debe tener al menos dos caracteres.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfEmployees()
        {
            var context = GetInMemoryDbContext();
            context.Employees.AddRange(
                new Employee { Id = 1, Name = "John Doe" },
                new Employee { Id = 2, Name = "Jane Doe" }
            );
            context.SaveChanges();

            var controller = new EmployeeController(context);

            var result = await controller.GetAll();

            Assert.Equal(2, result.Count);
            Assert.Equal("John Doe", result[0].Name);
            Assert.Equal("Jane Doe", result[1].Name);
        }

        [Fact]
        public async Task GetById_ReturnsEmployeeById()
        {
            var context = GetInMemoryDbContext();
            context.Employees.Add(new Employee { Id = 1, Name = "John Doe" });
            context.SaveChanges();

            var controller = new EmployeeController(context);

            var result = await controller.GetById(1);

            Assert.NotNull(result);
            Assert.IsType<Employee>(result.Value);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal("John Doe", result.Value.Name);
        }

        [Fact]
        public async Task Create_AddsEmployee()
        {
            var context = GetInMemoryDbContext();
            var controller = new EmployeeController(context);

            var newEmployee = new Employee { Id = 3, Name = "New Employee" };

            await controller.Create(newEmployee);

            var employee = await context.Employees.FindAsync(3);
            Assert.NotNull(employee);
            Assert.Equal("New Employee", employee.Name);
        }

        [Fact]
        public async Task Update_UpdatesEmployee()
        {
            var context = GetInMemoryDbContext();
            var existingEmployee = new Employee { Id = 1, Name = "Old Name" };
            context.Employees.Add(existingEmployee);
            context.SaveChanges();

            var controller = new EmployeeController(context);

            var updatedEmployee = new Employee { Id = 1, Name = "Updated Name" };

            await controller.Update(updatedEmployee);

            var employee = await context.Employees.FindAsync(1);
            Assert.NotNull(employee);
            Assert.Equal("Updated Name", employee.Name);
        }

        [Fact]
        public async Task Delete_RemovesEmployee()
        {
            var context = GetInMemoryDbContext();
            var employeeToDelete = new Employee { Id = 1, Name = "John Doe" };
            context.Employees.Add(employeeToDelete);
            context.SaveChanges();

            var controller = new EmployeeController(context);

            await controller.Delete(1);

            var employee = await context.Employees.FindAsync(1);
            Assert.Null(employee);
        }
    }
}
