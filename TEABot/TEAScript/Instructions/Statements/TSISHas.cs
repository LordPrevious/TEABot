﻿using System;
using TEABot.TEAScript.Attributes;

namespace TEABot.TEAScript.Instructions.Statements
{
    /// <summary>
    /// Check if a value has been set
    /// </summary>
    [TSKeyword("has")]
    public class TSISHas : TSIStatement
    {
        /// <summary>
        /// Name of the target variable to store the check result in
        /// </summary>
        private string mTargetName = String.Empty;

        /// <summary>
        /// The value to check for
        /// </summary>
        private ITSValueArgument mSourceValue = new TSConstantNumberArgument(0L);

        protected override bool Parse(string a_instructionArguments)
        {
            if ((!SplitValueArguments(a_instructionArguments, out ITSValueArgument[] valueArguments))
                || (!(new TSValidator(ParsingBroadcaster).ContainsEnoughArguments(valueArguments, 2)))
                || (!VariableNameValueArgument(valueArguments[0], out mTargetName)))
            {
                return false;
            }
            mSourceValue = valueArguments[1];
            return true;
        }

        public override ITSControlFlow Execute(TSExecutionContext a_context)
        {
            a_context.Values[mTargetName] = mSourceValue.HasValue(a_context.Values);
            return TSFlow.Next;
        }
    }
}
