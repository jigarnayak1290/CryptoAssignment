using System.ComponentModel.DataAnnotations;

namespace UserEnquiry.Models
{
    /// <summary>
    /// User information
    /// </summary>
    /// <param name="UserId">User Id in Int</param>
    /// <param name="Balance">Balance in int</param>
    public record UserInfo(
        [property: Key] int? UserId = null,
        int? Balance = null);
}
