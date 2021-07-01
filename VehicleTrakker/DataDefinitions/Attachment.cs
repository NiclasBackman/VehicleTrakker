using System;

namespace VehicleTrakker.DataDefinitions
{
    public class Attachment
    {
        public Attachment()
        {
        }

        public Guid EventId { get; set; }

        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string SourceFileName { get; set; }
    }
}
