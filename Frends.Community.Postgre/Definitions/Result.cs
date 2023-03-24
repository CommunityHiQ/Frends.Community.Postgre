using System.ComponentModel.DataAnnotations;

namespace Frends.Community.Postgre.Definitions
{
    /// <summary>
    /// Return object
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Boolean value for successful opration.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Possible error message.
        /// </summary>
        [UIHint(nameof(Success), "", false)]
        public string Message { get; set; }
    }
}
