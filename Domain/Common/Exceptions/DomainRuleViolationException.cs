using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Exceptions
{
    public class DomainRuleViolationException : Exception
    {
        public string RuleCode { get; }

        public DomainRuleViolationException(string ruleCode, string message)
            : base(message)
        {
            RuleCode = ruleCode;
        }
    }

    // Take condition of statement, and return rulecode and message if true.
    public static class Guard
    {
        public static void Against(bool condition, string ruleCode, string message)
        {
            if (condition) throw new DomainRuleViolationException(ruleCode, message);
        }
    }
}
