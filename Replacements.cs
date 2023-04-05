using System;
using System.Diagnostics;
using System.IO;

namespace Name2Joliet
{
	/// <summary>
	/// Summary description for Replacements.
	/// </summary>
	public class Replace
	{
		System.Collections.ArrayList mArrL;
		System.Collections.ArrayList mArrH;

		public struct oReplace
		{
			private string sFrom, sTo;
			public oReplace(string strFrom, string strTo)
			{
				sFrom = strFrom;
				sTo = strTo;
			}
			public string GetFrom()
			{
				return sFrom;
			}

			public string GetTo()
			{
				return sTo;
			}
		};

		public Replace()
		{
			//
			// TODO: Add constructor logic here
			//
			mArrL = new System.Collections.ArrayList();
			mArrH = new System.Collections.ArrayList();
			LoadReplacements();

		}

		public void DumpRules()
		{
			System.Console.WriteLine(" {0} 'light' replacements list:",mArrL.Count);
			foreach(oReplace oRepl in mArrL)
				System.Console.WriteLine("{0} --> {1}", oRepl.GetFrom(), oRepl.GetTo());

			System.Console.WriteLine(" {0} 'hard' replacements list:",mArrH.Count);
			foreach(oReplace oRepl in mArrH)
				System.Console.WriteLine("{0} --> {1}", oRepl.GetFrom(), oRepl.GetTo());
		
		} 

		public void EditRules()
		{
			if(!File.Exists(GetPathToConfig()))
			{
				FileStream oFS = File.Create(GetPathToConfig());
				oFS.Close();
			}
			Process oNotepad = Process.Start(GetPathToConfig());
			oNotepad.WaitForExit();
			LoadReplacements();	// and reload data - wszak byly zmiany...
		}

		public string ReplaceLight(string sFileName)
		{
			foreach(oReplace oRepl in mArrL)
				sFileName = sFileName.Replace(oRepl.GetFrom(),oRepl.GetTo());

			return sFileName;
		}

		public string ReplaceHard(string sFileName)
		{
			foreach(oReplace oRepl in mArrH)
				sFileName = sFileName.Replace(oRepl.GetFrom(),oRepl.GetTo());

			return sFileName;
		}

		private void LoadReplacements()
		{
			// zakladajac, ze Clear zrobi delete na kazdym pointerze
			mArrL.Clear();
			mArrH.Clear();

			if(File.Exists(GetPathToConfig()))
			{
				StreamReader oSR = new StreamReader(GetPathToConfig(),System.Text.Encoding.ASCII);
				string sTmp,sTmp1;
				int iMode = 0; int iInd;

				for(int cLine=1; oSR.Peek() >= 0 ; cLine++)
				{
					sTmp = oSR.ReadLine();
					sTmp1 = sTmp.ToLower();

					switch(sTmp1[0])
					{
						case '[': // sekcja
							if(sTmp1 == "[light]") iMode = 1;
							else if(sTmp1 == "[hard]") iMode = 2;
							else
								System.Console.Error.WriteLine("Rules file line #{0} error: unrecognized section {1}",cLine,sTmp);
							break;
						case '\'': // remark
							break;
						case '#': // remark
							break;
						case '/': // remark
							break;
						case ';': // remark
							break;
						default: // no to podmiana
							if(iMode > 0)
							{
								iInd = sTmp.IndexOf("=");
								if(iInd > 0)
								{
									sTmp1 = sTmp.Substring(0,iInd);
									sTmp = sTmp.Substring(iInd+1);
									switch(iMode)
									{
										case 1:
											mArrL.Add(new oReplace(sTmp1,sTmp));
											break;
										case 2:
											mArrH.Add(new oReplace(sTmp1,sTmp));
											break;
									}
								}
								else
									System.Console.Error.WriteLine("Rules file line #{0} error: no '=' sign ({1})",cLine,sTmp);
							}
							else
								System.Console.Error.WriteLine("Rules file line #{0} error: data without section",cLine);
							break;
					}

				};
			}
		
		}

		private string GetPathToConfig()
		{
			return System.Environment.SystemDirectory + @"\name2joliet.txt"; 
		}
	}
}
