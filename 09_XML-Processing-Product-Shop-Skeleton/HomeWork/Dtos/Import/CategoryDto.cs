using System;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Import
{

    [XmlType("Category")]
    public class CategoryDto
    {
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
