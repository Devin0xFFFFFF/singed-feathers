using System;

namespace Assets.Scripts {

	public static class EnvironmentVariables {
		
		public static bool IsTestingWithTravis() { return GetEnvironmentVariable("TRAVIS_TESTING"); }

		private static bool GetEnvironmentVariable(String variable)
		{
			bool result;
			try {
				string variable_content = System.Environment.GetEnvironmentVariable(variable);
				result = Convert.ToBoolean(variable_content);
			}
			catch {
				result = false;
			}
			return result;

		}
	}
}

