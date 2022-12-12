using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial record ProductDataModel : BaseNopEntityModel
    {
        public string operationId { get; set; }
        public Vehicle vehicle { get; set; }
        public Inquiryresponse inquiryResponse { get; set; }

        public class Vehicle
        {
            public string year { get; set; }
            public string makeTypeId { get; set; }
            public string make { get; set; }
            public string makeText { get; set; }
            public string model { get; set; }
            public string modelText { get; set; }
            public string engine { get; set; }
            public string engineText { get; set; }
        }

        public class Inquiryresponse
        {
            public string[] epnTrackNums { get; set; }
            public Part[] parts { get; set; }
        }

        public class Part
        {
            public int id { get; set; }
            public string type { get; set; }
            public string partGroupId { get; set; }
            public string partTypeId { get; set; }
            public string manufacturerCode { get; set; }
            public string partNumber { get; set; }
            public string description { get; set; }
            public string status { get; set; }
            public string availability { get; set; }
            public int availableQuantity { get; set; }
            public string displayQuantity { get; set; }
            public int packNumUnits { get; set; }
            public int qtyBuyInc { get; set; }
            public int roundedQtyBuyInc { get; set; }
            public string[] miscTexts { get; set; }
            public string otherText { get; set; }
            public string catLongDescription { get; set; }
            public string catShortDescription { get; set; }
            public string years { get; set; }
            public int quantityPerCar { get; set; }
            public string manufacturerName { get; set; }
            public string[] headers { get; set; }
            public object[] unanswered { get; set; }
            public string genericDescription { get; set; }
            public int partialAvailableQuantity { get; set; }
            public bool priceAvailable { get; set; }
            public string c2cUrl { get; set; }
            public string imageUrl { get; set; }
            public object[] alternateSellers { get; set; }
            public string catalogOrderNumber { get; set; }
            public Price price { get; set; }
            public string headerText { get; set; }
        }

        public class Price
        {
            public float unitCost { get; set; }
            public float unitList { get; set; }
            public Corepricing corePricing { get; set; }
        }

        public class Corepricing
        {
            public float unitCost { get; set; }
            public float unitList { get; set; }
        }

    }
}