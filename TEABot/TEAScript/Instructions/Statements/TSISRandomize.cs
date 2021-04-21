﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Copy instruction to copy a value to a variable
    /// </summary>
    [TSKeyword("randomize")]
    public class TSISRandomize : TSIStatement
    {
        /// <summary>
        /// A randomizer shared across all instances
        /// </summary>
        private static readonly Random sRandomizer = new Random();

        /// <summary>
        /// Name of the value to randomize
        /// </summary>
        private string mValueName = String.Empty;

        /// <summary>
        /// One bound of the range from which to pick a number
        /// </summary>
        private ITSValueArgument mFirstLimit = new TSConstantValueArgument(0L);

        /// <summary>
        /// The other bound of the range from which to pick a number
        /// </summary>
        private ITSValueArgument mSecondLimit = new TSConstantValueArgument(0L);

        protected override bool Parse(string a_instructionArguments)
        {
            ITSValueArgument[] valueArguments;
            if (!SplitValueArguments(a_instructionArguments, out valueArguments)) return false;

            var validator = new TSValidator(ParsingBroadcaster);
            if (!validator.ContainsEnoughArguments(valueArguments, 3)) return false;

            if (!VariableNameValueArgument(valueArguments[0], out mValueName)) return false;

            mFirstLimit = valueArguments[1];
            mSecondLimit = valueArguments[2];
            return true;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            // get current values
            long firstLimit = mFirstLimit.GetValue(a_context);
            long secondLimit = mSecondLimit.GetValue(a_context);
            // get upper and lower limit
            long lowerLimit;
            long upperLimit;
            if (firstLimit > secondLimit)
            {
                lowerLimit = secondLimit;
                upperLimit = firstLimit;
            }
            else
            {
                lowerLimit = firstLimit;
                upperLimit = secondLimit;
            }
            // get random value within these limits without modulo bias
            byte[] buffer = new byte[8];
            ulong randomizerRange = (ulong)(upperLimit - lowerLimit) + 1; // map range to start at 0, include both bounds
            ulong randomValue;
            do
            {
                sRandomizer.NextBytes(buffer);
                randomValue = (ulong)BitConverter.ToInt64(buffer, 0);
            }
            while (randomValue > (ulong.MaxValue - (ulong.MaxValue % randomizerRange)));
            a_context.Values[mValueName] = (long)(randomValue % randomizerRange) + lowerLimit;
            // proceed
            return TSFlow.Next;
        }
    }
}
