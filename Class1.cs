using System;
using System.IO;

namespace Name2Joliet
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		private static Joliett mJoliett;
		private static Replace mReplace;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{

			mJoliett = new Joliett();
			mReplace = new Replace();

			string sArg0 = ".";
            if(args.Length != 0) sArg0 = args[0].ToLower();

			switch(sArg0)
				{
				case "/h":
					goto case "/?";
				case "/?":
					ShowHelp();
					return;
				case "/dump":
					mJoliett.DumpStandard();
					mReplace.DumpRules();
					break;
				case "/edit":
					goto case "/rules";
				case "/rules":
					mReplace.EditRules();
					break;
				default:
					string sErr = WrapperKatalogu(args[0]);
					if(sErr != null)
						System.Console.Error.WriteLine(sErr);
					break;
				}




			//
			// TODO: Add code to start application here
			//
		}

		private static void ShowHelp()
		{
			System.Console.WriteLine("Name2Joliet v1.0 (c) PKAR");
			System.Console.WriteLine("Usage:");
			System.Console.WriteLine(" name2joliet [dirname]");
			System.Console.WriteLine("or use switches:");
			System.Console.WriteLine(" name2joliet /?\t- show help");
			System.Console.WriteLine(" name2joliet /h\t- show help");
			System.Console.WriteLine(" name2joliet /rules\t- edit rules");
			System.Console.WriteLine(" name2joliet /edit\t- edit rules");
			System.Console.WriteLine(" name2joliet /dump\t- dump rules");
		}

		private static string WrapperKatalogu(string sDirName)
		{
			if(!Directory.Exists(sDirName))
				return(String.Format("Directory '{0}' does not exist",sDirName));

			string sL2S = sDirName + @"\long2short.cmd";
			string sS2L = sDirName + @"\short2long.cmd";
			string sInd = sDirName + @"\index.txt";
			string sLog = sDirName + @"\name2joliet.log";
			string sHtm = sDirName + @"\default.htm";

			if(File.Exists(sL2S))
				return "long2short batch exist";
			if(File.Exists(sS2L))
				return "short2long batch exist";
			if(File.Exists(sInd))
				return "index file exist";
			if(File.Exists(sHtm))
				return "HTML file exist";
			
			StreamWriter fL2S = File.AppendText(sL2S);
			fL2S.WriteLine("@echo off");
			fL2S.WriteLine("rem batch generated by Name2Joliett on {0}",DateTime.Now);
			StreamWriter fS2L = File.AppendText(sS2L);
			fS2L.WriteLine("@echo off");
			fS2L.WriteLine("rem batch generated by Name2Joliett on {0}",DateTime.Now);
			StreamWriter fHtm = File.AppendText(sHtm);
			fHtm.WriteLine("<!-- generated by Name2Joliett on {0} -->",DateTime.Now);
			fHtm.WriteLine("<ul>");

			StreamWriter fInd = File.AppendText(sInd);
			StreamWriter fLog = File.AppendText(sLog);

			DirectoryInfo oDi = new DirectoryInfo(sDirName);
			FileInfo[] aoFI = oDi.GetFiles();

			string sTmp,sTmp1,sOrgFName;
			int cTotal, cCnt;
			cTotal = cCnt = 0;
			foreach(FileInfo oFI in aoFI)
			{
				sOrgFName = oFI.Name;
				fInd.WriteLine(sOrgFName);

				cTotal++;

				sTmp = mJoliett.CheckStd(sOrgFName);
				
				if(sTmp != null)
				{
					cCnt++;

					fLog.WriteLine("\"{0}\" \t name error: {1}",sOrgFName,sTmp);
					// krok 1 - wyrzucenie znakow kontrolnych
					sTmp = mJoliett.SkipControlChars(sOrgFName);
					if(sTmp != sOrgFName)
						fLog.WriteLine(" after skipping control chars:\n  {0}",sTmp);

					// krok 2 - wyrzucenie znakow niedozwolonych
					sTmp1 = mJoliett.SkipDisallowedChars(sTmp);
					if(sTmp1 != sTmp)
						fLog.WriteLine(" after skipping disallowed chars:\n  {0}",sTmp1);
					sTmp = sTmp1;

					// moze juz?
					if(mJoliett.CheckStd(sTmp) != null)
					{
						// krok 3 - sprobuj podmieniac teksty
						sTmp1 = mReplace.ReplaceLight(sTmp);

						if(sTmp1 != sTmp)
							fLog.WriteLine(" after light string substitutes:\n  {0}",sTmp1);
						sTmp = sTmp1;

						if(mJoliett.CheckStd(sTmp) != null)
						{
							// krok 4 - podmiana "na sile"
							sTmp1 = mReplace.ReplaceHard(sTmp);
							if(sTmp1 != sTmp)
								fLog.WriteLine(" after hard string substitutes:\n  {0}",sTmp1);
							sTmp = sTmp1;

							// wystarczylo?
							if(mJoliett.CheckStd(sTmp) != null)
							{
								// krok 5 - po prostu utnij, niestety
								sTmp1 = mJoliett.TruncateName(sTmp);
								if(sTmp1 != sTmp)
									fLog.WriteLine(" after truncating:\n  {0}",sTmp1);
								sTmp = sTmp1;
							}
						}
					}

					fL2S.WriteLine("ren \"{0}\" \"{1}\"",sOrgFName,sTmp);
					fS2L.WriteLine("ren \"{0}\" \"{1}\"",sTmp,sOrgFName);
					fHtm.WriteLine("<li><a href='{0}'>{1}</a></li>",sTmp, sOrgFName);

				}
				else
				{
					fLog.WriteLine("\"{0}\" \t name OK",sOrgFName);
					fHtm.WriteLine("<li><a href='{0}'>{0}</a></li>",sOrgFName);
				}

			}

			fL2S.Close();
			fS2L.Close();
			fInd.Close();
			fLog.Close();
			fHtm.WriteLine("</ul>");
			fHtm.Close();
//			if(cCnt == 0)
//			{
//				File.Delete(sS2L);
//				File.Delete(sL2S);
//			}
			System.Console.WriteLine("{0} files (out of {1}) to be renamed",cCnt,cTotal);
			return null;
		}


		
	}
}
