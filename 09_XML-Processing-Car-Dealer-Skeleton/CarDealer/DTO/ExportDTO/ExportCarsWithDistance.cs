using System;
using System.Xml.Serialization;

namespace CarDealer.DTO.ExportDTO
{
    [XmlType("car")]
    public class ExportCarsWithDistance
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}