using HospitalManagementSystem.Core.ContributorAggregate;

namespace HospitalManagementSystem.UseCases.Contributors;
public record ContributorDto(ContributorId Id, ContributorName Name, PhoneNumber PhoneNumber);
