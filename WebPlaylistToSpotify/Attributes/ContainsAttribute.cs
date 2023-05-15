using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebPlaylistToSpotify.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ContainsAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var collection = value as IList;
            if (collection != null)
            {
                return collection.Count > 0;
            }
            return false;
        }
    }
}
