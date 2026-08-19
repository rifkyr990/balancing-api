namespace BalancingApi.Dto;

public class CreatePlanningRequest
{
    public string RequestCode { get; set; } = "";
    public string CandidateToken { get; set; } = "";
    public List<SlotItemDto> Slots { get; set; } = new List<SlotItemDto>();
}

public class SlotItemDto
{
    public string SlotName { get; set; } = "";
    public int Quantity { get; set; }
}