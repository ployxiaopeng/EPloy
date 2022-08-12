using EPloy.Hotfix.Net;
namespace EPloy.Hotfix.Net
{
	[Message(OuterOpcode.C2G_EnterMap)]
	public partial class C2G_EnterMap : IRequest {}

	[Message(OuterOpcode.G2C_EnterMap)]
	public partial class G2C_EnterMap : IResponse {}

// 自己的unit id
// 所有的unit
	[Message(OuterOpcode.UnitInfo)]
	public partial class UnitInfo {}

	[Message(OuterOpcode.C2R_Ping)]
	public partial class C2R_Ping : IRequest {}

	[Message(OuterOpcode.R2C_Ping)]
	public partial class R2C_Ping : IResponse {}

	[Message(OuterOpcode.G2C_Test)]
	public partial class G2C_Test : IMessage {}

	[Message(OuterOpcode.C2M_Reload)]
	public partial class C2M_Reload : IRequest {}

	[Message(OuterOpcode.M2C_Reload)]
	public partial class M2C_Reload : IResponse {}

	[Message(OuterOpcode.TestMessage)]
	public partial class TestMessage : IMessage {}

	[Message(OuterOpcode.TestRpcRequest)]
	public partial class TestRpcRequest : IRequest {}

	[Message(OuterOpcode.TestRpcResponse)]
	public partial class TestRpcResponse : IResponse {}

}
namespace EPloy.Hotfix.Net
{
	public static partial class OuterOpcode
	{
		 public const ushort C2G_EnterMap = 101;
		 public const ushort G2C_EnterMap = 102;
		 public const ushort UnitInfo = 103;
		 public const ushort C2R_Ping = 104;
		 public const ushort R2C_Ping = 105;
		 public const ushort G2C_Test = 106;
		 public const ushort C2M_Reload = 107;
		 public const ushort M2C_Reload = 108;
		 public const ushort TestMessage = 109;
		 public const ushort TestRpcRequest = 110;
		 public const ushort TestRpcResponse = 111;
	}
}
