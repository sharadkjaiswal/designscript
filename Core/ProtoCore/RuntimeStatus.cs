using System;
using System.Collections.Generic;
using ProtoCore.DSASM;
using ProtoCore.Utils;

namespace ProtoCore
{
    namespace RuntimeData
    {
        public enum WarningID
        {
            kDefault,
            kAccessViolation,
            kAmbiguousMethodDispatch,
            kAurgumentIsNotExpected,
            kCallingConstructorOnInstance,
            kConversionNotPossible,
            kDereferencingNonPointer,
            kFileNotExist,
            kIndexOutOfRange,
            kInvalidRecursion,
            kInvalidArguments,
            kCyclicDependency,
            kMethodResolutionFailure,
            kOverIndexing,
            kTypeConvertionCauseInfoLoss,
            kTypeMismatch,
            kReplicationWarning
        }

        public struct WarningMessage
        {
            public const string kArrayOverIndexed = "Variable is over indexed.";
            public const string kIndexOutOfRange = "Index is out of range.";
            public const string kSymbolOverIndexed = "'{0}' is over indexed.";
            public const string kStringOverIndexed = "String is over indexed.";
            public const string kStringIndexOutOfRange = "The index to string is out of range";
            public const string kAssignNonCharacterToString = "Only character can be assigned to a position in a string.";
            public const string KCallingConstructorOnInstance = "Cannot call constructor '{0}()' on instance.";
            public const string kInvokeMethodOnInvalidObject = "Method '{0}()' is invoked on invalid object.";
            public const string kMethodStackOverflow = "Stack overflow caused by calling method '{0}()' recursively.";
            public const string kCyclicDependency = "Cyclic dependency detected at '{0}' and '{1}'.";
            public const string kFFIFailedToObtainThisObject = "Failed to obtain this object for '{0}.{1}'.";
            public const string kFFIFailedToObtainObject = "Failed to obtain object '{0}' for '{1}.{2}'.";
            public const string kFFIInvalidCast = "'{0}' is being cast to '{1}', but the allowed range is [{2}..{3}].";
            public const string kDeferencingNonPointer = "Deferencing a non-pointer.";
            public const string kFailToConverToPointer = "Converting other things to pointer is not allowed.";
            public const string kFailToConverToNull = "Converting other things to null is not allowed.";
            public const string kFailToConverToFunction = "Converting other things to function pointer is not allowed.";
            public const string kConvertDoubleToInt = "Converting double to int will cause possible information loss.";
            public const string kArrayRankReduction = "Type conversion would cause array rank reduction. This is not permitted outside of replication. {511ED65F-FB66-4709-BDDA-DCD5E053B87F}";
            public const string kConvertArrayToNonArray = "Converting an array to {0} would cause array rank reduction and is not permitted.";
            public const string kConvertNonConvertibleTypes = "Asked to convert non-convertible types.";
            public const string kFunctionNotFound = "No candidate function could be found.";
            public const string kAmbigousMethodDispatch = "Candidate function could not be located on final replicated dispatch GUARD {FDD1F347-A9A6-43FB-A08E-A5E793EC3920}.";
            public const string kInvalidArguments = "Argument is invalid.";
            public const string kInvalidArgumentsInRangeExpression = "The value that used in range expression should be either interger or double.";
            public const string kFileNotFound = "'{0}' doesn't exist.";
            public const string kPropertyNotFound = "Object does not have a property '{0}'.";
            public const string kPropertyOfClassNotFound = "Class '{0}' does not have a property '{1}'.";
            public const string kPropertyInaccessible = "Property '{0}' is inaccessible.";
            public const string kMethodResolutionFailure = "Method resolution failure on: {0}() - 0CD069F4-6C8A-42B6-86B1-B5C17072751B.";
            public const string kMethodResolutionFailureForOperator = "Operator '{0}' cannot be applied to operands of type '{1}' and '{2}'.";
        }

        public struct WarningEntry
        {
            public RuntimeData.WarningID id;
            public string message;
            public int Line;
            public int Col;
            public string Filename;
        }
    }

    public class RuntimeStatus
    {
        private ProtoCore.Core core;
        private bool warningAsError;
        private System.IO.TextWriter output = System.Console.Out;
        private readonly List<RuntimeData.WarningEntry> warnings;

        public IOutputStream MessageHandler { get; set; }
        public IOutputStream WebMsgHandler { get; set; }
        public List<RuntimeData.WarningEntry> Warnings { get { return warnings; } }
        //public CodeModel.CodePoint CodePoint { get; private set; }

        public RuntimeStatus(Core core,bool warningAsError = false, System.IO.TextWriter writer = null)
        {
            warnings = new List<RuntimeData.WarningEntry>();
            this.warningAsError = warningAsError;
            this.core = core;
            if (core.Options.WebRunner)
            {
                this.WebMsgHandler = new WebOutputStream(core);
            }
            if (writer != null)
            {
                output = System.Console.Out;
                System.Console.SetOut(writer);
            }
        }

        public void LogWarning(RuntimeData.WarningID id, string msg, string path, int line, int col)
        {
            string filename = string.IsNullOrEmpty(path) ? string.Empty : path;
            if (string.IsNullOrEmpty(filename) || line == -1 || col == -1)
            {
                ProtoCore.CodeGen.AuditCodeLocation(core, ref filename, ref line, ref col);
            }
            OutputMessage outputMsg = new OutputMessage(string.Format("> Runtime warning: {0}\n - \"{1}\" <line: {2}, col: {3}>", msg, filename, line, col));
            System.Console.WriteLine(string.Format("> Runtime warning: {0}\n - \"{1}\" <line: {2}, col: {3}>", msg, filename, line, col));
            if (WebMsgHandler != null)
            {
                WebMsgHandler.Write(outputMsg);
            }
            warnings.Add(new RuntimeData.WarningEntry { id = id, message = msg, Col = col, Line = line, Filename = filename });

            if (core.Options.IsDeltaExecution)
            {
                core.LogErrorInGlobalMap(Core.ErrorType.Warning, msg, filename, line, col, BuildData.WarningID.kDefault, id);
            }

            if (null != MessageHandler)
            {
                OutputMessage.MessageType type = OutputMessage.MessageType.Warning;
                MessageHandler.Write(new OutputMessage(type, msg.Trim(), filename, line, col));
            }
            CodeModel.CodeFile cf = new CodeModel.CodeFile { FilePath = path };

            /*CodePoint = new CodeModel.CodePoint
            {
                SourceLocation = cf,
                LineNo = line,
                CharNo = col
            };*/
        }

        public void LogWarning(RuntimeData.WarningID id, string msg)
        {
            LogWarning(id, msg, string.Empty, -1, -1);
        }


        public void LogWarning(RuntimeData.WarningID id, string msg, int blockID)
        {
            LogWarning(id, msg, string.Empty, -1, -1);
        }


        public bool ContainsWarning(RuntimeData.WarningID warnId)
        {
            foreach (RuntimeData.WarningEntry warn in warnings)
            {
                if (warnId == warn.id)
                    return true;
            }
            return false;
        }

        public void LogMethodResolutionWarning(Core core, string methodName, int classScope = ProtoCore.DSASM.Constants.kInvalidIndex, List<StackValue> arguments = null)
        {
            string message;
            string propertyName;
            Operator op;

            if (CoreUtils.TryGetPropertyName(methodName, out propertyName))
            {
                if (classScope != Constants.kGlobalScope)
                {
                    string classname = core.ClassTable.ClassNodes[classScope].name;
                    message = string.Format(RuntimeData.WarningMessage.kPropertyOfClassNotFound, classname, propertyName);
                }
                else
                {
                    message = string.Format(RuntimeData.WarningMessage.kPropertyNotFound, propertyName);
                }
            }
            else if (CoreUtils.TryGetOperator(methodName, out op))
            {
                string strOp = OpKeywordData.OpSymbolTable[op];
                message = String.Format(RuntimeData.WarningMessage.kMethodResolutionFailureForOperator,
                                        strOp,
                                        core.TypeSystem.GetType((int)arguments[0].metaData.type),
                                        core.TypeSystem.GetType((int)arguments[1].metaData.type));
            }
            else
            {
                message = string.Format(ProtoCore.RuntimeData.WarningMessage.kMethodResolutionFailure, methodName);
            }

            LogWarning(ProtoCore.RuntimeData.WarningID.kMethodResolutionFailure, message);
        }

        public void LogMethodNotAccessibleWarning(Core core, string methodName)
        {
            string message;
            string propertyName;

            if (CoreUtils.TryGetPropertyName(methodName, out propertyName))
            {
                message = String.Format(RuntimeData.WarningMessage.kPropertyInaccessible, propertyName);
            }
            else
            {
                message = String.Format(RuntimeData.WarningMessage.kMethodResolutionFailure, methodName);
            }
            LogWarning(ProtoCore.RuntimeData.WarningID.kMethodResolutionFailure, message);
        }
    }
}