using System;

namespace Name2Joliet
{
	/// <summary>
	/// 
	/// </summary>
	public class Joliett
	{
		public Joliett()
		{
			// 
			// TODO: Add constructor logic here
			//
		}

		public void DumpStandard()
		{
			System.Console.WriteLine("Joliett rules:");
			System.Console.WriteLine("128 bytes of file name (64 Unicode characters)");
			System.Console.WriteLine(@"No control chars (0x00 - 0x1f) and no / * : ; ? \");
		}

		public string CheckStd(string sFileName)
		{
			if(sFileName.Length > 64)
				return "too long";
			if(sFileName.IndexOfAny(@"/*:;?\".ToCharArray()) > -1)
				return "special char";
			for(int i=0;i<sFileName.Length;i++)
				if(sFileName[i].CompareTo(' ') < 0)
					return "control char";

			return null;
		}

		public string SkipControlChars(string sFileName)
		{
			string sTmp1, sTmp = sFileName;
			// zakladam, ze sTmp.Len bedzie wyliczane za kazdym razem!
			for(int i=0;i<sTmp.Length;i++)
				if(sTmp[i].CompareTo(' ') < 0)
				{
					sTmp1 = sTmp.Substring(0,i) + sTmp.Substring(i+1);
					sTmp = sTmp1;
				}

			return sTmp;
		}

		public string SkipDisallowedChars(string sFileName)
		{
			string sTmp1, sTmp = sFileName;
			for(int i = sTmp.IndexOfAny(@"/*:;?\".ToCharArray()); i > -1;)
			{
				sTmp1 = sTmp.Substring(0,i) + sTmp.Substring(i+1);
				sTmp = sTmp1;
			}

			return sTmp;
		}

		public string TruncateName(string sFileName)
		{
			return sFileName.Substring(0,63);
		}
	}
}
