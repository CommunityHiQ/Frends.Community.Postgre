using System.ComponentModel;

namespace Frends.Community.Postgre.Definitions
{
    /// <summary>
    /// Task options.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Choose if error should be thrown if Task failes.
        /// Otherwise returns Object {Success = false }
        /// </summary>
        [DefaultValue(true)]
        public bool ThrowErrorOnFailure { get; set; }
    }
}
