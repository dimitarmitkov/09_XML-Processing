using System;
using System.Xml.Serialization;

namespace CarDealer.DTO.ExportDTO
{
    [XmlType("customer")]
    public class ExportTotalSalesCustomersDto
    {
        [XmlAttribute("full-name")]
        public string FullName { get; set; }

        [XmlAttribute("bought-cars")]
        public int BoughtCars { get; set; }

        [XmlAttribute("spent-money")]
        public decimal SpentMoney { get; set; }
    }
}
//<customer full-name="Hai Everton" bought-cars="1" spent-money="2544.67" />