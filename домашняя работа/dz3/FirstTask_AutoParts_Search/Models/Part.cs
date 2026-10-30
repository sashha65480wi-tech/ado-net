namespace FirstTask_AutoParts_Search.Models;
public class Part
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Article { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
}