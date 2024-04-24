namespace QuartzTest.TestInfo;

public partial class Variable
{
    public Variable()
    {

    }
    public Variable(string id, string name, string variableId, string description, string method, string address, int type, int endian, string readExpression, string writeExpression, bool isUpload, bool isTrigger, int permission, string alias, uint index, string deviceId, DateTime createTime, string createBy, string plcGroupId, string businessType, bool enable, int cmdPriod, int period, int stringLength, DateTime? updateTime = null, string? updateBy = null)
    {
        ID = id;
        Name = name;
        VariableId = variableId;
        Description = description;
        Method = method;
        Address = address;
        Type = type;
        Endian = endian;
        ReadExpression = readExpression;
        WriteExpression = writeExpression;
        IsUpload = isUpload;
        IsTrigger = isTrigger;
        Permission = permission;
        Alias = alias;
        Index = index;
        DeviceId = deviceId;
        CreateTime = createTime;
        CreateBy = createBy;
        UpdateTime = updateTime;
        UpdateBy = updateBy;
        PLCGroupId = plcGroupId;
        BusinessType = businessType;
        Enable = enable;
        CmdPeriod = cmdPriod;
        Period = period;
        StringLength = stringLength;
    }
    public string ID { get; set; }
    public string Name { get; set; }
    public string VariableId { get; set; }
    public string Description { get; set; }
    public string Method { get; set; }
    public string Address { get; set; }
    public int Type { get; set; }
    public int Endian { get; set; }
    public string ReadExpression { get; set; }
    public string WriteExpression { get; set; }
    public bool IsUpload { get; set; }
    public bool IsTrigger { get; set; }
    public int Permission { get; set; }
    public string Alias { get; set; }
    public uint Index { get; set; }
    public string DeviceId { get; set; }
    public DateTime CreateTime { get; set; }
    public string CreateBy { get; set; }
    public DateTime? UpdateTime { get; set; }
    public string? UpdateBy { get; set; }
    public string PLCGroupId { get; set; }
    public string BusinessType { get; set; }
    public bool Enable { get; set; }
    public int CmdPeriod { get; set; }
    public int Period { get; set; }
    public int StringLength { get; set; }
}

public partial class Variable
{
    public static List<Variable> MockData => GetMockData();

    private static List<Variable> GetMockData()
    {
        var mockData = new List<Variable>()
        {
            new("variable1", "variableName1", "variableCode1", "description1", "m1", "addr1", 0, 0, "", "", true, true, 0, "", 1, "device1", DateTime.Now, "createrA", "plcG1", "businessType1", true, 5000, 5000, 100),
            new("variable2", "variableName2", "variableCode2", "description2", "m1", "addr1", 0, 0, "", "", true, true, 0, "", 2, "device1", DateTime.Now, "createrA", "plcG1", "businessType1", true, 5000, 5000, 100),
            new("variable3", "variableName3", "variableCode3", "description3", "m1", "addr2", 0, 0, "", "", true, true, 0, "", 3, "device2", DateTime.Now, "createrB", "plcG2", "businessType1", true, 5000, 5000, 100),
            new("variable4", "variableName4", "variableCode4", "description4", "m1", "addr2", 0, 0, "", "", true, true, 0, "", 4, "device2", DateTime.Now, "createrB", "plcG2", "businessType1", true, 5000, 5000, 100),
            new("variable5", "variableName5", "variableCode5", "description5", "m1", "addr3", 0, 0, "", "", true, true, 0, "", 5, "device2", DateTime.Now, "createrC", "plcG3", "businessType1", true, 5000, 5000, 100),
            new("variable6", "variableName6", "variableCode6", "description6", "m1", "addr3", 0, 0, "", "", true, true, 0, "", 6, "device2", DateTime.Now, "createrC", "plcG3", "businessType1", true, 5000, 5000, 100),
            new("variable7", "variableName7", "variableCode7", "description7", "m1", "addr3", 0, 0, "", "", true, true, 0, "", 7, "device1", DateTime.Now, "createrC", "plcG3", "businessType1", true, 5000, 5000, 100),
            new("variable8", "variableName8", "variableCode8", "description8", "m1", "addr3", 0, 0, "", "", true, true, 0, "", 8, "device2", DateTime.Now, "createrC", "plcG3", "businessType1", true, 5000, 5000, 100),
            new("variable9", "variableName9", "variableCode9", "description9", "m1", "addr3", 0, 0, "", "", true, true, 0, "", 9, "device1", DateTime.Now, "createrC", "plcG3", "businessType1", true, 5000, 5000, 100),
            new("variable10", "variableName10", "variableCode10", "description10", "m1", "addr1", 0, 0, "", "", true, true, 0, "", 10, "device1", DateTime.Now, "createrC", "plcG1", "businessType1", true, 5000, 5000, 100),
        };
        //if (MockData?.Any() == false)
        //{
        //    MockData!.Add(new Variable("variable1", "variableName1", "variableCode1", "description1", "m1", "addr1", 0, 0, "", "", true, true, 0, "", 1, "device1", DateTime.Now, "createrA", "plcG1", "businessType1", true, 5000, 5000, 100));
        //    MockData!.Add(new Variable("variable2", "variableName2", "variableCode2", "description2", "m1", "addr1", 0, 0, "", "", true, true, 0, "", 2, "device1", DateTime.Now, "createrA", "plcG1", "businessType1", true, 5000, 5000, 100));
        //    MockData!.Add(new Variable("variable3", "variableName3", "variableCode3", "description3", "m1", "addr2", 0, 0, "", "", true, true, 0, "", 3, "device2", DateTime.Now, "createrB", "plcG2", "businessType1", true, 5000, 5000, 100));
        //    MockData!.Add(new Variable("variable4", "variableName4", "variableCode4", "description4", "m1", "addr2", 0, 0, "", "", true, true, 0, "", 4, "device2", DateTime.Now, "createrB", "plcG2", "businessType1", true, 5000, 5000, 100));
        //    MockData!.Add(new Variable("variable5", "variableName5", "variableCode5", "description5", "m1", "addr3", 0, 0, "", "", true, true, 0, "", 5, "device3", DateTime.Now, "createrC", "plcG3", "businessType1", true, 5000, 5000, 100));
        //}

        return mockData!;
    }
}