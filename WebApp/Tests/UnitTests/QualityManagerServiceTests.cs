using AutoMapper;
using Infrastructure.Models;
using Models.Enum;
using Models.Messaging;
using Models.Request;
using Models.Response;
using Moq;
using Repositories.Interface;
using Services.Implementation;
using Services.Interface;
using System.Text.Json;
using Xunit;

namespace Tests.UnitTests
{
    public class QualityManagerServiceTests
    {
        private readonly Mock<IFoodAnalaysisRepository> _foodRepositoryMock;
        private readonly Mock<IMessagingBus> _messagingBusMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly QualityManagerService _qualityManagerService;

        public QualityManagerServiceTests()
        {
            _foodRepositoryMock = new Mock<IFoodAnalaysisRepository>();
            _messagingBusMock = new Mock<IMessagingBus>();
            _mapperMock = new Mock<IMapper>();

            _qualityManagerService = new QualityManagerService(
                _foodRepositoryMock.Object,
                _mapperMock.Object,
                _messagingBusMock.Object
            );
        }

        [Fact]
        public async Task Process_ShouldPublishMessageAndSaveNewEntity()
        {
            // Arrange
            var processFoodRequest = new ProcessFoodRequest
            {
                Name = "Test Food",
                SerialNumber = "12345",
                TypeOfAnalysis = (int)AnalysisType.CHEMICAL_ANALAYSIS
            };

            var foodAnalysisEntity = new FoodAnalysis
            {
                Name = processFoodRequest.Name,
                SerialNumber = processFoodRequest.SerialNumber,
                TypeOfAnalysis = (int)AnalysisType.CHEMICAL_ANALAYSIS,
                CreateDate = DateTime.Now,
                State = (int)AnalysisState.IN_PROGRESS
            };

            var foodAnalysisResponse = new ProcessFoodResponse
            {
                Name = processFoodRequest.Name,
                SerialNumber = processFoodRequest.SerialNumber
            };

            _mapperMock.Setup(m => m.Map<FoodAnalysis>(It.IsAny<ProcessFoodRequest>()))
                .Returns(foodAnalysisEntity);

            _mapperMock.Setup(m => m.Map<ProcessFoodResponse>(It.IsAny<FoodAnalysis>()))
                .Returns(foodAnalysisResponse);

            _foodRepositoryMock.Setup(repo => repo.Add(It.IsAny<FoodAnalysis>()))
                .ReturnsAsync(foodAnalysisEntity);

            var expectedMessage = JsonSerializer.Serialize(new FoodAnalysisMessageRequest
            {
                FoodName = processFoodRequest.Name,
                SerialNumber = processFoodRequest.SerialNumber,
                AnalysisType = processFoodRequest.TypeOfAnalysis
            });

            // Act
            var result = await _qualityManagerService.Process(processFoodRequest);

            // Assert
            _messagingBusMock.Verify(mb => mb.Publish("food_analysing_create", expectedMessage), Times.Once);
            _foodRepositoryMock.Verify(repo => repo.Add(It.IsAny<FoodAnalysis>()), Times.Once);
            Assert.Equal(processFoodRequest.Name, result.Name);
            Assert.Equal(processFoodRequest.SerialNumber, result.SerialNumber);
        }
    }
}