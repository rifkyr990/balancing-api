using BalancingApi.Data;
using BalancingApi.Dto;
using BalancingApi.Models;
using BalancingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BalancingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanningController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly BalancingService _service;

    public PlanningController(AppDbContext context, BalancingService service)
    {
        _context = context;
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlanning([FromBody] CreatePlanningRequest req)
    {

        if (string.IsNullOrEmpty(req.RequestCode))
        {
            return BadRequest(new { message = "RequestCode wajib diisi" });
        }

        if (req.Slots == null || req.Slots.Count == 0)
        {
            return BadRequest(new { message = "Daftar slot tidak boleh kosong" });
        }

        // Cek idempotency biar RequestCode duplikat ga nambah data baru
        var cekData = await _context.Plannings
            .Include(p => p.Slots)
            .FirstOrDefaultAsync(p => p.RequestCode == req.RequestCode);

        if (cekData != null)
        {
            return Ok(cekData);
        }

        // Cek angka negatif
        for (int i = 0; i < req.Slots.Count; i++)
        {
            if (req.Slots[i].Quantity < 0)
            {
                return BadRequest(new { message = $"Jumlah slot '{req.Slots[i].SlotName}' tidak boleh negatif" });
            }
        }

        // Pindahin quantity ke array biasa buat dihitung
        int[] dataAwal = new int[req.Slots.Count];
        for (int i = 0; i < req.Slots.Count; i++)
        {
            dataAwal[i] = req.Slots[i].Quantity;
        }

        // Jalankan logika balancing dari Case 1
        int[] hasilBalancing = _service.Balance(dataAwal);

        // Buat data planning baru
        var newPlanning = new Planning
        {
            RequestCode = req.RequestCode,
            CandidateToken = string.IsNullOrEmpty(req.CandidateToken) ? "VEH-MIRANDA" : req.CandidateToken,
            CreatedAt = DateTime.UtcNow,
            Status = "SUCCESS"
        };

        for (int i = 0; i < req.Slots.Count; i++)
        {
            var item = req.Slots[i];

            var slotDetail = new PlanningSlot
            {
                SlotOrder = i + 1,
                SlotName = string.IsNullOrEmpty(item.SlotName) ? $"Slot {i + 1}" : item.SlotName,
                OriginalQuantity = item.Quantity,
                BalancedQuantity = hasilBalancing[i],
                IsActive = item.Quantity > 0
            };

            newPlanning.Slots.Add(slotDetail);
        }

        // Simpan ke DB
        _context.Plannings.Add(newPlanning);
        await _context.SaveChangesAsync();

        return Ok(newPlanning);
    }

    [HttpGet]
    public async Task<IActionResult> GetHistory()
    {
        var listData = await _context.Plannings
            .Include(p => p.Slots)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(listData);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var detail = await _context.Plannings
            .Include(p => p.Slots)
            .FirstOrDefaultAsync(p => p.PlanningId == id);

        if (detail == null)
        {
            return NotFound(new { message = "Data planning tidak ditemukan" });
        }

        return Ok(detail);
    }
}