using System;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{

    [XmlType("User")]
    public class ExportUserSoldProductDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }


        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlArray("soldProducts")]
        public UserProductDto[] SoldProducts { get; set; }
    }

    [XmlType("Product")]
    public class UserProductDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
//<User>
//    <firstName>Almire</firstName>
//    <lastName>Ainslee</lastName>
//    <soldProducts>
//      <Product>
//        <name>olio activ mouthwash</name>
//        <price>206.06</price>
//      </Product>
//      <Product>
