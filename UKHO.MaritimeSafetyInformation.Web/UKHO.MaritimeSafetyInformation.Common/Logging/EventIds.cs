using Microsoft.Extensions.Logging;

namespace UKHO.MaritimeSafetyInformation.Common.Logging
{
    public enum EventIds
    {
        /// <summary>
        /// 700001 - An unhandled exception occurred while processing the request.
        /// </summary>
        UnhandledControllerException = 700001,
        /// <summary>
        /// 700002 - Maritime safety information request started.
        /// </summary>
        Start = 700002
    }

    public static class EventIdExtensions
    {
        public static EventId ToEventId(this EventIds eventId)
        {
            return new EventId((int)eventId, eventId.ToString());
        }
    }
}
