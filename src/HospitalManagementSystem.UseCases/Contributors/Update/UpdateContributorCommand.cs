using HospitalManagementSystem.Core.ContributorAggregate;

namespace HospitalManagementSystem.UseCases.Contributors.Update;

public record UpdateContributorCommand(ContributorId ContributorId, ContributorName NewName) : ICommand<Result<ContributorDto>>;
