using HospitalManagementSystem.Core.ContributorAggregate;
using Vogen;

namespace HospitalManagementSystem.Infrastructure.Data.Config;

[EfCoreConverter<ContributorId>]
[EfCoreConverter<ContributorName>]
internal partial class VogenEfCoreConverters;
