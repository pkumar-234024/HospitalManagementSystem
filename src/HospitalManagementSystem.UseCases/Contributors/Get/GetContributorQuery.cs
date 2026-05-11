using HospitalManagementSystem.Core.ContributorAggregate;

namespace HospitalManagementSystem.UseCases.Contributors.Get;

public record GetContributorQuery(ContributorId ContributorId) : IQuery<Result<ContributorDto>>;
