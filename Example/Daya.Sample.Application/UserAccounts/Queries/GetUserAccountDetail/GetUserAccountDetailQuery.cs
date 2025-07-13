using DAYA.Cloud.Framework.V2.Application.Contracts;

public record GetUserAccountDetailQuery(Guid UserId) : Query<UserAccountDetailQueryDto>();