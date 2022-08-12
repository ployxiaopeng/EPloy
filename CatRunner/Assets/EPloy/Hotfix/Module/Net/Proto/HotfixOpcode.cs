using EPloy.Net;
namespace EPloy.Net
{
//客户端注册请求
	[Message(HotfixOpcode.C2G_Register)]
	public partial class C2G_Register : IRequest {}

//客户端注册请求回复
	[Message(HotfixOpcode.G2C_Register)]
	public partial class G2C_Register : IResponse {}

//登录请求
	[Message(HotfixOpcode.C2R_Login)]
	public partial class C2R_Login : IRequest {}

//登录请求回复
	[Message(HotfixOpcode.R2C_Login)]
	public partial class R2C_Login : IResponse {}

//注销请求
	[Message(HotfixOpcode.C2R_Logout)]
	public partial class C2R_Logout : IRequest {}

//登录网关请求
	[Message(HotfixOpcode.C2G_LoginGate)]
	public partial class C2G_LoginGate : IRequest {}

//登录网关请求回复
	[Message(HotfixOpcode.G2C_LoginGate)]
	public partial class G2C_LoginGate : IResponse {}

//玩家匹配请求
	[Message(HotfixOpcode.C2G_StartMatch)]
	public partial class C2G_StartMatch : IRequest {}

//玩家匹配请求回复
	[Message(HotfixOpcode.G2C_StartMatch)]
	public partial class G2C_StartMatch : IResponse {}

//玩家匹配成功
	[Message(HotfixOpcode.G2C_Match)]
	public partial class G2C_Match : IResponse {}

//玩家信息
	[Message(HotfixOpcode.GamerInfo)]
	public partial class GamerInfo {}

//返回大厅
	[Message(HotfixOpcode.C2G_ReturnLobby)]
	public partial class C2G_ReturnLobby : IMessage {}

//获取房间内玩家信息请求
	[Message(HotfixOpcode.C2G_GetUserInfoInRoom)]
	public partial class C2G_GetUserInfoInRoom : IRequest {}

//获取房间内玩家信息返回
	[Message(HotfixOpcode.G2C_GetUserInfoInRoom)]
	public partial class G2C_GetUserInfoInRoom : IResponse {}

//客户端准备游戏消息
	[Message(HotfixOpcode.C2G_GamerReady)]
	public partial class C2G_GamerReady : IRequest {}

	[Message(HotfixOpcode.G2C_TestHotfixMessage)]
	public partial class G2C_TestHotfixMessage : IMessage {}

	[Message(HotfixOpcode.PlayerInfo)]
	public partial class PlayerInfo : IMessage {}

//获取用户信息
	[Message(HotfixOpcode.C2G_UserInfo)]
	public partial class C2G_UserInfo : IRequest {}

//获取用户信息回复 设置用户信息回复
	[Message(HotfixOpcode.G2C_UserInfo)]
	public partial class G2C_UserInfo : IResponse {}

//设置用户信息
	[Message(HotfixOpcode.C2G_SetUserInfo)]
	public partial class C2G_SetUserInfo : IRequest {}

}
namespace EPloy.Net
{
	public static partial class HotfixOpcode
	{
		 public const ushort C2G_Register = 10001;
		 public const ushort G2C_Register = 10002;
		 public const ushort C2R_Login = 10003;
		 public const ushort R2C_Login = 10004;
		 public const ushort C2R_Logout = 10005;
		 public const ushort C2G_LoginGate = 10006;
		 public const ushort G2C_LoginGate = 10007;
		 public const ushort C2G_StartMatch = 10008;
		 public const ushort G2C_StartMatch = 10009;
		 public const ushort G2C_Match = 10010;
		 public const ushort GamerInfo = 10011;
		 public const ushort C2G_ReturnLobby = 10012;
		 public const ushort C2G_GetUserInfoInRoom = 10013;
		 public const ushort G2C_GetUserInfoInRoom = 10014;
		 public const ushort C2G_GamerReady = 10015;
		 public const ushort G2C_TestHotfixMessage = 10016;
		 public const ushort PlayerInfo = 10017;
		 public const ushort C2G_UserInfo = 10018;
		 public const ushort G2C_UserInfo = 10019;
		 public const ushort C2G_SetUserInfo = 10020;
	}
}
