using Domain.Common.Errors;
using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Rules
{
    public sealed record ProductId
    {
        public string Value { get; }

        public ProductId(string id)
        {
            string value = id.Trim();

            if (string.IsNullOrEmpty(value))
                throw new DomainRuleViolationException(
                    ProductErrors.IdRequired,
                    "The product ID must be set");

            if (!value.All(char.IsDigit))
                throw new DomainRuleViolationException(
                    ProductErrors.IdFormat,
                    "The id must only consist of numbers.");

            Value = value;
        }

        public override string ToString() => Value;
    }
}
