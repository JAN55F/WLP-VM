using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace VMPro
{
    [Serializable]
    internal class CodeEditTool : ToolBase
    {
        internal CodeEditTool()
        {
            sourceCode =
@"using System;
using System.Collections.Generic;

namespace VMPro.CodeEditRuntime
{
    public class UserScript : ScriptBase
    {
        public void Execute(
            Dictionary<string, object> inputs,
            Dictionary<string, object> outputs)
        {
            ////////////////////////////////////////////////////////////////
            ////////////////////// 输 入 初 始 化 区 域 //////////////////////
            ////////////////////////////////////////////////////////////////
            #region InputInitialization



            #endregion

            ////////////////////////////////////////////////////////////////
            ////////////////////// 功 能 实 现 区 域 //////////////////////
            ////////////////////////////////////////////////////////////////
            #region FunctionImplementation



            // 输 出 初 始 化 开 始
            // 输 出 初 始 化 结 束

            // 在 这 里 添 加 计 算、临 时 变 量、辅 助 逻 辑，并 给 输 出 变 量 赋 值。



            // 输 出 回 写 开 始
            // 输 出 回 写 结 束

            #endregion

            ////////////////////////////////////////////////////////////////
        }
    }
}";
        }

        private const string HiddenRuntimeSource =
@"using System;
using System.Collections.Generic;
using System.Globalization;

namespace VMPro.CodeEditRuntime
{
    public abstract class ScriptBase
    {
        protected static T Read<T>(Dictionary<string, object> values, string name)
        {
            object rawValue;
            if (!values.TryGetValue(name, out rawValue) || rawValue == null)
                return default(T);

            if (rawValue is T)
                return (T)rawValue;

            return (T)Convert.ChangeType(rawValue, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}";

        private readonly object obj = new object();
        internal ToolPar toolPar = new ToolPar();
        internal string sourceCode = string.Empty;
        internal List<CodeInputItem> L_inputItems = new List<CodeInputItem>();
        internal List<CodeOutputItem> L_outputItems = new List<CodeOutputItem>();
        // 保留旧字段，仅用于兼容已有项目数据；脚本编辑已不再执行运算模块。
        internal List<CodeCalcItem> L_calcItems = new List<CodeCalcItem>();
        internal string compileResult = string.Empty;

        public override void Run(bool updateImage, bool b, string toolName)
        {
            try
            {
                lock (obj)
                {
                    EnsureClassTemplate();
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因;
                    compileResult = string.Empty;
                    toolPar.InputPar.Clear();
                    toolPar.ResultPar.Clear();

                    Dictionary<string, object> inputs = ApplyInputItems();
                    Dictionary<string, object> outputs = RunCode(inputs);
                    ApplyOutputItems(outputs);
                    toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功;
                }
            }
            catch (Exception ex)
            {
                compileResult = GetExceptionMessage(ex);
                toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Not_Succeed : ToolRunStatu.未知原因;
                Log.SaveError(ex);
            }
        }

        private Dictionary<string, object> ApplyInputItems()
        {
            Dictionary<string, object> inputs = new Dictionary<string, object>();
            for (int i = 0; i < L_inputItems.Count; i++)
            {
                CodeInputItem item = L_inputItems[i];
                if (item == null || string.IsNullOrEmpty(item.InputName))
                    continue;
                object rawValue = GetLinkedSourceValue(item.VariableSource);
                object value = ConvertValue(rawValue, item.ValueType);
                inputs[item.InputName] = value;
                toolPar.InputPar.SetValue(item.InputName, value);
            }
            return inputs;
        }

        private Dictionary<string, object> RunCode(Dictionary<string, object> inputs)
        {
            CompilerResults compiled = Compile(sourceCode);
            if (compiled.Errors.HasErrors)
            {
                compileResult = ReturnErrors(compiled);
                throw new Exception(compileResult);
            }

            Type scriptType = compiled.CompiledAssembly.GetType("VMPro.CodeEditRuntime.UserScript");
            if (scriptType == null)
                throw new Exception("未找到 VMPro.CodeEditRuntime.UserScript 类。");

            object script = Activator.CreateInstance(scriptType);
            Type dictionaryType = typeof(Dictionary<string, object>);
            MethodInfo execute = scriptType.GetMethod(
                "Execute",
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new Type[] { dictionaryType, dictionaryType },
                null);
            if (execute == null)
                throw new Exception("未找到 Execute(Dictionary<string, object> inputs, Dictionary<string, object> outputs) 方法。");

            Dictionary<string, object> outputs = new Dictionary<string, object>();
            execute.Invoke(script, new object[] { inputs, outputs });
            return outputs;
        }

        internal string ValidateCode(List<CodeInputItem> inputItems, List<CodeOutputItem> outputItems, string body)
        {
            try
            {
                CompilerResults compiled = Compile(body);
                if (compiled.Errors.HasErrors)
                    return ReturnErrors(compiled);

                Type scriptType = compiled.CompiledAssembly.GetType("VMPro.CodeEditRuntime.UserScript");
                if (scriptType == null)
                    return "编译失败：未找到 VMPro.CodeEditRuntime.UserScript 类。";
                Type dictionaryType = typeof(Dictionary<string, object>);
                MethodInfo execute = scriptType.GetMethod(
                    "Execute",
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new Type[] { dictionaryType, dictionaryType },
                    null);
                if (execute == null || execute.ReturnType != typeof(void))
                    return "编译失败：未找到 public void Execute(Dictionary<string, object> inputs, Dictionary<string, object> outputs) 方法。";
                return "编译成功";
            }
            catch (Exception ex)
            {
                return "编译失败：" + ex.Message;
            }
        }

        internal void EnsureClassTemplate()
        {
            if (!string.IsNullOrEmpty(sourceCode))
            {
                sourceCode = sourceCode.Replace("#region 输入初始化", "#region InputInitialization");
                sourceCode = sourceCode.Replace("#region 功能实现", "#region FunctionImplementation");
            }
            bool usesDictionaryContract = !string.IsNullOrEmpty(sourceCode)
                && sourceCode.Contains("Dictionary<string, object> inputs")
                && sourceCode.Contains("Dictionary<string, object> outputs");
            bool hasPlannedRegions = !string.IsNullOrEmpty(sourceCode)
                && sourceCode.Contains("#region InputInitialization")
                && sourceCode.Contains("#region FunctionImplementation");
            if (usesDictionaryContract && hasPlannedRegions)
            {
                const string legacyInputDivider = "////////////////////// 输入初始化区域 //////////////////////";
                const string legacyFunctionDivider = "////////////////////// 功能实现区域 //////////////////////";
                const string explicitInputDivider = "// ////////////////////// 输入初始化区域 //////////////////////";
                const string explicitFunctionDivider = "// ////////////////////// 功能实现区域 //////////////////////";
                const string inputDivider = "////////////////////// 输 入 初 始 化 区 域 //////////////////////";
                const string functionDivider = "////////////////////// 功 能 实 现 区 域 //////////////////////";
                if (!sourceCode.Contains(inputDivider) && !sourceCode.Contains(legacyInputDivider) && !sourceCode.Contains(explicitInputDivider))
                    sourceCode = sourceCode.Replace("#region InputInitialization", "////////////////////////////////////////////////////////////\r\n            " + inputDivider + "\r\n            ////////////////////////////////////////////////////////////\r\n            #region InputInitialization");
                if (!sourceCode.Contains(functionDivider) && !sourceCode.Contains(legacyFunctionDivider) && !sourceCode.Contains(explicitFunctionDivider))
                    sourceCode = sourceCode.Replace("#region FunctionImplementation", "////////////////////////////////////////////////////////////\r\n            " + functionDivider + "\r\n            ////////////////////////////////////////////////////////////\r\n            #region FunctionImplementation");
                if (!sourceCode.Contains(inputDivider))
                {
                    sourceCode = sourceCode.Replace(explicitInputDivider, inputDivider);
                    sourceCode = sourceCode.Replace(legacyInputDivider, inputDivider);
                }
                if (!sourceCode.Contains(functionDivider))
                {
                    sourceCode = sourceCode.Replace(explicitFunctionDivider, functionDivider);
                    sourceCode = sourceCode.Replace(legacyFunctionDivider, functionDivider);
                }
                MigrateHiddenRuntimeHelpers();
                return;
            }

            bool isGeneratedOldTemplate = string.IsNullOrEmpty(sourceCode)
                || !sourceCode.Contains("class UserScript")
                || sourceCode.Contains("// 输入声明")
                || sourceCode.Contains("// 输入声明：")
                || sourceCode.Contains("// 1. 读取输入")
                || sourceCode.Contains("// 2. 临时变量和业务计算");
            if (!isGeneratedOldTemplate)
                return;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine();
            builder.AppendLine("namespace VMPro.CodeEditRuntime");
            builder.AppendLine("{");
            builder.AppendLine("    public class UserScript : ScriptBase");
            builder.AppendLine("    {");
            builder.AppendLine("        public void Execute(");
            builder.AppendLine("            Dictionary<string, object> inputs,");
            builder.AppendLine("            Dictionary<string, object> outputs)");
            builder.AppendLine("        {");
            builder.AppendLine("            ////////////////////////////////////////////////////////////////");
            builder.AppendLine("            ////////////////////// 输 入 初 始 化 区 域 //////////////////////");
            builder.AppendLine("            ////////////////////////////////////////////////////////////////");
            builder.AppendLine("            #region InputInitialization");
            builder.AppendLine();
            builder.AppendLine();
            for (int i = 0; i < L_inputItems.Count; i++)
            {
                if (!string.IsNullOrEmpty(L_inputItems[i].VariableSource))
                    builder.AppendLine("            // " + FormatCommentText("链接：" + L_inputItems[i].VariableSource));
                builder.AppendLine("            " + GetCSharpType(L_inputItems[i].ValueType) + " " + L_inputItems[i].InputName + " = Read<" + GetCSharpType(L_inputItems[i].ValueType) + ">(inputs, \"" + L_inputItems[i].InputName + "\");");
            }
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("            #endregion");
            builder.AppendLine();
            builder.AppendLine("            ////////////////////////////////////////////////////////////////");
            builder.AppendLine("            ////////////////////// 功 能 实 现 区 域 //////////////////////");
            builder.AppendLine("            ////////////////////////////////////////////////////////////////");
            builder.AppendLine("            #region FunctionImplementation");
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("            // " + FormatCommentText("输出初始化开始"));
            for (int i = 0; i < L_outputItems.Count; i++)
                builder.AppendLine("            " + GetCSharpType(L_outputItems[i].ValueType) + " " + L_outputItems[i].OutputName + " = default(" + GetCSharpType(L_outputItems[i].ValueType) + ");");
            builder.AppendLine("            // " + FormatCommentText("输出初始化结束"));
            builder.AppendLine();
            builder.AppendLine("            // 在 这 里 添 加 计 算、临 时 变 量、辅 助 逻 辑，并 给 输 出 变 量 赋 值。");
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("            // " + FormatCommentText("输出回写开始"));
            for (int i = 0; i < L_outputItems.Count; i++)
                builder.AppendLine("            outputs[\"" + L_outputItems[i].OutputName + "\"] = " + L_outputItems[i].OutputName + ";");
            builder.AppendLine("            // " + FormatCommentText("输出回写结束"));
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("            #endregion");
            builder.AppendLine();
            builder.AppendLine("            ////////////////////////////////////////////////////////////////");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            sourceCode = builder.ToString();
        }

        private void MigrateHiddenRuntimeHelpers()
        {
            if (!sourceCode.Contains("class UserScript : ScriptBase"))
            {
                sourceCode = sourceCode.Replace(
                    "public class UserScript\r\n    {",
                    "public class UserScript : ScriptBase\r\n    {");
                sourceCode = sourceCode.Replace(
                    "public class UserScript\n    {",
                    "public class UserScript : ScriptBase\n    {");
            }

            const string helperRegion = "#region 类型转换辅助方法";
            int markerStart = sourceCode.IndexOf(helperRegion, StringComparison.Ordinal);
            if (markerStart < 0)
                return;

            int regionStart = sourceCode.LastIndexOf('\n', markerStart);
            regionStart = regionStart < 0 ? markerStart : regionStart + 1;
            int regionEnd = sourceCode.IndexOf("#endregion", markerStart, StringComparison.Ordinal);
            if (regionEnd < 0)
                return;

            regionEnd += "#endregion".Length;
            if (regionEnd < sourceCode.Length && sourceCode[regionEnd] == '\r')
                regionEnd++;
            if (regionEnd < sourceCode.Length && sourceCode[regionEnd] == '\n')
                regionEnd++;
            sourceCode = sourceCode.Remove(regionStart, regionEnd - regionStart);
        }

        private string GetCSharpType(CodeValueType type)
        {
            if (type == CodeValueType.Int) return "int";
            if (type == CodeValueType.String) return "string";
            if (type == CodeValueType.Bool) return "bool";
            return "double";
        }

        internal static string FormatCommentText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char current = text[i];
                builder.Append(current);
                if (i < text.Length - 1 && IsCjkCommentCharacter(current) && IsCjkCommentCharacter(text[i + 1]))
                    builder.Append(' ');
            }
            return builder.ToString();
        }

        private static bool IsCjkCommentCharacter(char character)
        {
            int code = character;
            return (code >= 0x3400 && code <= 0x4DBF)
                || (code >= 0x4E00 && code <= 0x9FFF)
                || (code >= 0xF900 && code <= 0xFAFF)
                || (code >= 0x3000 && code <= 0x303F)
                || (code >= 0xFF00 && code <= 0xFFEF);
        }

        private static CompilerResults Compile(string code)
        {
            string[] assemblyNames = new[] { "mscorlib.dll", "Microsoft.CSharp.dll", "System.dll", "System.Core.dll" };
            Dictionary<string, string> options = new Dictionary<string, string>();
            options.Add("CompilerVersion", "v4.0");
            using (CSharpCodeProvider csc = new CSharpCodeProvider(options))
            {
                CompilerParameters parameters = new CompilerParameters(assemblyNames);
                parameters.IncludeDebugInformation = false;
                parameters.GenerateInMemory = true;
                return csc.CompileAssemblyFromSource(parameters, code, HiddenRuntimeSource);
            }
        }

        private static string ReturnErrors(CompilerResults compiled)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Compilation errors:");
            foreach (CompilerError error in compiled.Errors)
            {
                if (!error.IsWarning)
                    builder.AppendLine(error.ErrorText);
            }
            return builder.ToString();
        }

        private static string GetExceptionMessage(Exception ex)
        {
            TargetInvocationException targetException = ex as TargetInvocationException;
            if (targetException != null && targetException.InnerException != null)
                return targetException.InnerException.Message;
            return ex == null ? string.Empty : ex.Message;
        }

        private void ApplyOutputItems(Dictionary<string, object> outputs)
        {
            for (int i = 0; i < L_outputItems.Count; i++)
            {
                CodeOutputItem item = L_outputItems[i];
                if (item == null || string.IsNullOrEmpty(item.OutputName))
                    continue;
                if (!outputs.ContainsKey(item.OutputName))
                    throw new Exception("脚本未写入输出：" + item.OutputName);
                object value = ConvertValue(outputs[item.OutputName], item.ValueType);
                toolPar.ResultPar.SetValue(item.OutputName, value);
                if (item.WriteGlobalVariable && !string.IsNullOrEmpty(item.OutputName))
                    SetCustomGlobalVariableValue(item.OutputName, item.ValueType.ToString(), value);
                if (item.WriteLocalVariable && !string.IsNullOrEmpty(item.OutputName))
                    SetLocalVariableValue(item.OutputName, item.ValueType.ToString(), value);
            }
        }

        private object ConvertValue(object value, CodeValueType type)
        {
            string text = value == null ? string.Empty : value.ToString();
            int intValue;
            double doubleValue;
            bool boolValue;
            switch (type)
            {
                case CodeValueType.Int:
                    return int.TryParse(text, out intValue) ? intValue : 0;
                case CodeValueType.Double:
                    if (!double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleValue))
                        double.TryParse(text, out doubleValue);
                    return doubleValue;
                case CodeValueType.Bool:
                    return bool.TryParse(text, out boolValue) && boolValue;
                default:
                    return text;
            }
        }

        private void SetCustomGlobalVariableValue(string name, string type, object value)
        {
            Variable target = null;
            for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
            {
                Variable variable = Project.Instance.curEngine.globelVariable.L_variable[i];
                if (variable.variableType == 1 && variable.name == name)
                {
                    target = variable;
                    break;
                }
            }
            if (target == null)
            {
                target = new Variable(GetCustomVariableCount() + 1, type, name);
                target.variableType = 1;
                Project.Instance.curEngine.globelVariable.L_variable.Add(target);
            }
            target.type = type;
            target.value = value;
        }

        private int GetCustomVariableCount()
        {
            int count = 0;
            for (int i = 0; i < Project.Instance.curEngine.globelVariable.L_variable.Count; i++)
                if (Project.Instance.curEngine.globelVariable.L_variable[i].variableType == 1)
                    count++;
            return count;
        }

        /// <summary>
        /// 把脚本输出写入所属流程的局部变量；变量不存在时按输出类型自动创建。
        /// 运行在流程工作线程，找不到所属流程时静默跳过，不影响脚本成功状态。
        /// </summary>
        private void SetLocalVariableValue(string name, string valueType, object value)
        {
            try
            {
                Job ownerJob = FindOwnerJobQuietly();
                if (ownerJob == null)
                    return;
                ownerJob.SetLocalVariableValue(name, valueType, value);
            }
            catch (Exception ex)
            {
                Log.SaveError(ex);
            }
        }

        /// <summary>
        /// 静默查找所属流程；避免 FindJobByName 在未命中时弹窗干扰后台运行线程。
        /// </summary>
        private Job FindOwnerJobQuietly()
        {
            Scheme engine = Project.Instance.curEngine;
            if (engine == null || engine.L_jobList == null)
                return null;
            for (int i = 0; i < engine.L_jobList.Count; i++)
            {
                if (engine.L_jobList[i] != null && engine.L_jobList[i].jobName == jobName)
                    return engine.L_jobList[i];
            }
            return null;
        }

        internal object GetInputValue(string name) { return toolPar.InputPar.GetValue(name); }
        internal object GetOutputValue(string name) { return toolPar.ResultPar.GetValue(name); }

        private object GetLinkedSourceValue(string sourceFrom)
        {
            if (string.IsNullOrEmpty(sourceFrom))
                return string.Empty;
            string[] parts = sourceFrom.Split(new string[] { "->" }, StringSplitOptions.None);
            if (parts.Length < 2)
                return string.Empty;
            string sourceToolName = parts[0].Trim();
            if (sourceToolName.StartsWith("《-"))
                sourceToolName = sourceToolName.Substring(3).Trim();
            string itemName = parts[1].Trim();
            if (sourceToolName == "全局变量")
                return Project.Instance.curEngine.globelVariable.GetGlobalVariableValue(itemName);
            if (sourceToolName == "局部变量")
            {
                Job ownerJob = FindOwnerJobQuietly();
                return ownerJob == null ? null : ownerJob.GetLocalVariableValue(itemName);
            }

            string sourceJobName = jobName;
            if (sourceToolName.StartsWith("[") && sourceToolName.Contains("]"))
            {
                int endIndex = sourceToolName.IndexOf("]");
                sourceJobName = sourceToolName.Substring(1, endIndex - 1);
                sourceToolName = sourceToolName.Substring(endIndex + 1);
            }
            Job sourceJob = Project.Instance.curEngine.FindJobByName(sourceJobName);
            ToolInfo sourceTool = sourceJob == null ? null : sourceJob.FindToolInfoByName(sourceToolName);
            return sourceTool == null ? string.Empty : sourceTool.GetOutput(itemName).value;
        }

        [Serializable]
        internal class ToolPar : ToolParBase
        {
            private InputPar _inputPar = new InputPar();
            public InputPar InputPar
            {
                get { return _inputPar ?? (_inputPar = new InputPar()); }
                set { _inputPar = value; }
            }
            private RunPar _runPar = new RunPar();
            public RunPar RunPar
            {
                get { return _runPar ?? (_runPar = new RunPar()); }
                set { _runPar = value; }
            }
            private ResultPar _resultPar = new ResultPar();
            public ResultPar ResultPar
            {
                get { return _resultPar ?? (_resultPar = new ResultPar()); }
                set { _resultPar = value; }
            }
        }

        [Serializable]
        internal class InputPar
        {
            private Dictionary<string, object> values = new Dictionary<string, object>();
            private Dictionary<string, object> Values { get { return values ?? (values = new Dictionary<string, object>()); } }
            internal void Clear() { Values.Clear(); }
            internal void SetValue(string name, object value) { Values[name] = value; }
            internal object GetValue(string name) { return Values.ContainsKey(name) ? Values[name] : null; }
        }

        [Serializable]
        internal class RunPar { }

        [Serializable]
        internal class ResultPar
        {
            private Dictionary<string, object> values = new Dictionary<string, object>();
            private Dictionary<string, object> Values { get { return values ?? (values = new Dictionary<string, object>()); } }
            internal void Clear() { Values.Clear(); }
            internal void SetValue(string name, object value) { Values[name] = value; }
            internal object GetValue(string name) { return Values.ContainsKey(name) ? Values[name] : null; }
        }
    }

    internal enum CodeValueType { Double, Int, String, Bool }

    [Serializable]
    internal class CodeInputItem
    {
        internal CodeInputItem() { }
        internal CodeInputItem(string inputName, CodeValueType valueType, CodeInputSourceType sourceType, string fixedValue, string variableSource)
        {
            InputName = inputName;
            ValueType = valueType;
            SourceType = sourceType;
            FixedValue = fixedValue;
            VariableSource = variableSource;
        }
        internal string InputName = "imput1";
        internal CodeValueType ValueType = CodeValueType.Double;
        internal CodeInputSourceType SourceType = CodeInputSourceType.固定值;
        internal string FixedValue = "0";
        internal string VariableSource = string.Empty;
    }

    internal enum CodeInputSourceType { 固定值, 变量 }

    [Serializable]
    internal class CodeOutputItem
    {
        internal CodeOutputItem() { }
        internal CodeOutputItem(string outputName, CodeValueType valueType, bool writeGlobalVariable, string globalVariableName)
        {
            OutputName = outputName;
            ValueType = valueType;
            WriteGlobalVariable = writeGlobalVariable;
            GlobalVariableName = globalVariableName;
        }
        internal string OutputName = "output1";
        internal CodeValueType ValueType = CodeValueType.Double;
        internal bool WriteGlobalVariable;
        internal bool WriteLocalVariable;
        internal string GlobalVariableName = string.Empty;
        // 旧字段保留用于兼容已有项目数据。
        internal CodeOutputSource SourceType = CodeOutputSource.自定义代码;
        internal string SourceName = string.Empty;
    }

    [Serializable]
    internal class CodeCalcItem
    {
        internal string OutputName = string.Empty;
        internal string LeftValue = string.Empty;
        internal string Operator = string.Empty;
        internal string RightValue = string.Empty;
    }

    internal enum CodeOutputSource { 运算模块, 自定义代码 }
}
