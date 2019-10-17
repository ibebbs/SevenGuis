using Bebbs.Monads;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cells.Common.Spreadsheet.Content
{
    public static class Evaluator
    {
        private static readonly IEnumerable<Func<string, Option<object>>> Evaluators = new Func<string, Option<object>>[]
        {
            EvaluateInt32,
            EvaluateDecimal,
            EvaluateFloat,
            EvaluateDouble,
            EvaluateDateTime,
        };

        private static Option<object> EvaluateInt32(string text)
        {
            return Int32.TryParse(text, out int value) 
                ? Option<object>.Some(value) 
                : Option<object>.None;
        }

        private static Option<object> EvaluateDecimal(string text)
        {
            return Decimal.TryParse(text, out decimal value)
                ? Option<object>.Some(value)
                : Option<object>.None;
        }

        private static Option<object> EvaluateFloat(string text)
        {
            return Single.TryParse(text, out float value)
                ? Option<object>.Some(value)
                : Option<object>.None;
        }

        private static Option<object> EvaluateDouble(string text)
        {
            return Double.TryParse(text, out double value)
                ? Option<object>.Some(value)
                : Option<object>.None;
        }

        private static Option<object> EvaluateDateTime(string text)
        {
            return DateTime.TryParse(text, out DateTime value)
                ? Option<object>.Some(value)
                : Option<object>.None;
        }

        public static object Evaluate(string text)
        {
            return Evaluators
                .Select(evaluator => evaluator(text))
                .Collect()
                .FirstOption()
                .Coalesce(() => text);
        }
    }
}
