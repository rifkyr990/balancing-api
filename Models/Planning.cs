namespace BalancingApi.Models;

public class Planning
{
    public int PlanningId { get; set; }
    public string RequestCode { get; set; } = "";
    public string CandidateToken { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "SUCCESS";

    public List<PlanningSlot> Slots { get; set; } = new List<PlanningSlot>();
}