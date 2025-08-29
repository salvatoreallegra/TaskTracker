using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Options;
using TaskTracker.Api.Abstractions;
using TaskTracker.Api.Models;
using TaskTracker.Api.Services;
using TaskTracker.Api.Options;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Mapping;

namespace TaskTracker.Tests
{
    public class TaskServiceTests
    {
        private readonly IMapper _mapper;

        public TaskServiceTests()
        {
            // Create real AutoMapper config (using profile)
            var config = new MapperConfiguration(cfg => cfg.AddProfile<TaskProfile>());
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnReadDto()
        {
            // Arrange
            var repo = new Mock<ITaskRepository>();
            var options = Options.Create(new AppOptions { DefaultPageSize = 10, MaxPageSize = 100 });
            var service = new TaskService(repo.Object, options, _mapper);

            var dto = new TaskCreateDto { Title = "Test Task" };

            repo.Setup(r => r.AddAsync(It.IsAny<TaskItem>(), default)).Returns(Task.CompletedTask);
            repo.Setup(r => r.SaveChangesAsync(default)).Returns(Task.CompletedTask);

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.Equal("Test Task", result.Title);
            repo.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), default), Times.Once);
            repo.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenIdsMismatch()
        {
            // Arrange
            var repo = new Mock<ITaskRepository>();
            var options = Options.Create(new AppOptions { DefaultPageSize = 10, MaxPageSize = 100 });
            var service = new TaskService(repo.Object, options, _mapper);

            var dto = new TaskUpdateDto { Id = 2, Title = "Mismatch" };

            // Act
            var result = await service.UpdateAsync(1, dto);

            // Assert
            Assert.False(result);
            repo.VerifyNoOtherCalls();
        }
    }
}
