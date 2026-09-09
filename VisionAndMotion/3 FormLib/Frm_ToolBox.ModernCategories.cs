using System.Windows.Forms;

namespace VMPro
{
    internal partial class Frm_ToolBox
    {
        /// <summary>
        /// 将历史上按开发批次堆叠的工具分类，归并为用户更容易查找的七类。
        /// 仅移动既有 TreeNode，不改变叶节点文字、图标索引或工具创建分支。
        /// </summary>
        private void ApplyModernToolCategories(
            TreeNode imageNode,
            TreeNode detectNode,
            TreeNode calibrationNode,
            TreeNode alignNode,
            TreeNode logicNode,
            TreeNode findAndFitNode,
            TreeNode createNode,
            TreeNode geometryNode,
            TreeNode calculateNode,
            TreeNode deviceNode,
            TreeNode communicationNode,
            TreeNode empty3DNode,
            TreeNode outputNode)
        {
            bool english = Project.Instance.configuration.language == Language.English;

            imageNode.Text = english ? "Image input and preprocessing" : "图像输入与预处理";
            detectNode.Text = english ? "Detection and recognition" : "检测与识别";
            calibrationNode.Text = english ? "Calibration and positioning" : "标定与定位";
            geometryNode.Text = english ? "Geometry and ROI" : "几何与 ROI";
            logicNode.Text = english ? "Logic and calculation" : "逻辑与计算";
            deviceNode.Text = english ? "Devices and communication" : "设备与通信";
            outputNode.Text = english ? "Output and display" : "输出与显示";

            imageNode.Tag = english ? "Image acquisition, conversion, preprocessing and storage" : "图像采集、转换、预处理与存储";
            detectNode.Tag = english ? "Matching, feature detection and code or text recognition" : "匹配、特征检测及条码文字识别";
            calibrationNode.Tag = english ? "Calibration, coordinate conversion and visual positioning" : "标定、坐标变换与视觉定位引导";
            geometryNode.Tag = english ? "ROI construction, fitting and geometric measurement" : "ROI 构建、查找拟合与几何测量";
            logicNode.Tag = english ? "Flow control, arithmetic, scripts and data analysis" : "流程控制、运算、脚本与数据分析";
            deviceNode.Tag = english ? "Lighting, scanners, PLC and Ethernet communication" : "光源、扫码器、PLC 与以太网通信";
            outputNode.Tag = english ? "Result conversion, display and output" : "结果转换、显示与输出";

            // “距离测量”历史上被放在检测识别中，移动到几何测量。
            if (detectNode.Nodes.Count > 1)
                MoveNode(detectNode.Nodes[1], geometryNode);

            // “图像相减”属于预处理，不应继续混在检测识别里。
            TreeNode imageSubtractNode = FindChildByText(detectNode, "图像相减", "SubImage");
            if (imageSubtractNode != null)
                MoveNode(imageSubtractNode, imageNode);

            MoveAllChildren(alignNode, calibrationNode);
            MoveAllChildren(findAndFitNode, geometryNode);

            // 创建组合的最后三项是文本/显示工具，归入输出，其余归入 ROI。
            while (createNode.Nodes.Count > 5)
                MoveNode(createNode.Nodes[5], outputNode);
            MoveAllChildren(createNode, geometryNode);

            MoveAllChildren(calculateNode, logicNode);
            MoveAllChildren(communicationNode, deviceNode);

            // 历史“其它相关”里的点补偿实际服务于视觉定位，输出分类只保留
            // 转文本、数据显示和输出项等结果呈现工具。
            TreeNode pointCompensationNode = FindChildByText(outputNode, "点补偿", "FindLine");
            if (pointCompensationNode != null)
                MoveNode(pointCompensationNode, calibrationNode);

            RemoveRootNode(alignNode);
            RemoveRootNode(findAndFitNode);
            RemoveRootNode(createNode);
            RemoveRootNode(calculateNode);
            RemoveRootNode(communicationNode);
            RemoveRootNode(empty3DNode);

            // 导航顺序按从图像处理到测量、再到逻辑和外部资源的常见流程排列。
            if (geometryNode.TreeView == tvw_tools && logicNode.TreeView == tvw_tools && geometryNode.Index > logicNode.Index)
            {
                int targetIndex = logicNode.Index;
                tvw_tools.Nodes.Remove(geometryNode);
                tvw_tools.Nodes.Insert(targetIndex, geometryNode);
            }
        }

        private static void MoveAllChildren(TreeNode source, TreeNode target)
        {
            while (source.Nodes.Count > 0)
                MoveNode(source.Nodes[0], target);
        }

        private static TreeNode FindChildByText(TreeNode parent, params string[] candidateTexts)
        {
            foreach (TreeNode child in parent.Nodes)
            {
                foreach (string candidateText in candidateTexts)
                {
                    if (child.Text == candidateText)
                        return child;
                }
            }

            return null;
        }

        private static void MoveNode(TreeNode node, TreeNode target)
        {
            if (node.Parent != null)
                node.Parent.Nodes.Remove(node);
            target.Nodes.Add(node);
        }

        private void RemoveRootNode(TreeNode node)
        {
            if (node != null && node.TreeView == tvw_tools)
                tvw_tools.Nodes.Remove(node);
        }
    }
}
