using System.Diagnostics.CodeAnalysis;

namespace UKHO.MaritimeSafetyInformation.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
