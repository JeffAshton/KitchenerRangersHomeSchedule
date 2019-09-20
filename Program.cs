using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace KitchenerRangersHomeSchedule
{
	class Program
	{
		private static readonly Regex m_lineRegex = new Regex(
			@"^(\w+)\:(.+)",
			RegexOptions.Compiled
		);

		private static (string, string) ParseLine(string line)
		{
			Match m = m_lineRegex.Match(line);
			string param = m.Groups[1].Value;
			string value = m.Groups[2].Value;
			return (param, value);
		}

		static void Main(string[] args)
		{

			using (StreamReader sr = new StreamReader(@"C:\Users\jeffa\Downloads\invite.ics"))
			using (StreamWriter sw = new StreamWriter(@"C:\Users\jeffa\Downloads\invite-filtered.ics"))
			{

				string line;
				while ((line = sr.ReadLine()) != null)
				{
					(string param, string value) = ParseLine(line);
					if (param.Equals("BEGIN") && value.Equals("VEVENT"))
					{
						ParseEventLines(
							sr,
							out Dictionary<string, string> parameters,
							out List<string> eventLines
						);

						string summary = parameters["SUMMARY"];
						if( summary.EndsWith("@ Kitchener Rangers")) {

							sw.WriteLine(line);
							eventLines.ForEach(sw.WriteLine);
						}
					}
					else
					{
						sw.WriteLine(line);
					}
				}

			}
		}

		private static void ParseEventLines( 
				StreamReader sr,
				out Dictionary<string,string> parameters,
				out List<string> lines
			) {

			parameters = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase);
			lines = new List<string>();

			for (; ; ) {

				string line = sr.ReadLine();
				lines.Add(line);

				(string param, string value) = ParseLine(line);
				if (param.Equals("END") && value.Equals("VEVENT"))
				{
					return;
				}
				else
				{
					parameters.Add(param, value);
				}
			}
		}
	}
}
