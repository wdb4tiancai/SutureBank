
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;


namespace Game.Data
{
	public partial struct jint
	{
		public jint(ByteBuf _buf) 
		{
			Value = _buf.ReadInt();
		}

		public static jint Deserializejint(ByteBuf _buf)
		{
			return new jint(_buf);
		}

		public readonly int Value;
	   

		public  void ResolveRef(GameConfigs tables)
		{
			
		}

		public override string ToString()
		{
			return "{ "
			+ "value:" + Value + ","
			+ "}";
		}
	}

}
