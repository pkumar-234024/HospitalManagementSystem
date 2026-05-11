using HospitalManagementSystem.Core.ContributorAggregate;

namespace HospitalManagementSystem.UseCases.Contributors.Delete;

public record DeleteContributorCommand(ContributorId ContributorId) : ICommand<Result>;
