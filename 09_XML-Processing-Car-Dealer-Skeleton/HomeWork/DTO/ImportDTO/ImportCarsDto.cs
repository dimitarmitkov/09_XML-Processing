﻿using System;
using System.Xml.Serialization;

namespace CarDealer.DTO.ImportDTO
{
    [XmlType("Car")]
    public class ImportCarsDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TraveledDistance { get; set; }

        [XmlArray("parts")]
        public ImportCarsPartsDto[] Parts { get; set; }

    }

    [XmlType("partId")]
    public class ImportCarsPartsDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
//<Car>
//    <make>Opel</make>
//    <model>Omega</model>
//    <TraveledDistance>176664996</TraveledDistance>
//    <parts>
//      <partId id = "38" />
//      < partId id="102"/>
//      <partId id = "23" />
//      < partId id="116"/>
//      <partId id = "46" />
//      < partId id="68"/>
//      <partId id = "88" />
//      < partId id="104"/>
//      <partId id = "71" />
//      < partId id="32"/>
//      <partId id = "114" />
//    </ parts >
//  </ Car >