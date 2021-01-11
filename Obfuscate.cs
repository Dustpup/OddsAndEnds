using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ICE_ObFu.CNinja.LuaStructure
{
    partial class Obfuscate
    {
        string code = null;
        string Out = null;
        Dictionary<string, string> ConstantsReplacement = new Dictionary<string, string>();

        public Obfuscate(string DocumentIn)
        {
            code = DocumentIn;

        }
        public async Task Initalize()
        {
            await Task.Run(() => {

                code = FixStrings(code, "[[", "]]");
                code = StripComments(code);

                code = FixStrings(code, "\"", "\"");
                code = FixStrings(code, "\'", "\'");

                ReturnCarriage[] funcs = CarriagePointerChunkAsync(code, "function", "\n", "end", 0, "function", "if").Result;
                ReturnCarriage[] ifst  = CarriagePointerChunkAsync(code, "if"      , "\n", "end", 0, "function", "if").Result;
                ReturnVar[] vars = CarriagePointerDefineAsync(code, "local","}", 0, "function", "local").Result;
            });
        }

        private async Task<ReturnCarriage[]> CarriagePointerChunkAsync(string documentCodeIn, string startingWord, string lineEndingWord,
            string chunkEndingWord, int startIndex, params string[] wordsToIgnore)
        {
            /* This Method is used to find Paticualr Word and pairs
             */
            return await Task.Run(() =>
            {
                #region DEBUG TEST
#if DEBUG && CONSOLE
                Console.Clear();
#endif
                #endregion

                int dynPntrSize   = -1;
                int ignoreCount   = 0;
                                  
                int st_pntr       = -1;
                int st_linePntr   = -1;
                int st_chunkPntr  = -1;

                int en_pntr       = -1;
                int en_linePntr   = -1;
                int en_chunkpnter = -1;

                string pnter      = null;
                string linePnter  = null;
                string chunkPnter = null;

                ReturnCarriage carriage = new ReturnCarriage();
                List<ReturnCarriage> carriageResults = new List<ReturnCarriage>();

                for (st_pntr = startIndex; st_pntr < documentCodeIn.Length; st_pntr++)
                {
                    for (dynPntrSize = startingWord.Length; dynPntrSize > startingWord.Length; dynPntrSize--)
                        if (dynPntrSize < documentCodeIn.Length)
                        {
                            en_pntr = dynPntrSize;
                            break;
                        }
                    pnter = documentCodeIn.Substring(st_pntr, en_pntr - st_pntr);

                    #region DEBUG TEST Pointer
#if DEBUG && CONSOLE
                    Console.SetCursorPosition(0, 0);
                    for (int i = 0; i < 100; i++)
                        Console.Write(new string(' ',100));
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Pointer = [{0}]", pnter);
                    Console.WriteLine(new string('\n', 5));
#endif
                    #endregion

                    if (pnter == startingWord)
                    {
                        for (st_linePntr = st_pntr; st_linePntr < documentCodeIn.Length; st_linePntr++)
                        {
                            en_linePntr = st_linePntr + lineEndingWord.Length;

                            if (documentCodeIn.Substring(st_linePntr, en_linePntr - st_linePntr) == lineEndingWord)
                            {
                                linePnter = documentCodeIn.Substring(st_pntr, en_linePntr - st_pntr);
                                carriage.HeaderLine = documentCodeIn.Substring(st_pntr, en_linePntr - st_pntr);
                                carriage.StartPointer = st_pntr;

                                #region DEBUG TEST Line
#if DEBUG && CONSOLE
                                
                                Console.WriteLine("Line =====>>>> \n {0}", linePnter);
                                Console.WriteLine(new string('\n', 5));
                                Thread.Sleep(1000);
#endif
                                #endregion

                                for (st_chunkPntr = en_linePntr; st_chunkPntr < documentCodeIn.Length; st_chunkPntr++)
                                {
                                    en_chunkpnter = st_chunkPntr + chunkEndingWord.Length;

                                    for (int badWord = 0; badWord < wordsToIgnore.Length; badWord++)
                                        if (documentCodeIn.Substring(st_chunkPntr, en_chunkpnter - st_chunkPntr) == wordsToIgnore[badWord])
                                            ignoreCount++; 


                                    if (documentCodeIn.Substring(st_chunkPntr, en_chunkpnter - st_chunkPntr) == chunkEndingWord && ignoreCount <= 0)
                                    {
                                        chunkPnter = documentCodeIn.Substring(st_pntr, en_chunkpnter - st_pntr);
                                        carriage.Chunk = chunkPnter;
                                        carriage.OldChunk = chunkPnter;

                                        #region DEBUG TEST Chunk
#if DEBUG && CONSOLE
                                        Console.WriteLine("Chunk =====>>>> \n {0}",chunkPnter );
                                        Console.WriteLine(new string('\n', 5));
                                        Thread.Sleep(1000);
#endif
                                        #endregion

                                        break;
                                    }
                                    else if (documentCodeIn.Substring(st_chunkPntr, en_chunkpnter - st_chunkPntr) == chunkEndingWord && ignoreCount > 0)
                                        ignoreCount--;
                                }
                                break; 
                            }
                        }
                    }
                    carriageResults.Add(carriage);
                }
                return carriageResults.ToArray();
            });
        }

        private async Task<ReturnVar[]> CarriagePointerDefineAsync(string documentCodeIn, string startingWord,
            string chunkEndingWord, int startIndex, params string[] wordsToIgnore)
        {
            /* This Method is used to find Paticualr Word and pairs */

            return await Task.Run(() =>
            {
                int dynPntrSize = -1;
                int ignoreCount = 0;

                int st_pntr = -1;
                //int st_linePntr = -1;
                int st_chunkPntr = -1;

                int en_pntr = -1;
                //int en_linePntr = -1;
                int en_chunkpnter = -1;

                string pnter = null;
                //string linePnter = null;
                string chunkPnter = null;

                ReturnVar carriage = new ReturnVar();
                List<ReturnVar> carriageResults = new List<ReturnVar>();

                for (st_pntr = startIndex; st_pntr < documentCodeIn.Length; st_pntr++)
                {
                    for (dynPntrSize = startingWord.Length; dynPntrSize > startingWord.Length; dynPntrSize--)
                        if (dynPntrSize <= documentCodeIn.Length)
                            break;

                    pnter = documentCodeIn.Substring(st_pntr, en_pntr - st_pntr);

                    // Check Starter Word
                    if (pnter == startingWord)
                    {
                        for (int varNamePtr = st_pntr; varNamePtr < documentCodeIn.Length; varNamePtr++)
                            if (documentCodeIn[varNamePtr] == '=')
                                carriage.VaribleName = documentCodeIn.Substring(st_pntr, varNamePtr - st_pntr).Trim(' ', '=');


                        for (st_chunkPntr = st_pntr; st_chunkPntr < documentCodeIn.Length; st_chunkPntr++)
                        {
                            en_chunkpnter = st_chunkPntr + chunkEndingWord.Length;

                            for (int badWord = 0; badWord < wordsToIgnore.Length; badWord++)
                                if (documentCodeIn.Substring(st_chunkPntr, en_chunkpnter - st_chunkPntr) == wordsToIgnore[badWord])
                                    ignoreCount++;

                            // Check End Word
                            if (documentCodeIn.Substring(st_chunkPntr, en_chunkpnter - st_chunkPntr) == chunkEndingWord && ignoreCount <= 0)
                            {
                                chunkPnter = documentCodeIn.Substring(st_pntr, en_chunkpnter - st_pntr);
                                carriage.DefineInfo = chunkPnter;
                                carriage.StartPointer = st_pntr;
                                carriage.EndPointer = en_chunkpnter;

                                break;
                            }
                            else if (documentCodeIn.Substring(st_chunkPntr, en_chunkpnter - st_chunkPntr) == chunkEndingWord && ignoreCount > 0)
                                ignoreCount--;
                        }
                        break;

                    }
                    carriageResults.Add(carriage);
                }
                return carriageResults.ToArray();

            });
        }

        private string FixStrings(string DocumentIn, string startStr, string endStr, bool useEmptyStrings = true)
        {
            int startingQuotesChar = -1;
            int endingQuotesChar = -1;
            bool hasFoundStartingChar = false;
            string FinDocument = DocumentIn;
            string PointerStr = null;

            // Pulls String Lines and fixes them so that they are on one line;
            for (int curs = 0; curs < FinDocument.Length; curs++)
            {
                // Builde a multi pointer for the next check
                // Heres what it looks like 
                // Pointer = "XX"                -- X = the character position
                if (startStr.Length > 1)
                {
                    try { PointerStr = String.Format("{0}{1}", FinDocument[curs], FinDocument[curs + 1]); }
                    catch (IndexOutOfRangeException) { PointerStr = FinDocument[curs].ToString(); }
                }
                else PointerStr = FinDocument[curs].ToString();
                // Big Check Ahead // Checks the multistring pointer for success if not multi string then character. 

                if ((PointerStr == startStr && !hasFoundStartingChar) ||
                    (PointerStr == endStr && hasFoundStartingChar))
                {
                    if (!hasFoundStartingChar)
                    {
                        startingQuotesChar = curs + PointerStr.Length - 1;
                        hasFoundStartingChar = true;
                    }
                    else
                    {
                        endingQuotesChar = curs + PointerStr.Length;

                        string fix = FinDocument.Substring(startingQuotesChar, endingQuotesChar - startingQuotesChar);
                        fix = fix.Replace("\n", useEmptyStrings ? String.Empty : "\\n");
                        fix = fix.Replace("\r", useEmptyStrings ? String.Empty : "\\r");
                        FinDocument = FinDocument.Remove(startingQuotesChar, endingQuotesChar - startingQuotesChar);
                        FinDocument = FinDocument.Insert(startingQuotesChar, fix);

                        hasFoundStartingChar = false;
                    }

                }
            }
#if DEBUG && CONSOLE // BUST A MOVE! 
            Console.WriteLine(FinDocument);
#endif
            return FinDocument;
        }
        private string StripComments(string DocumentIn)
        {
            int startingChar = 0;
            string[] lines = DocumentIn.Split('\n');
            string finDocument = null;
            string pointer = null;

            for (int lineCurs = 0; lineCurs < lines.Length; lineCurs++)
            {
                for (int curs = 0; curs < lines[lineCurs].Length; curs++)
                {
                    try { pointer = String.Format("{0}{1}", lines[lineCurs][curs], lines[lineCurs][curs + 1]); }
                    catch (IndexOutOfRangeException) { pointer = lines[lineCurs][curs].ToString(); }

                    if (pointer == "--")
                    {
                        startingChar = curs;
                        lines[lineCurs] = lines[lineCurs].Remove(startingChar, lines[lineCurs].Length - startingChar);
                    }

                }
                string trimedLine = lines[lineCurs].Trim('\r', '\t', '\n', ' ');

                if (trimedLine != String.Empty)
                    finDocument += lines[lineCurs] + '\n';
            }
            return finDocument;
        }
    }

    /// <summary>
    /// Used for return info when the Carriage pointer function finds something.
    /// </summary>
    struct ReturnCarriage
    {
        public string HeaderLine;
        public string Chunk;
        public string OldChunk;
        public int StartPointer;
        public int EndPointer;
    }
    struct ReturnVar
    {
        public string VaribleName;
        public string DefineInfo;
        public int StartPointer;
        public int EndPointer;
    }
}
