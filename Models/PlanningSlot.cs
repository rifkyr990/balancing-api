namespace BalancingApi.Models;

public class PlanningSlot
{
    public int Id { get; set; }
    public int PlanningId { get; set; }
    public int SlotOrder { get; set; }
    public string SlotName { get; set; } = "";
    public int OriginalQuantity { get; set; }
    public int BalancedQuantity { get; set; }
    public bool IsActive { get; set; }
}