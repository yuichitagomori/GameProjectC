namespace data.resource
{
	/// <summary>
	/// テクスチャアセット
	/// </summary>
	public class CustomizePartsTextureResource : TextureResource
	{
		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public override Data CreateData(int id)
		{
			string format = "CustomizeParts/Parts{0:0000}.png";
			string path = string.Format(CommonPath + format, id);
			return CreateData(id, path);
		}
	}
}