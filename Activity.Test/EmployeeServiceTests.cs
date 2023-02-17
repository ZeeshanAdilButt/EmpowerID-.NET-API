using ActivityApp.Application.Core.ApplicationContracts.Requests.Employee;
using ActivityApp.Application.Core.ApplicationContracts.Requests.Example;
using ActivityApp.Application.Core.ApplicationContracts.Responses.Example;
using ActivityApp.Domain.Data;
using ActivityApp.Infrastructure.SQL.Repostiories.Implementations;
using ActivityApp.Infrastructure.SQL.Repostiories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Linq.Expressions;

namespace ActivityApp.Application.Implementations.Tests
{
    public class EmployeeServiceTests
    {
        private readonly IEmployeeService _employeeService;
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;

        public EmployeeServiceTests()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            
            var configurationMock = new Mock<IConfiguration>();

            _employeeRepositoryMock = new Mock<IEmployeeRepository>();

            _employeeService = new EmployeeService(_employeeRepositoryMock.Object, httpContextAccessorMock.Object, configurationMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsSuccessfulResponse()
        {
            // Arrange
            var request = new CreateEmployeeRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                DateOfBirth = new DateTime(1980, 1, 1),
                Department = "IT",
            };

            _employeeRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Employee>()))
                .Returns(Task.FromResult((int)1));

            // Act
            var response = await _employeeService.CreateAsync(request);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Null(response.OriginalException);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_WithValidRequest_ReturnsSuccessfulResponse()
        {
            // Arrange
            var request = new DeleteEmployeeRequest { Id = 1 };

            _employeeRepositoryMock.Setup(x => x.DeleteEmployeeAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            // Act
            var response = await _employeeService.DeleteEmployeeAsync(request);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Null(response.OriginalException);
        }

        [Fact]
        public async Task GetEmployeeAsync_WithValidRequest_ReturnsSuccessfulResponse()
        {
            // Arrange
            var request = new GetEmployeeRequest { Id = 1 };
            var employee = new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                DateOfBirth = new DateTime(1980, 1, 1),
                Department = "IT",
            };

            _employeeRepositoryMock.Setup(x => x.GetEmployeeAsync(It.IsAny<int>()))
                .ReturnsAsync(employee);

            // Act
            var response = await _employeeService.GetEmployeeAsync(request);

            // Assert
            Assert.Equal(1, response.Id);
            Assert.Equal("John", response.FirstName);
            Assert.Equal("Doe", response.LastName);
            Assert.Equal("john.doe@example.com", response.Email);
            Assert.Equal(new DateTime(1980, 1, 1), response.DateOfBirth);
            Assert.Equal("IT", response.Department);
            Assert.Null(response.OriginalException);
        }

        [Fact]
        public async Task GetAllEmployeeAsync_ReturnsAllEmployees()
        {
            // Arrange
            var searchItem = "John";
            var employee1 = new Employee { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(1990, 1, 1), Department = "IT" };
            var employee2 = new Employee { Id = 2, FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", DateOfBirth = new DateTime(1995, 1, 1), Department = "HR" };
            var employees = new List<Employee> { employee1, employee2 };
            _employeeRepositoryMock.Setup(x => x.GetAllEmployeeAsync(searchItem)).ReturnsAsync(employees);

            // Act
            var request = new GetAllEmployeeRequest { searchItem = searchItem };
            var result = await _employeeService.GetAllEmployeeAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal(employee1.Id, result.Data[0].Id);
            Assert.Equal(employee1.FirstName, result.Data[0].FirstName);
            Assert.Equal(employee1.LastName, result.Data[0].LastName);
            Assert.Equal(employee1.Email, result.Data[0].Email);
            Assert.Equal(employee1.DateOfBirth, result.Data[0].DateOfBirth);
            Assert.Equal(employee1.Department, result.Data[0].Department);
            Assert.Equal(employee2.Id, result.Data[1].Id);
            Assert.Equal(employee2.FirstName, result.Data[1].FirstName);
            Assert.Equal(employee2.LastName, result.Data[1].LastName);
            Assert.Equal(employee2.Email, result.Data[1].Email);
            Assert.Equal(employee2.DateOfBirth, result.Data[1].DateOfBirth);
            Assert.Equal(employee2.Department, result.Data[1].Department);
        }

        [Fact]
        public async Task GetAllEmployeeAsync_ReturnsErrorResponseWhenRepositoryThrowsException()
        {
            // Arrange
            var searchItem = "John";
            _employeeRepositoryMock.Setup(x => x.GetAllEmployeeAsync(searchItem)).ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var request = new GetAllEmployeeRequest { searchItem = searchItem };
            var result = await _employeeService.GetAllEmployeeAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEmployee_WhenValidEmployeeIdAndModelProvided()
        {
           
            
            var employeeToUpdate = new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var updatedEmployeeModelVM = new UpdateEmployeeRequest
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com"
            };

            var updatedEmployeeModel = new Employee
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com"
            };
            _employeeRepositoryMock.Setup(r => r.GetEmployeeAsync(employeeToUpdate.Id)).ReturnsAsync(employeeToUpdate);

            _employeeRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Employee>())).ReturnsAsync(updatedEmployeeModel);
            
            // Act
            await _employeeService.UpdateAsync(updatedEmployeeModelVM);

            // Assert
            _employeeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Once);
            Assert.NotEqual(updatedEmployeeModel.FirstName, employeeToUpdate.FirstName);
            Assert.Equal(updatedEmployeeModel.LastName, employeeToUpdate.LastName);
            Assert.NotEqual(updatedEmployeeModel.Email, employeeToUpdate.Email);
        }

    }
}