namespace HospitalManagementSystem.UseCases.Dtos.Hospital;

public class HospitalDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Code { get; set; } = string.Empty;
}
