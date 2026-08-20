using BalancingApi.Controllers;
using BalancingApi.Data;
using BalancingApi.Dto;
using BalancingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BalancingApi.Tests;

public class PlanningApiTest
{
    public static async Task RunApiTest(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Console.WriteLine("Testing API Flow...");

        var service = new BalancingService();
        var controller = new PlanningController(db, service);

        var testRequestCode = "test-api-code";

        var req = new CreatePlanningRequest
        {
            RequestCode = testRequestCode,
            CandidateToken = "VEH-RIFKY",
            Slots = new List<SlotItemDto>
            {
                new SlotItemDto { SlotName = "hari 1", Quantity = 4 },
                new SlotItemDto { SlotName = "hari 2", Quantity = 2 }
            }
        };

        var actionResult = await controller.CreatePlanning(req);
        var dataDB = await db.Plannings.Include(p => p.Slots).FirstOrDefaultAsync(p => p.RequestCode == testRequestCode);

        if (dataDB == null)
        {
            throw new Exception("Gagal: Data tidak tersimpan di database!");
        }

        if (dataDB.Slots.Count != 2)
        {
            throw new Exception("Gagal: Jumlah slot di database tidak sesuai!");
        }

        Console.WriteLine("Test API Flow Berasil");
    }
}