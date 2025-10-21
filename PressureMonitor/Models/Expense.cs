using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PressureMonitor.Models;

public class Expense
{
    
    [Key]
    [DatabaseGenerated(databaseGeneratedOption: DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

}