namespace Docx.Templater.Tests
{
	public static class TestHelper
	{
		/// <summary>
		/// Pack object of type T to Array<T> 
		/// </summary>
		/// <returns>The array.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T [] AsArray<T> (this T obj)
		{
			return new [] { obj };
		}
	}
}