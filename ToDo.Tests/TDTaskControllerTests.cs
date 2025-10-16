using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;
using System.Net;
using ToDo.API.Controllers;
using ToDo.DataAccess.Repositories.IRepository;
using ToDo.Models.DTOs;
using ToDo.Models.Models;

namespace ToDo.Tests
{
    public class TDTaskControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ITDTaskRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly TDTaskController _controller;
        public TDTaskControllerTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<ITDTaskRepository>();
            _mockMapper = new Mock<IMapper>();

            _mockUow.Setup(u => u.TDTask).Returns(_mockRepo.Object);

            _controller = new TDTaskController(_mockUow.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllTDTasks_ReturnsOk_WithListOfTasks()
        {
            // Arrange
            var tasks = new List<TDTask>
            {
                new() { Id = 1, Title = "Task 1" },
                new() { Id = 2, Title = "Task 2" }
            };

            _mockRepo.Setup(r => r.GetAll(null, null))
                     .ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetAllTDTasks();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult.Value as APIResponse;
            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.OK);
            ((IEnumerable<TDTask>)response.Result!).Should().HaveCount(2);
        }

        [Fact]
        public async Task GetSpecificTDTask_ReturnsOk_WhenFound()
        {
            // Arrange
            var task = new TDTask { Id = 1, Title = "Test Task" };

            _mockRepo.Setup(r => r.Get(It.IsAny<Expression<Func<TDTask, bool>>>(), null, false))
                     .ReturnsAsync(task);

            // Act
            var result = await _controller.GetSpecificTDTask(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult!.Value as APIResponse;
            response!.IsSuccess.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            ((TDTask)response.Result!).Id.Should().Be(1);
        }

        [Fact]
        public async Task GetSpecificTDTask_ReturnsBadRequest_WhenIdIsZero()
        {
            // Act
            var result = await _controller.GetSpecificTDTask(0);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();

            var response = badRequest!.Value as APIResponse;
            response!.IsSuccess.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetSpecificTDTask_ReturnsNotFound_WhenNoTask()
        {
            // Arrange
            _mockRepo.Setup(r => r.Get(It.IsAny<Expression<Func<TDTask, bool>>>(), null, false))
                     .ReturnsAsync((TDTask?)null);

            // Act
            var result = await _controller.GetSpecificTDTask(99);

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            notFound.Should().NotBeNull();

            var response = notFound!.Value as APIResponse;
            response!.IsSuccess.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpsertTDTask_CreatesNew_WhenIdIsZero()
        {
            // Arrange
            var dto = new TDTaskDTO { Id = 0, Title = "New Task" };
            var task = new TDTask { Id = 0, Title = "New Task" };

            _mockMapper.Setup(m => m.Map<TDTask>(dto)).Returns(task);
            _mockRepo.Setup(r => r.Add(It.IsAny<TDTask>())).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpsertTDTask(dto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var response = okResult!.Value as APIResponse;
            response!.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Result.Should().Be(task);
        }

        [Fact]
        public async Task UpsertTDTask_Updates_WhenIdIsNotZero()
        {
            // Arrange
            var dto = new TDTaskDTO { Id = 5, Title = "Updated Task" };
            var task = new TDTask { Id = 5, Title = "Updated Task" };

            _mockMapper.Setup(m => m.Map<TDTask>(dto)).Returns(task);
            _mockUow.Setup(u => u.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpsertTDTask(dto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var response = okResult!.Value as APIResponse;
            response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteTDTask_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            _mockRepo.Setup(r => r.Get(It.IsAny<Expression<Func<TDTask, bool>>>(), null, false))
                     .ReturnsAsync((TDTask?)null);

            // Act
            var result = await _controller.DeleteTDTask(1);

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            notFound.Should().NotBeNull();

            var response = notFound!.Value as APIResponse;
            response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteTDTask_ReturnsOk_WhenDeleted()
        {
            // Arrange
            var task = new TDTask { Id = 1 };
            _mockRepo.Setup(r => r.Get(It.IsAny<Expression<Func<TDTask, bool>>>(), null, false))
                     .ReturnsAsync(task);

            _mockUow.Setup(u => u.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTDTask(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var response = okResult!.Value as APIResponse;
            response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateTDTaskPercentage_ReturnsOk_WhenUpdated()
        {
            // Arrange
            var dto = new UpdatePercentageDTO { Id = 1, Percentage = 80 };
            var task = new TDTask { Id = 1, Title = "Test", CompletionPercentage = 20, Status = "InProgress" };

            _mockRepo.Setup(r => r.Get(It.IsAny<Expression<Func<TDTask, bool>>>(), null, false))
                     .ReturnsAsync(task);

            _mockUow.Setup(u => u.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateTDTaskPercentage(dto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult!.Value as APIResponse;
            response.Should().NotBeNull();
            response!.IsSuccess.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            task.CompletionPercentage.Should().Be(80);
            task.Status.Should().Be("InProgress");
        }

        [Fact]
        public async Task UpdateTDTaskPercentage_SetsStatusToDone_WhenPercentageIs100()
        {
            // Arrange
            var dto = new UpdatePercentageDTO { Id = 1, Percentage = 100 };
            var task = new TDTask { Id = 1, Title = "Test", CompletionPercentage = 50, Status = "InProgress" };

            _mockRepo.Setup(r => r.Get(It.IsAny<Expression<Func<TDTask, bool>>>(), null, false))
                     .ReturnsAsync(task);
            _mockUow.Setup(u => u.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateTDTaskPercentage(dto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var response = okResult!.Value as APIResponse;

            response!.StatusCode.Should().Be(HttpStatusCode.NoContent);
            task.Status.Should().Be("Done");
            task.CompletionPercentage.Should().Be(100);
        }

        [Fact]
        public async Task UpdateTDTaskPercentage_ReturnsNotFound_WhenTaskMissing()
        {
            // Arrange
            var dto = new UpdatePercentageDTO { Id = 99, Percentage = 50 };

            _mockRepo.Setup(r => r.Get(It.IsAny<Expression<Func<TDTask, bool>>>(), null, false))
                     .ReturnsAsync((TDTask?)null);

            // Act
            var result = await _controller.UpdateTDTaskPercentage(dto);

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            notFound.Should().NotBeNull();

            var response = notFound!.Value as APIResponse;
            response!.IsSuccess.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task MarkTDTaskAsDone_ReturnsOk_WhenSuccessfullyMarked()
        {
            // Arrange
            var task = new TDTask { Id = 1, Title = "Do Stuff", Status = "InProgress", CompletionPercentage = 70 };

            _mockRepo.Setup(r => r.Get(It.IsAny<Expression<Func<TDTask, bool>>>(), null, false))
                     .ReturnsAsync(task);
            _mockUow.Setup(u => u.Save()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.MarkTDTaskAsDone(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();

            var response = okResult!.Value as APIResponse;
            response!.StatusCode.Should().Be(HttpStatusCode.NoContent);

            task.Status.Should().Be("Done");
            task.CompletionPercentage.Should().Be(100);
        }

        [Fact]
        public async Task MarkTDTaskAsDone_ReturnsBadRequest_WhenAlreadyDone()
        {
            // Arrange
            var task = new TDTask { Id = 1, Title = "Already Done", Status = "Done", CompletionPercentage = 100 };

            _mockRepo.Setup(r => r.Get(It.IsAny<Expression<Func<TDTask, bool>>>(), null, false))
                     .ReturnsAsync(task);

            // Act
            var result = await _controller.MarkTDTaskAsDone(1);

            // Assert
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();

            var response = badRequest!.Value as APIResponse;
            response!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.ErrorMessages.Should().ContainSingle(m => m.Contains("already marked as done"));
        }

        [Fact]
        public async Task MarkTDTaskAsDone_ReturnsNotFound_WhenTaskMissing()
        {
            // Arrange
            _mockRepo.Setup(r => r.Get(It.IsAny<Expression<Func<TDTask, bool>>>(), null, false))
                     .ReturnsAsync((TDTask?)null);

            // Act
            var result = await _controller.MarkTDTaskAsDone(99);

            // Assert
            var notFound = result.Result as NotFoundObjectResult;
            notFound.Should().NotBeNull();

            var response = notFound!.Value as APIResponse;
            response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}
