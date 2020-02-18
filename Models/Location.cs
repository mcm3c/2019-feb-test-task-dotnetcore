using System.Collections.Generic;

namespace nib.Models {
  public class Location {
    public int ID { get; set; }
    public string Name { get; set; }
    public string State { get; set; }
    public List<Job> Jobs { get; set; } = new List<Job>();
  }
}
