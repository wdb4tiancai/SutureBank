using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"YooAsset.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// System.Action<object>
	// }}

	public void RefMethods()
	{
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetAsync<object>(string)
		// YooAsset.AssetHandle YooAsset.YooAssets.LoadAssetAsync<object>(string)
	}
}