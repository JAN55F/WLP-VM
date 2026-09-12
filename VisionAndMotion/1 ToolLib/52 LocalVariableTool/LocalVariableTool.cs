using System;

namespace VMPro
{
    /// <summary>
    /// 局部变量工具：仅作为当前流程局部变量的编辑入口。
    /// 数据实际保存在 Job.localVariables（随项目持久化），
    /// 工具运行不产生输入输出，也不参与数据流转。
    /// </summary>
    [Serializable]
    internal class LocalVariableTool : ToolBase
    {
        private readonly object obj = new object();

        public override void Run(bool updateImage, bool b, string toolName)
        {
            lock (obj)
            {
                toolRunStatu = Project.Instance.configuration.language == Language.English ? ToolRunStatu.Succeed : ToolRunStatu.成功;
            }
        }
    }
}
