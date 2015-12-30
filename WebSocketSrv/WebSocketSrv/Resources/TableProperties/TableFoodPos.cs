using ShitMan;
using System.Collections;

public class TableFoodPos
{
	public TableFoodPos(int id, Vector3 pos, Vector3 dir, string modelPath)
	{
		this.id = id;
		this.pos = pos;
		this.dir = dir;
		this.modelPath = modelPath;
	}

	/// <summary>
	/// ID
	/// </summary>
	public int id;
	/// <summary>
	/// 位置
	/// </summary>
	public Vector3 pos;
	/// <summary>
	/// 方向
	/// </summary>
	public Vector3 dir;
	/// <summary>
	/// 模型路径
	/// </summary>
	public string modelPath;
}