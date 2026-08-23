using AIITSM.Application._02_M2_IncidentManagement;
using AIITSM.Domain._02_M2_IncidentManagement;
using AIITSM.Infrastructure._02_M2_IncidentManagement;
using AIITSM.Infrastructure._06_M6_AI;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AIITSM.Tests
{
    public class IncidentServiceTests
    {
        // Each test gets its own isolated in-memory DB so tests never
        // interfere with each other.
        private static AIITSMDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AIITSMDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AIITSMDbContext(options);

            context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Network" });
            context.Categories.Add(new Category { CategoryId = 2, CategoryName = "Hardware" });
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task CreateIncidentAsync_SetsStatusToOpen()
        {
            using var context = CreateContext();
            var service = new IncidentService(context);

            var request = new CreateIncidentRequest
            {
                Title = "Cannot connect to VPN",
                Description = "VPN client fails to authenticate.",
                CategoryId = 1,
                Priority = IncidentPriority.High
            };

            var incidentId = await service.CreateIncidentAsync(request, currentUserId: 42);

            var saved = await context.Incidents.FindAsync(incidentId);

            Assert.NotNull(saved);
            Assert.Equal(IncidentStatus.Open, saved!.Status);
        }

        [Fact]
        public async Task CreateIncidentAsync_SetsCreatedByToLoggedInEmployee()
        {
            using var context = CreateContext();
            var service = new IncidentService(context);

            var request = new CreateIncidentRequest
            {
                Title = "Laptop won't boot",
                Description = "Screen stays black after power on.",
                CategoryId = 2,
                Priority = IncidentPriority.Critical
            };

            var incidentId = await service.CreateIncidentAsync(request, currentUserId: 7);

            var saved = await context.Incidents.FindAsync(incidentId);

            Assert.NotNull(saved);
            Assert.Equal(7, saved!.CreatedBy);
        }

        [Fact]
        public async Task GetMyIncidentsAsync_OnlyReturnsIncidentsForThatUser()
        {
            using var context = CreateContext();
            var service = new IncidentService(context);

            await service.CreateIncidentAsync(
                new CreateIncidentRequest { Title = "User 1 issue", Description = "d", CategoryId = 1, Priority = IncidentPriority.Low },
                currentUserId: 1);

            await service.CreateIncidentAsync(
                new CreateIncidentRequest { Title = "User 2 issue", Description = "d", CategoryId = 1, Priority = IncidentPriority.Low },
                currentUserId: 2);

            var myIncidents = await service.GetMyIncidentsAsync(currentUserId: 1);

            Assert.Single(myIncidents);
            Assert.Equal("User 1 issue", myIncidents[0].Title);
        }

        [Fact]
        public async Task CreateIncidentAsync_IncidentNumberIsFormattedFromIncidentId()
        {
            using var context = CreateContext();
            var service = new IncidentService(context);

            var incidentId = await service.CreateIncidentAsync(
                new CreateIncidentRequest { Title = "Printer offline", Description = "d", CategoryId = 1, Priority = IncidentPriority.Medium },
                currentUserId: 1);

            var details = await service.GetIncidentDetailsAsync(incidentId);

            Assert.NotNull(details);
            Assert.Equal($"INC-{incidentId:D6}", details!.IncidentNumber);
        }

        [Fact]
        public async Task GetIncidentDetailsAsync_ReturnsNull_WhenIncidentDoesNotExist()
        {
            using var context = CreateContext();
            var service = new IncidentService(context);

            var details = await service.GetIncidentDetailsAsync(999);

            Assert.Null(details);
        }
    }
}
