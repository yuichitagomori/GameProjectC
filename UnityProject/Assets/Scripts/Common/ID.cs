
namespace Common
{
	public class ID
	{
		public int value;
		public int Invalid => -1;

		public bool IsInvalid => value == Invalid;
	}
}
