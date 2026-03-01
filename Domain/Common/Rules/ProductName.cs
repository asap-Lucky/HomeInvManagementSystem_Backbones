using Domain.Common.Errors;
using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Rules
{
    public class ProductName
    {
        public string Value { get; }

        public ProductName(string name)
        {
            var value = name?.Trim();

            if (string.IsNullOrEmpty(value))
                throw new DomainRuleViolationException(
                    ProductErrors.NameRequired,
                    "Product name is required.");

            if (value.Length < 2 || value.Length > 120)
                throw new DomainRuleViolationException(
                    ProductErrors.NameLength,
                    "Product name must be between 2 and 120 characters.");

            Value = value;
        }

        public override string ToString() => Value;
    }
}
