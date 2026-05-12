using Ardalis.SharedKernel;

namespace HospitalManagementSystem.Core.HospitalAggregate;

public class Hospital : IAggregateRoot
{
  private Hospital() { }

  public Hospital(Guid id, string name, string code)
  {
    Id = id;
    Name = name;
    Code = code;
    IsActive = true;
    CreatedAt = DateTime.UtcNow;
  }

  public Hospital(string name, string code)
  {
    Id = Guid.NewGuid();
    Name = name;
    Code = code;
    IsActive = true;
    CreatedAt = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }
  public string Name { get; private set; } = string.Empty;
  public string Code { get; private set; } = string.Empty;
  public bool IsActive { get; private set; }
  public DateTime CreatedAt { get; private set; }
  public DateTime? UpdatedAt { get; private set; }
}
