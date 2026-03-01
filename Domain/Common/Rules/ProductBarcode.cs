using Domain.Common.Errors;
using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Rules
{
    public class ProductBarcode
    {
        public string Value { get; }

        public ProductBarcode(string? barcode)
        {
            // Early return if barcode is empty or null
            if (string.IsNullOrEmpty(barcode))
                return;

            string value = barcode.Trim();

            if (value.Length < 8 && value.Length > 14)
                throw new DomainRuleViolationException(
                    ProductErrors.BarcodeLenght,
                    "The lenght of the barcode must be between 8-14 characters.");

            if (!value.All(char.IsDigit))
                throw new DomainRuleViolationException(
                    ProductErrors.BarcodeFormat,
                    "The barcode must only consist of numbers.");

            Value = value;
        }

        public override string ToString() => Value;
    }
}
