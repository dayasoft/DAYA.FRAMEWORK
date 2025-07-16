using System.Threading.Tasks;

namespace DAYA.Cloud.Framework.V2.Domain;

public abstract class Entity
{
    protected static async Task CheckRuleAsync(IBusinessRule rule)
    {
        if (await rule.IsBrokenAsync())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }
}