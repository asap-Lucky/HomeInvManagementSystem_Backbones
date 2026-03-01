using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Errors
{
    public static class ProductErrors
    {
        public const string NameRequired = "PRODUCT.NAME.REQUIRED";
        public const string NameLength = "PRODUCT.NAME.LENGTH";
        public const string IdRequired = "PRODUCT.ID.REQUIRED";
        public const string IdFormat = "PRODUCT.ID.FORMAT";
        public const string CategoryRequired = "PRODUCT.CATEGORY.REQUIRED";
        public const string BarcodeFormat = "PRODUCT.BARCODE.FORMAT";
        public const string BarcodeLenght = "PRODUCT.BARCODE.LENGHT";
    }
}
