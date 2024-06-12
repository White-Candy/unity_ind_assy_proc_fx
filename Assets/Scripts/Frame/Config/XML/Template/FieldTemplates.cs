using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftwareQuestionVosItem
{
    /// <summary>
    /// 
    /// </summary>
    public int questionId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int type { get; set; }
    /// <summary>
    /// 单选题测试1
    /// </summary>
    public string body { get; set; }
    /// <summary>
    /// 选项A
    /// </summary>
    public string choiceA { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string choliceF { get; set; }
    /// <summary>
    /// 选项B
    /// </summary>
    public string choiceB { get; set; }
    /// <summary>
    /// 选项C
    /// </summary>
    public string choiceC { get; set; }
    /// <summary>
    /// 选项D
    /// </summary>
    public string choiceD { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string choiceE { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string answer { get; set; }
}

public class SXChildItem
{
    /// <summary>
    /// 
    /// </summary>
    public int softwareId { get; set; }
    /// <summary>
    /// 电源故障
    /// </summary>
    public string softwareName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string code { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int level { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int sep { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string softwareQuestionVos { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string child { get; set; }
}

public class ChildItem
{
    /// <summary>
    /// 
    /// </summary>
    public int softwareId { get; set; }
    /// <summary>
    /// 计算机
    /// </summary>
    public string softwareName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string code { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int level { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int sep { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<SoftwareQuestionVosItem> softwareQuestionVos { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<SXChildItem> child { get; set; }
}

public class SoftwareInfoVo
{
    /// <summary>
    /// 
    /// </summary>
    public int softwareId { get; set; }
    /// <summary>
    /// 设备检修虚拟仿真软件
    /// </summary>
    public string softwareName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string code { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int level { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int sep { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string softwareQuestionVos { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<ChildItem> child { get; set; }
}

public class Data
{
    /// <summary>
    /// 
    /// </summary>
    public int examId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string examSerializeId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int softwareId { get; set; }
    /// <summary>
    /// 设备检修虚拟仿真软件
    /// </summary>
    public string softwareName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public SoftwareInfoVo softwareInfoVo { get; set; }
}

public class ExamJsonData
{
    /// <summary>
    /// 操作成功
    /// </summary>
    public string msg { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int code { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Data data { get; set; }
}


public class AnswerDetailVoListItem
{
    /// <summary>
    /// 
    /// </summary>
    public int resourceId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string userAnswer { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string userScore { get; set; }
}

public class UserExamDetailVoListItem
{
    /// <summary>
    /// 
    /// </summary>
    public int type { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<AnswerDetailVoListItem> answerDetailVoList { get; set; }
}

public class SubmitExamInfoData
{
    /// <summary>
    /// 
    /// </summary>
    public string examSerializeId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<UserExamDetailVoListItem> userExamDetailVoList { get; set; }
}
