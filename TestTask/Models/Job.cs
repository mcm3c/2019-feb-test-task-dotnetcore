using System;
namespace TestTask.Models {
  public class Job {
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateAdded { get; set; }
    public int LocationID { get; set; }
  }
}
